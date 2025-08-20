using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Interfaces;

namespace MinimalAPI.Dominio.Servicos
{
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbContexto _context;

        public VeiculoServico(DbContexto context)
        {
            _context = context;
        }

        public void Apagar(Veiculo veiculo)
        {
            _context.Veiculos.Remove(veiculo);
            _context.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _context.Veiculos.Update(veiculo);
            _context.SaveChanges();
        }

        public Veiculo? BuscaPorId(int id)
        {
            return _context.Veiculos
                .Where(v => v.Id == id)
                .FirstOrDefault();
        }

        public void Incluir(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
        }
        public List<Veiculo> Todos(int pagina = 1, string nome = null, string? marca = null, int? ano = null)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca, StringComparison.OrdinalIgnoreCase));
            }

            if (ano.HasValue)
            {
                query = query.Where(v => v.Ano == ano.Value);
            }

            return query
                .Skip((pagina - 1) * 10)
                .Take(10)
                .ToList();
        }
    }
}