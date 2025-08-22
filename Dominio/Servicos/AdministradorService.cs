using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Interfaces;

namespace MinimalApi.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _context;

        public AdministradorServico(DbContexto context)
        {
            _context = context;
        }

        public Administrador Incluir(Administrador administrador)
        {
            _context.Administradores.Add(administrador);
            _context.SaveChanges();

            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _context.Administradores
                .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)
                .FirstOrDefault();

            return adm;
        }

            public Administrador? BuscaPorId(int id)
        {
            return _context.Administradores
                .Where(v => v.Id == id)
                .FirstOrDefault();
        }

        public List<Administrador> Todos(int? pagina)
        {
            var query = _context.Administradores.AsQueryable();
            int pageNumber = pagina ?? 1;

            return query
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToList();
        }
    }
}
