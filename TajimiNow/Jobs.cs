using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TajimiNow.Jma;
using TajimiNow.Jma.Weather;

namespace TajimiNow
{
    internal class Jobs
    {
        public static async Task RunAmedas()
        {
            var lastTime = DateTime.MinValue;

            while (true)
            {
                if (DateTime.Now - lastTime > TimeSpan.FromMinutes(15))
                {
                    var amedas = await Amedas.Get("52606", DateTime.Now);

                    if (amedas != null)
                    {
                        var text = $"🌡 {amedas.Temperature} ℃ 💨 {amedas.WindSpeed} m/s\n" +
                            $"☀ {amedas.SunshineHours} min/h  🌧 {amedas.Precipitation1h} mm/h\n" +
                            $"({amedas.Time:HH:mm})";
                        try
                        {
                            await MisskeyApi.Post(text, Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY_AMEDAS"));
                            lastTime = amedas.Time;
                            Console.Error.WriteLine($"Successful: {text}");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Failed: {ex}");
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        public static async Task RunForecast()
        {
            var lastDate = DateOnly.MinValue;

            while (true)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (lastDate < today)
                {
                    var forecast = await Forecast.Get("210010", today);
                    if (forecast != null)
                    {
                        var weather = WeatherRegistry.GetFromCode(forecast.WeatherCode);
                        var text = $"{today:MM/dd} 天気予報(美濃地方)\n" +
                            $"{weather.Mfm} {weather.Name}\n" +
                            $"降水確率: {string.Join("→", forecast.Pops)} %\n" +
                            $"気温: ↓{forecast.MinTemperature} ↑{forecast.MaxTemperature} ℃";

                        try
                        {
                            await MisskeyApi.Post(text, Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY_FORECAST"));
                            lastDate = today;
                            Console.Error.WriteLine($"Successful: {text}");
                        }
                        catch(Exception ex)
                        {
                            Console.Error.WriteLine($"Failed: {ex}");
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}
