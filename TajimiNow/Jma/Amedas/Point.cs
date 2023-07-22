using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Amedas
{
    internal class Point
    {
        private static readonly Dictionary<string, Point> points = new();
        private static readonly HttpClient httpClient = new();
        private static readonly string url = "https://www.jma.go.jp/bosai/amedas/const/amedastable.json";
        private static readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static async Task<Point?> GetPointFromCode(string code)
        {
            if (points.ContainsKey(code)) return points[code];
            await Update();
            if (points.ContainsKey(code)) return points[code];
            return null;
        }

        private static async Task Update()
        {
            Dictionary<string, RawData>? rawData;
            try
            {
                var res = await httpClient.GetAsync(url);
                var stream = await res.Content.ReadAsStreamAsync();
                rawData = await JsonSerializer.DeserializeAsync<Dictionary<string, RawData>>(stream, serializerOptions);
            }
            catch (Exception) { return; }
            if (rawData == null) return;

            points.Clear();

            foreach (var (code, data) in rawData)
            {
                points.Add(code, new(code, data.KjName, data.KnName, data.EnName));
            }
        }

        private Point(string code, string name, string kanaName, string englishName)
        {
            Code = code;
            Name = name;
            KanaName = kanaName;
            EnglishName = englishName;
        }

        public string Code { get; }
        public string Name { get; }
        public string KanaName { get; }
        public string EnglishName { get; }

        private record RawData(
            string KjName,
            string KnName,
            string EnName
        );
    }
}
