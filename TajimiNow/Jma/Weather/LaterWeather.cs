using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Weather
{
    internal class LaterWeather : IWeather
    {
        public int Code { get; }
        public string Name { get; }
        public string Mfm => $"$[x2 {Emoji1}]→$[x2 {Emoji2}]";
        public string Emoji1 { get; }
        public string Emoji2 { get; }

        public LaterWeather(int code, string name, string emoji1, string emoji2)
        {
            Code = code;
            Name = name;
            Emoji1 = emoji1;
            Emoji2 = emoji2;
        }
    }
}
