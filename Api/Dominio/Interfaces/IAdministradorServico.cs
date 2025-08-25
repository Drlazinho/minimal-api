using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);
        Administrador Incluir(Administrador administrador);
        List<Administrador> Todos(int? pagina);
        Administrador ? BuscaPorId(int id);
    }
}