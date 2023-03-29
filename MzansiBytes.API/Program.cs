using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MzansiBytes.API.Data;

namespace MzansiBytes.API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            var sqliteConnection = new SqliteConnection($"Filename={builder.Configuration["Database:FileName"]}");
            sqliteConnection.Open();

            builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(sqliteConnection));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(cors => cors.AllowAnyOrigin());

            var scope = app.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetService<AppDbContext>();
            _ = await appDbContext.Database.EnsureCreatedAsync();

            await Seed.AddWeatherForecastData(appDbContext);

            app.MapGet("/weatherforecast", async () =>
            {
                var forecasts = await appDbContext.WeatherForecasts.ToListAsync();

                return forecasts;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}

