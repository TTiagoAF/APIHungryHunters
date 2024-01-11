using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using APIHungryHunters.Models;

namespace APIHungryHunters.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> opcao)
        : base(opcao)
        {
        }

        public DbSet<Contas> Contas { get; set; } = null!;
        public DbSet<Restaurantes> Restaurantes { get; set; } = null!;
        public DbSet<PlantaRestaurante> PlantaRestaurantes { get; set; } = null!;
        public DbSet<RestauranteMenu> RestauranteMenu { get; set; } = null!;
        public DbSet<Empresas> Empresas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlantaRestaurante>()
                .HasOne(ot => ot.Restaurante)
                .WithMany(r => r.PlantaRestaurantes)
                .HasForeignKey(ot => ot.RestauranteId)
                .HasConstraintName("fk_idrestaurante_planta");

            modelBuilder.Entity<RestauranteMenu>()
                .HasOne(ot => ot.Restaurante)
                .WithMany(r => r.RestauranteMenus)
                .HasForeignKey(ot => ot.RestauranteId)
                .HasConstraintName("fk_idrestaurante_pratos");

            modelBuilder.Entity<Restaurantes>()
                .HasOne(ot => ot.Empresas)
                .WithMany(r => r.Restaurantes)
                .HasForeignKey(ot => ot.NipcEmpresa)
                .HasConstraintName("fk_nipcempresa");
        }      
    }
}
