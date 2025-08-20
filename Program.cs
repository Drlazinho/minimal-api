using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.Db;
using MinimalAPI.Dominio.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("mySqlConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mySqlConnection"))          
));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin" && loginDTO.Senha == "password")
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();
