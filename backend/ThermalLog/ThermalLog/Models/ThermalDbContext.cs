using Microsoft.EntityFrameworkCore;
using System;

namespace ThermalLog.Models
{
    public class ThermalDbContext : DbContext
    {
        public ThermalDbContext(DbContextOptions<ThermalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Thermal> Thermals { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }
    }

}