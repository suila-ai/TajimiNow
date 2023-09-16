using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Amedas
{
    internal class Amedas
    {
        private static readonly HttpClient httpClient = new();
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task<Amedas?> Get(string pointCode, DateTime time)
        {
            var point = await Point.GetPointFromCode(pointCode);
            if (point == null) return null;

            var date = time.ToString("yyyyMMdd");
            var floorHour = (time.Hour / 3 * 3).ToString("D2");
            var url = $"https://www.jma.go.jp/bosai/amedas/data/point/{pointCode}/{date}_{floorHour}.json";

            Dictionary<string, RawData>? rawData;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<Dictionary<string, RawData>>(stream, serializerOptions);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var floorMinute = (time.Minute / 10 * 10).ToString("D2");
            var timeKey = $"{time:yyyyMMddHH}{floorMinute}00";
            if (!rawData.ContainsKey(timeKey)) return null;

            var floorTime = DateTime.ParseExact(timeKey, "yyyyMMddHHmmss", null);
            var data = rawData[timeKey];

            return new(point, floorTime, data?.Temp?[0], data?.Precipitation1h?[0], 60 * data?.Sun1h?[0], data?.Wind?[0]);
        }

        public static async IAsyncEnumerable<Amedas> GetDay(string pointCode, DateOnly date)
        {
            var point = await Point.GetPointFromCode(pointCode);
            if (point == null) yield break;

            var dateStr = date.ToString("yyyyMMdd");

            for (var i = 0; i < 8; i++)
            {
                var url = $"https://www.jma.go.jp/bosai/amedas/data/point/{pointCode}/{dateStr}_{i * 3:D2}.json";

                Dictionary<string, RawData>? rawData = null;
                try
                {
                    var res = await httpClient.GetAsync(url);
                    var stream = await res.Content.ReadAsStreamAsync();
                    rawData = await JsonSerializer.DeserializeAsync<Dictionary<string, RawData>>(stream, serializerOptions);
                }
                catch (Exception) { }

                if (rawData != null)
                {
                    foreach (var (timeKey, data) in rawData)
                    {
                        var time = DateTime.ParseExact(timeKey, "yyyyMMddHHmmss", null);
                        yield return new(point, time, data?.Temp?[0], data?.Precipitation1h?[0], 60 * data?.Sun1h?[0], data?.Wind?[0]);
                    }
                }
            }
        }

        private Amedas(Point point, DateTime time, double? temperature, double? precipitation1h, double? sunshineHours, double? windSpeed)
        {
            Point = point;
            Time = time;
            Temperature = temperature;
            Precipitation1h = precipitation1h;
            SunshineHours = sunshineHours;
            WindSpeed = windSpeed;
        }

        public Point Point { get; }
        public DateTime Time { get; }
        public double? Temperature { get; }
        public double? Precipitation1h { get; }
        public double? SunshineHours { get; }
        public double? WindSpeed { get; }

        private record RawData(
            IReadOnlyList<double?>? Temp,
            IReadOnlyList<double?>? Sun1h,
            IReadOnlyList<double?>? Precipitation1h,
            IReadOnlyList<double?>? Wind
        );
    }
}
