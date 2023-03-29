using Microsoft.EntityFrameworkCore;

namespace MzansiBytes.API.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
}
