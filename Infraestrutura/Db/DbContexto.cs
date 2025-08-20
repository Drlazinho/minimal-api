using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        public readonly IConfiguration _configurationAppSettings;
        public DbContexto(DbContextOptions<DbContexto> options, IConfiguration configuracaoAppSettings)
            : base(options)
        {
            _configurationAppSettings = configuracaoAppSettings;
        }

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        // Configuração do modelo e dados iniciais - Seed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>()
                .HasData(new Administrador
                {
                    Id = 1,
                    Senha = "password",
                    Email = "email@teste.com",
                    Perfil = "Admin"
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConnection = _configurationAppSettings.GetConnectionString("mySqlConnection")?.ToString();
            if (string.IsNullOrEmpty(stringConnection))
            {
                optionsBuilder.UseMySql(stringConnection, ServerVersion.AutoDetect(stringConnection));
            }

        }
    }
}