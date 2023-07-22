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
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task<Forecast?> Get(string officeCode, string areaCode, DateOnly date)
        {
            var url = $"https://www.jma.go.jp/bosai/forecast/data/forecast/{officeCode}.json";
            IReadOnlyList<RawData>? rawData = null;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<IReadOnlyList<RawData>>(stream, serializerOptions);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var timeSeries = rawData[0].TimeSeries;
            var areaIndex = timeSeries[0].Areas.Indexes(e => e.Area.Code == areaCode).FirstOrDefault(-1);
            var weatherIndex = timeSeries[0].TimeDefines.Indexes(e => DateOnly.FromDateTime(e) == date).FirstOrDefault(-1);
            if (areaIndex == -1 || weatherIndex == -1) return null;

            var areaData = timeSeries[0].Areas[areaIndex];
            var areaName = areaData.Area.Name;
            var weatherCode = int.Parse(areaData.WeatherCodes[weatherIndex]);
            var weather = areaData.Weathers[weatherIndex];

            var pops = timeSeries[1].TimeDefines.Indexes(e => DateOnly.FromDateTime(e) == date).Select(e => int.Parse(timeSeries[1].Areas[areaIndex].Pops[e])).ToList();
            if (pops.Count == 0) return null;

            var minTempIndex = timeSeries[2].TimeDefines.Indexes(e => e == date.ToDateTime(new(0, 0))).FirstOrDefault(-1);
            var maxTempIndex = timeSeries[2].TimeDefines.Indexes(e => e == date.ToDateTime(new(9, 0))).FirstOrDefault(-1);
            if (minTempIndex== -1 || maxTempIndex == -1) return null;
            var minTemp = int.Parse(timeSeries[2].Areas[areaIndex].Temps[minTempIndex]);
            var maxTemp = int.Parse(timeSeries[2].Areas[areaIndex].Temps[maxTempIndex]);

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
            IReadOnlyList<TimeSeries> TimeSeries
        );

        private record TimeSeries(
            IReadOnlyList<DateTime> TimeDefines,
            IReadOnlyList<AreaData> Areas
        );

        private record AreaData(
             AreaInfo Area,
             IReadOnlyList<string> WeatherCodes,
             IReadOnlyList<string> Weathers,
             IReadOnlyList<string> Pops,
             IReadOnlyList<string> Temps
        );

        private record AreaInfo(
            string Name,
            string Code
        );
    }
}
