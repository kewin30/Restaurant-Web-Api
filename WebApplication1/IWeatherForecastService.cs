using System.Collections.Generic;

namespace WebApplication1
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get(int count, int min, int max);
    }
}
