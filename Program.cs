using InmobiliariaLab3.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();  // Si tienes autenticación

app.MapControllers();  // Mapear los controladores

app.Run();
