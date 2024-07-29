using Exchangeapi.Models;
using Microsoft.EntityFrameworkCore;

namespace Exchangeapi.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.BaseCurrency)
                .WithMany() 
                .HasForeignKey(e => e.BaseCurrency)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ExchangeRate>()
                .HasOne(e => e.TargetCurrency)
                .WithMany() 
                .HasForeignKey(e => e.TargetCurrency)
                .OnDelete(DeleteBehavior.Restrict); 
        }

    }
}
