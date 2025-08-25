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
using MinimalApi.Dominio.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();

if (!string.IsNullOrEmpty(key))
{
    builder.Configuration["Jwt"] = key;
}

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira. Bearer {seu token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{}
    } });
});

builder.Services.AddDbContext<DbContexto>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("mySqlConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mySqlConnection"))
));

var app = builder.Build();


// Configure the HTTP request pipeline
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key))
        return string.Empty;

    var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);

    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }

}).AllowAnonymous().WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    return Results.Ok(administradorServico.Todos(pagina));
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{

    var administrador = administradorServico.BuscaPorId(id);
    if (administrador == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(administrador);
}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao();
    if (string.IsNullOrEmpty(administradorDTO.Senha) || string.IsNullOrEmpty(administradorDTO.Email) || string.IsNullOrEmpty(administradorDTO.Perfil.ToString()))
    {
        validacao.Mensagem = new List<string> { "Nome, Email e Senha são obrigatórios." };
        return Results.BadRequest(validacao);
    }

    var administradores = new Administrador
    {
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.editor.ToString(),
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha
    };

    administradorServico.Incluir(administradores);
    return Results.Created($"/administrador/{administradores.Id}", administradores);


}).RequireAuthorization().WithTags("Administradores");
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = new ErrosDeValidacao();
    if (string.IsNullOrEmpty(veiculoDTO.Nome) || string.IsNullOrEmpty(veiculoDTO.Marca) || veiculoDTO.Ano <= 0)
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
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos", (int? pagina, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.BuscaPorId(id);
    if (veiculos == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veiculos");

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
}).RequireAuthorization().WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{

    var veiculos = veiculoServico.BuscaPorId(id);
    if (veiculos == null)
    {
        return Results.NotFound();
    }


    veiculoServico.Apagar(veiculos);

    return Results.NoContent();
}).RequireAuthorization().WithTags("Veiculos");
#endregion

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseAuthentication();
app.UseAuthorization();
app.Run();
