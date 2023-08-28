using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Warning
{
    internal class Warnings
    {
        private static readonly HttpClient httpClient = new();
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task<Warnings?> Get(string officeCode, string areaCode)
        {
            var url = $"https://www.jma.go.jp/bosai/warning/data/warning/{officeCode}.json";
            RawData? rawData = null;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<RawData>(stream, serializerOptions);
            }
            catch (Exception) { return null; }
            if (rawData == null) return null;

            var warnings = rawData.AreaTypes[1].Areas.FirstOrDefault(e => e.Code == areaCode)?.Warnings;
            if (warnings == null) return null;

            var activeWarnings = warnings.Where(e => e.Code != null).ToList();

            var result = new Warnings(
                rawData.ReportDatetime,
                activeWarnings.Where(e => e.Status != "継続" && e.Status != "解除").Select(e => Warning.GetFromCode(e.Code!)).ToList(),
                activeWarnings.Where(e => e.Status == "継続").Select(e => Warning.GetFromCode(e.Code!)).ToList(),
                activeWarnings.Where(e => e.Status == "解除").Select(e => Warning.GetFromCode(e.Code!)).ToList()
            );

            return result;
        }

        public DateTime Time { get; }

        public IReadOnlyList<Warning> New { get; }
        public IReadOnlyList<Warning> Continue { get; }
        public IReadOnlyList<Warning> End { get; }

        private Warnings(DateTime time, IReadOnlyList<Warning> @new, IReadOnlyList<Warning> @continue, IReadOnlyList<Warning> end)
        {
            Time = time;
            New = @new;
            Continue = @continue;
            End = end;
        }
        
        private record RawData(
            DateTime ReportDatetime,
            IReadOnlyList<AreaType> AreaTypes
        );

        private record AreaType(
            IReadOnlyList<Area> Areas
        );

        private record Area(
            string Code,
            IReadOnlyList<RawWarning> Warnings
        );

        private record RawWarning(
            string? Code,
            string Status
        );
    }
}
