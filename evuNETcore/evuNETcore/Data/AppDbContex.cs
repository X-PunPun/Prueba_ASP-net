using evuNETcore.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace evuNETcore.Data
{
    public class AppDbContex : DbContext
    {
        public AppDbContex(DbContextOptions<AppDbContex> options) : base(options) { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar Symbol como índice único
            modelBuilder.Entity<Currency>()
                .HasIndex(c => c.Symbol)
                .IsUnique();

            // Relación BaseCurrency -> Currency
            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.BaseCurrencyObj)
                .WithMany(c => c.RatesFrom) // Asegúrate que Currency.cs tenga esta propiedad, si no pon .WithMany()
                .HasForeignKey(e => e.BaseCurrency)
                .HasPrincipalKey(c => c.Symbol)
                .OnDelete(DeleteBehavior.Restrict); // <--- ESTO ES LA SOLUCIÓN (Evita el borrado en cascada)

            // Relación TargetCurrency -> Currency
            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.TargetCurrencyObj)
                .WithMany(c => c.RatesTo) // Asegúrate que Currency.cs tenga esta propiedad, si no pon .WithMany()
                .HasForeignKey(e => e.TargetCurrency)
                .HasPrincipalKey(c => c.Symbol)
                .OnDelete(DeleteBehavior.Restrict); // <--- ESTO ES LA SOLUCIÓN
        }
    }
}
