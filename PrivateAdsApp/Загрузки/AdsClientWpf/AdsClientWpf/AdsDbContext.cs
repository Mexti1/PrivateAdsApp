using AdsClientWpf.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;

namespace AdsClientWpf.Data
{
    public class AdsDbContext : DbContext
    {
        public AdsDbContext(DbContextOptions<AdsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Ad> Ads { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<AdType> AdTypes { get; set; } = null!;
        public DbSet<AdStatus> AdStatuses { get; set; } = null!;
        public DbSet<Photo> Photos { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // констрейнты и конфигурация при необходимости
            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.AdId)
                .IsUnique();

            // Если надо: seed данных можно оставить пустым (мы уже создали через SSMS)
        }
    }
}
