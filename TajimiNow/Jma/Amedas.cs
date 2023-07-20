using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow.Jma
{
    internal class Amedas
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<Amedas?> Get(string point, DateTime time)
        {
            var date = time.ToString("yyyyMMdd");
            var floorHour = (time.Hour / 3 * 3).ToString("D2");
            var url = $"https://www.jma.go.jp/bosai/amedas/data/point/{point}/{date}_{floorHour}.json";

            Dictionary<string, RawData>? rawData;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<Dictionary<string, RawData>>(stream);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var floorMinute = (time.Minute / 10 * 10).ToString("D2");
            var timeKey = $"{time:yyyyMMddHH}{floorMinute}00";
            if (!rawData.ContainsKey(timeKey)) return null;
            var data = rawData[timeKey];
            var floorTime = DateTime.ParseExact(timeKey, "yyyyMMddHHmmss", null);

            return new(floorTime, data.temp[0], data.precipitation1h[0], 60 * data.sun1h[0], data.wind[0]);
        }

        public static async IAsyncEnumerable<Amedas> GetDay(string point, DateOnly date)
        {
            var dateStr = date.ToString("yyyyMMdd");

            for (var i = 0; i < 8; i++)
            {
                var url = $"https://www.jma.go.jp/bosai/amedas/data/point/{point}/{dateStr}_{i * 3:D2}.json";

                Dictionary<string, RawData>? rawData = null;
                try
                {
                    var res = await httpClient.GetAsync(url);
                    var stream = await res.Content.ReadAsStreamAsync();
                    rawData = await JsonSerializer.DeserializeAsync<Dictionary<string, RawData>>(stream);
                }
                catch (Exception) {}

                if (rawData != null)
                {
                    foreach (var (timeKey, data) in rawData)
                    {
                        var time = DateTime.ParseExact(timeKey, "yyyyMMddHHmmss", null);
                        yield return new(time, data.temp[0], data.precipitation1h[0], 60 * data.sun1h[0], data.wind[0]);
                    }
                }
            }
        }

        private Amedas(DateTime time, double temperature, double precipitation1h, double sunshineHours, double windSpeed)
        {
            Time = time;
            Temperature = temperature;
            Precipitation1h = precipitation1h;
            SunshineHours = sunshineHours;
            WindSpeed = windSpeed;
        }

        public DateTime Time { get; }
        public double Temperature { get; }
        public double Precipitation1h { get; }
        public double SunshineHours { get; }
        public double WindSpeed { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006")]
        private record RawData(
            IReadOnlyList<double> temp,
            IReadOnlyList<double> sun1h,
            IReadOnlyList<double> precipitation1h,
            IReadOnlyList<double> wind
        );
    }
}
