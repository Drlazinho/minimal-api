using System.Text.Json.Serialization;
using MinimalApi.Dominio.Enums;

namespace MinimalApi.Dominio.DTOs;
public class AdministradorDTO
{
    public string Senha { get; set; } = default;
    public string Email { get; set; } = default;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Perfil? Perfil { get; set; } = default;
}
