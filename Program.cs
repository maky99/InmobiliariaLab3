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

app.UseHttpsRedirection();
app.UseStaticFiles();  // Si tienes archivos estáticos como CSS, JS, etc.
app.UseRouting();

app.UseAuthentication();  // Habilitar autenticación antes de la autorización
app.UseAuthorization();  // Si tienes autenticación

app.MapControllers();  // Mapear los controladores

app.Run();
