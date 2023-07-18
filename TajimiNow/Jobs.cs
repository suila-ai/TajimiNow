﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TajimiNow.Jma;
using TajimiNow.Jma.Weather;
using TajimiNow.Misskey;

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
                            await Api.Post(new(text, Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY_AMEDAS") ?? "specified"));
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

        public static async Task RunDaily()
        {
            var lastDate = DateOnly.MinValue;
            var succeededMaxTemp = false;
            var succeededForecast = false;

            while (true)
            {
                var overwriteDate = Environment.GetEnvironmentVariable("MISSKEY_FORECAST_OVERWRITE_DATE");
                var today = overwriteDate == null ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.Parse(overwriteDate);

                if (lastDate < today)
                {
                    succeededMaxTemp = false;
                    succeededForecast = false;
                    lastDate = today;
                }

                if (!succeededMaxTemp) succeededMaxTemp = await RunMaxTemp(today.AddDays(-1));
                if (!succeededForecast) succeededForecast = await RunForecast(today);

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        private static async Task<bool> RunForecast(DateOnly date)
        {
            var forecast = await Forecast.Get("210010", date);
            if (forecast == null) return false;
            var weather = WeatherRegistry.GetFromCode(forecast.WeatherCode);
            var text = $"{date:MM/dd} 天気予報(美濃地方)\n" +
                $"{weather.Mfm} {weather.Name}\n" +
                $"降水確率: {string.Join("→", forecast.Pops)} %\n" +
                $"気温: ↓{forecast.MinTemperature} ↑{forecast.MaxTemperature} ℃";

            var note = new Note(text, Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY_FORECAST") ?? "specified");

            var tajimiChanceFile = Environment.GetEnvironmentVariable("MISSKEY_TAJIMI_CHANCE_FILE");
            if (forecast.MaxTemperature > 35.3 && tajimiChanceFile != null)
            {
                note = note.AddFiles(tajimiChanceFile);
            }

            try
            {
                await Api.Post(note);
                Console.Error.WriteLine($"Successful: {text}");
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed: {ex}");
                return false;
            }

        }

        private static async Task<bool> RunMaxTemp(DateOnly date)
        {
            var maxAmedas = await Amedas.GetDay("52606", date).MaxByAsync(e => e.Temperature);
            if (maxAmedas == null) return false;
            var text = $"昨日({date:MM/dd})の最高気温🌡\n" +
                $"{maxAmedas.Temperature} ℃ ({maxAmedas.Time:HH:mm})";
            var note = new Note(text, Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY_MAX_TEMP") ?? "specified");

            var tajimiAchievedFile = Environment.GetEnvironmentVariable("MISSKEY_TAJIMI_ACHIEVED_FILE");
            if (maxAmedas.Temperature > 35.3 && tajimiAchievedFile != null)
            {
                note = note.AddFiles(tajimiAchievedFile);
            }

            try
            {
                await Api.Post(note);
                Console.Error.WriteLine($"Successful: {text}");
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed: {ex}");
                return false;
            }
        }
    }
}
