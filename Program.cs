using MinimalApi.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("mySqlConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mySqlConnection"))          
));

var app = builder.Build();


// Configure the HTTP request pipeline
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administradores");

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = new ErrosDeValidacao();
    if(string.IsNullOrEmpty(veiculoDTO.Nome) || string.IsNullOrEmpty(veiculoDTO.Marca) || veiculoDTO.Ano <= 0)
    {
        validacao.Mensagem = new List<string> { "Nome, Marca e Ano são obrigatórios." };
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

#region Veiculos
app.MapGet("/veiculos", (int? pagina, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.BuscaPorId(id);
    if (veiculos == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.BuscaPorId(id);
    if (veiculos == null)
    {
        return Results.NotFound();
    }

    veiculos.Nome = veiculoDTO.Nome;
    veiculos.Marca = veiculoDTO.Marca;
    veiculos.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculos);

    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.BuscaPorId(id);
    if (veiculos == null)
    {
        return Results.NotFound();
    }


    veiculoServico.Apagar(veiculos);

    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});
app.Run();
