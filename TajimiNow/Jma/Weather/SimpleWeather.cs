using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Weather
{
    internal class SimpleWeather : IWeather
    {

        public int Code { get; }
        public string Name { get; }
        public string Mfm => $"$[x2 {Emoji}]";
        public string Emoji { get; }

        public SimpleWeather(int code, string name, string emoji)
        {
            Code = code;
            Name = name;
            Emoji = emoji;
        }
    }
}
