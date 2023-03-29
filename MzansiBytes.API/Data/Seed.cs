using Microsoft.EntityFrameworkCore;

namespace MzansiBytes.API.Data
{
    public static class Seed
    {
       public static async Task AddWeatherForecastData(AppDbContext appDbContext)
        {
            if (await appDbContext.WeatherForecasts.AnyAsync())
            {
                return;
            }

            var forecasts = new List<WeatherForecast>
            {
                new WeatherForecast
                {
                    Date = new DateOnly(2023, 3, 28),
                    TemperatureC = -16,
                    Summary = "Chilly"
                },
                new WeatherForecast
                {
                    Date = new DateOnly(2023, 3, 29),
                    TemperatureC = -19,
                    Summary = "Balmy"
                },
                new WeatherForecast
                {
                    Date = new DateOnly(2023, 3, 30),
                    TemperatureC = 48,
                    Summary = "Hot"
                },
                new WeatherForecast
                {
                    Date = new DateOnly(2023, 3, 31),
                    TemperatureC = 4,
                    Summary = "Hot"
                },
                new WeatherForecast
                {
                    Date = new DateOnly(2023, 4, 1),
                    TemperatureC = -20,
                    Summary = "Freezing"
                },
            };

            await appDbContext.WeatherForecasts.AddRangeAsync(forecasts);

            appDbContext.SaveChanges();
        }
    }
}
