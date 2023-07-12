using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow
{
    internal class Amedas
    {
        private static HttpClient httpClient = new HttpClient();

        public static async Task<Amedas?> Get(int Point, DateTime time)
        {
            var date = time.ToString("yyyyMMdd");
            var floorHour = (time.Hour / 3 * 3).ToString("D2");
            var url = $"https://www.jma.go.jp/bosai/amedas/data/point/{Point}/{date}_{floorHour}.json";

            SortedDictionary<string, RawData>? rawData = null;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<SortedDictionary<string, RawData>>(stream);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var floorMinute = (time.Minute / 10 * 10).ToString("D2");
            var timeKey = $"{time.ToString("yyyyMMddHH")}{floorMinute}00";
            if (!rawData.ContainsKey(timeKey)) return null;
            var data = rawData[timeKey];
            var floorTime = DateTime.ParseExact(timeKey, "yyyyMMddHHmmss", null);

            return new(floorTime, data.temp[0], data.precipitation1h[0], 60 * data.sun1h[0], data.wind[0]);
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

         private record RawData(
             IReadOnlyList<double> temp,
             IReadOnlyList<double> sun1h,
             IReadOnlyList<double> precipitation1h,
             IReadOnlyList<double> wind
         );
    }
}
