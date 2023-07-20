using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow.Jma
{
    internal class Forecast
    {
        private static readonly HttpClient httpClient = new();

        public static async Task<Forecast?> Get(string point, DateOnly date)
        {
            var prefCode = point[..2];
            var url = $"https://www.jma.go.jp/bosai/forecast/data/forecast/{prefCode}0000.json";
            IReadOnlyList<RawData>? rawData = null;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<IReadOnlyList<RawData>>(stream);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var timeSeries = rawData[0].timeSeries;
            var areaIndex = timeSeries[0].areas.Indexes(e => e.area.code == point).FirstOrDefault(-1);
            var weatherIndex = timeSeries[0].timeDefines.Indexes(e => DateOnly.FromDateTime(e) == date).FirstOrDefault(-1);
            if (areaIndex == -1 || weatherIndex == -1) return null;

            var areaData = timeSeries[0].areas[areaIndex];
            var areaName = areaData.area.name;
            var weatherCode = int.Parse(areaData.weatherCodes[weatherIndex]);
            var weather = areaData.weathers[weatherIndex];

            var pops = timeSeries[1].timeDefines.Indexes(e => DateOnly.FromDateTime(e) == date).Select(e => int.Parse(timeSeries[1].areas[areaIndex].pops[e])).ToList();
            if (pops.Count == 0) return null;

            var minTempIndex = timeSeries[2].timeDefines.Indexes(e => e == date.ToDateTime(new(0, 0))).FirstOrDefault(-1);
            var maxTempIndex = timeSeries[2].timeDefines.Indexes(e => e == date.ToDateTime(new(9, 0))).FirstOrDefault(-1);
            if (minTempIndex== -1 || maxTempIndex == -1) return null;
            var minTemp = int.Parse(timeSeries[2].areas[areaIndex].temps[minTempIndex]);
            var maxTemp = int.Parse(timeSeries[2].areas[areaIndex].temps[maxTempIndex]);

            return new(areaName, weatherCode, weather, pops, minTemp, maxTemp);
        }

        private Forecast(string areaName, int weatherCode, string weather, IReadOnlyList<int> pops, int minTemperature, int maxTemperature)
        {
            AreaName = areaName;
            WeatherCode = weatherCode;
            Weather = weather;
            Pops = pops;
            MinTemperature = minTemperature;
            MaxTemperature = maxTemperature;
        }

        public string AreaName { get; }
        public int WeatherCode { get; }
        public string Weather { get; }
        public IReadOnlyList<int> Pops { get; }
        public int MinTemperature { get; }
        public int MaxTemperature { get; }

        private record RawData(
            IReadOnlyList<TimeSeries> timeSeries
        );

        private record TimeSeries(
            IReadOnlyList<DateTime> timeDefines,
            IReadOnlyList<Area> areas
        );

        private record Area(
             AreaInfo area,
             IReadOnlyList<string> weatherCodes,
             IReadOnlyList<string> weathers,
             IReadOnlyList<string> pops,
             IReadOnlyList<string> temps
        );

        private record AreaInfo(
            string name,
            string code
        );
    }
}
