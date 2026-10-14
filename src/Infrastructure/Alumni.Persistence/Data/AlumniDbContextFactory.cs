using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Alumni.Persistence.Data
{
    public class AlumniDbContextFactory : IDesignTimeDbContextFactory<AlumniDbContext>
    {
        public AlumniDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AlumniDbContext>();

            // Öncelik: Ortam değişkeni (ConnectionStrings__DefaultConnection veya POSTGRES_*), yoksa localhost varsayılanı
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
                var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
                var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "alumnidb";
                var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
                var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";

                connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new AlumniDbContext(optionsBuilder.Options);
        }
    }
}
