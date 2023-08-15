using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TajimiNow.Jma;
using TajimiNow.Jma.Amedas;
using TajimiNow.Jma.Warning;
using TajimiNow.Jma.Weather;
using TajimiNow.Misskey;
using TajimiNow.Misskey.Models;

namespace TajimiNow
{
    internal class Jobs
    {
        private static readonly Regex forecastRegex = new("^[0-9]{2}/[0-9]{2} 天気予報\\(.*\\)\n");
        private static readonly Regex minMaxRegex = new("^昨日\\([0-9]{2}/[0-9]{2}\\)の気温\\(.*\\)\n");

        public static async Task RunAmedas()
        {
            var lastTime = DateTime.MinValue;

            var pointCode = EnvVar.AmedasPointCode;
            if (pointCode == null) return;

            while (true)
            {
                if (DateTime.Now - lastTime > TimeSpan.FromMinutes(15))
                {
                    var amedas = await Amedas.Get(pointCode, DateTime.Now);

                    if (amedas != null)
                    {
                        var text = $"{ValueFormat("🌡", amedas.Temperature, "℃")} {ValueFormat("💨", amedas.WindSpeed, "m/s")}\n" +
                            $"{ValueFormat("☀", amedas.SunshineHours, "min/h")} {ValueFormat("🌧", amedas.Precipitation1h, "mm/h")}";

                        if (EnvVar.ForecastOfficeCode != null && EnvVar.WarningAreaCode != null)
                        {
                            var warnings = await Warnings.Get(EnvVar.ForecastOfficeCode, EnvVar.WarningAreaCode);
                            if (warnings != null)
                            {
                                var warningsText = string.Join(" ", Enumerable.Concat(warnings.New, warnings.Continue)
                                    .OrderBy(e => e)
                                    .GroupBy(e => e.Level)
                                    .Select(e => $"$[bg.color={e.Key.BackgroundColor.ToHex()} $[fg.color={e.Key.ForegroundColor.ToHex()} {string.Join("・", e.Select(e => e.Type))}{e.Key.Name}]]"));
                                text += "\n" + warningsText;
                            }
                        }

                        text = EnvVar.RegexReplace.Aggregate(text, (a, b) => b.pattern.Replace(a, b.replacement));
                        var footer = $"({amedas.Point.Name} {amedas.Time:HH:mm})";

                        try
                        {
                            await Api.Post(new($"{text}\n{footer}", EnvVar.AmedasVisibility));
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

        private static string ValueFormat<T>(string header, T? value, string unit)
        {
            if (value == null) return $"{header} 休止中";
            return $"{header} {value} {unit}";
        }

        public static async Task RunDaily()
        {
            var overwriteDate = EnvVar.ForecastOverwriteDate;
            var lastDate =  overwriteDate == null ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.MinValue;
            var succeededMaxTemp = true;
            var succeededForecast = true;

            while (true)
            {
                var today = overwriteDate == null ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.Parse(overwriteDate);

                if (lastDate < today)
                {
                    succeededMaxTemp = false;
                    succeededForecast = false;
                    lastDate = today;
                }

                if (!succeededMaxTemp) succeededMaxTemp = await RunMinMaxTemp(today.AddDays(-1));
                if (!succeededForecast) succeededForecast = await RunForecast(today);

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        private static async Task<bool> RunForecast(DateOnly date)
        {
            var officeCode = EnvVar.ForecastOfficeCode;
            var areaCode = EnvVar.ForecastAreaCode;
            if (officeCode == null || areaCode == null) return true;

            var forecast = await Forecast.Get(officeCode, areaCode, date);
            if (forecast == null) return false;
            var weather = WeatherRegistry.GetFromCode(forecast.WeatherCode);

            var header = $"{date:MM/dd} 天気予報({forecast.AreaName})";
            var text = $"{weather.Mfm} {weather.Name}\n" +
                $"降水確率: {string.Join("→", forecast.Pops)} %\n" +
                $"気温: ↓{forecast.MinTemperature} ↑{forecast.MaxTemperature} ℃";
            text = EnvVar.RegexReplace.Aggregate(text, (a, b) => b.pattern.Replace(a, b.replacement));

            var note = new PostNote($"{header}\n{text}", EnvVar.ForecastVisibility);

            var maxChanceFile = EnvVar.MaxChanceFile;
            if (forecast.MaxTemperature > EnvVar.MaxChanceThreshold && maxChanceFile != null)
            {
                note = note.AddFiles(maxChanceFile);
            }
            var minChanceFile = EnvVar.MinChanceFile;
            if (forecast.MinTemperature < EnvVar.MinChanceThreshold && minChanceFile != null)
            {
                note = note.AddFiles(minChanceFile);
            }

            try
            {
                await RegexUnpin(forecastRegex);
                var result = await Api.Post(note);
                await Api.Pin(result.Id);

                Console.Error.WriteLine($"Successful: {text}");
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed: {ex}");
                return false;
            }
        }

        private static async Task<bool> RunMinMaxTemp(DateOnly date)
        {
            var pointCode = EnvVar.AmedasPointCode;
            if (pointCode == null) return true;

            var amedas = await Amedas.GetDay(pointCode, date).ToArrayAsync();
            var maxAmedas = amedas.MaxBy(e => e.Temperature);
            var minAmedas = amedas.MinBy(e => e.Temperature);
            if (maxAmedas?.Temperature == null || minAmedas?.Temperature == null) return false;

            var header = $"昨日({date:MM/dd})の気温({maxAmedas.Point.Name})";
            var text = $"最高: {maxAmedas.Temperature} ℃ ({maxAmedas.Time:HH:mm})\n" +
                $"最低: {minAmedas.Temperature} ℃ ({minAmedas.Time:HH:mm})";
            text = EnvVar.RegexReplace.Aggregate(text, (a, b) => b.pattern.Replace(a, b.replacement));

            var note = new PostNote($"{header}\n{text}", EnvVar.MinMaxTempVisibility);

            var maxAchievedFile = EnvVar.MaxAchievedFile;
            if (maxAmedas.Temperature > EnvVar.MaxChanceThreshold && maxAchievedFile != null)
            {
                note = note.AddFiles(maxAchievedFile);
            }
            var minAchievedFile = EnvVar.MinAchievedFile;
            if (minAmedas.Temperature < EnvVar.MinChanceThreshold && minAchievedFile != null)
            {
                note = note.AddFiles(minAchievedFile);
            }

            try
            {
                await RegexUnpin(minMaxRegex);
                var result = await Api.Post(note);
                await Api.Pin(result.Id);

                Console.Error.WriteLine($"Successful: {text}");
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed: {ex}");
                return false;
            }
        }

        private static async Task RegexUnpin(Regex regex)
        {
            try
            {
                var pinned = await Api.GetPinnedNotes();
                foreach (var note in pinned)
                {
                    if (note.Text != null && regex.IsMatch(note.Text)) await Api.Unpin(note.Id);
                }
            }
            catch (Exception) { throw; }
        }
    }
}
