using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Weather
{
    internal interface IWeather
    {
        int Code { get; }
        string Name { get; }
        string Mfm { get; }
    }
}
