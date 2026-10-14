using Alumni.Application.Interfaces;
using Alumni.Persistence.Data;
using Alumni.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alumni.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                   ?? "Host=localhost;Port=5432;Database=alumnidb;Username=postgres;Password=postgres;";

            services.AddDbContext<AlumniDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Veritabanı destekli CRUD Servisleri
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();

            return services;
        }
    }
}
