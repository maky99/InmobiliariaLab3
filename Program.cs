using System.Security.Claims;
using System.Text;
using InmobiliariaLab3.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.WebHost.UseUrls("http://localhost:5027", "http://*:5027");

// Añadir autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["TokenAuthentication:Issuer"],
        ValidAudience = configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                configuration["TokenAuthentication:SecretKey"])),
    };
    // opción extra para usar el token en el hub y otras peticiones sin encabezado (enlaces, src de img, etc.)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Leer el token desde el query string
            var accessToken = context.Request.Query["access_token"];
            // Si el request es para el Hub u otra ruta seleccionada...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatsegurohub") ||
                path.StartsWithSegments("/api/propietarios/reset") ||
                path.StartsWithSegments("/api/propietarios/token") ||
                 path.StartsWithSegments("/api/propietarios/mail&token")))
            {//reemplazar las urls por las necesarias ruta ⬆
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Añadir autorización con políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Administrador"));
});

// Agregar servicios al contenedor
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar el contexto de base de datos con MySQL
var connectionString = builder.Configuration.GetConnectionString("MySql");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Agregar soporte para controladores y vistas (si los tienes)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();//se tiene que desactivar para que corra en el celular 
app.UseStaticFiles();  // Si tienes archivos estáticos como CSS, JS, etc.
app.UseRouting();

app.UseAuthentication();  // Habilitar autenticación antes de la autorización
app.UseAuthorization();  // Si tienes autenticación

app.MapControllers();  // Mapear los controladores

app.Run();
