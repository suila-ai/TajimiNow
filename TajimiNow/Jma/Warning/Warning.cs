using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Warning
{
    internal class Warning : IComparable<Warning>
    {

        public string Code { get; }
        public string Type { get; }
        public Level Level { get; }

        private Warning(string code, string type, Level level)
        {
            Code = code;
            Type = type;
            Level = level;
        }
        public int CompareTo(Warning? other)
        {
            if (other == null) return 1;
            if (Level != other.Level) return other.Level.CompareTo(Level);
            return Code.CompareTo(other.Code);
            
        }

        private static readonly Dictionary<string, Warning> warnings = new()
        {
            {"02", new("02", "暴風雪", Level.GetFromNumber(2))},
            {"03", new("03", "大雨", Level.GetFromNumber(2))},
            {"04", new("04", "洪水", Level.GetFromNumber(2))},
            {"05", new("05", "暴風", Level.GetFromNumber(2))},
            {"06", new("06", "大雪", Level.GetFromNumber(2))},
            {"07", new("07", "波浪", Level.GetFromNumber(2))},
            {"08", new("08", "高潮", Level.GetFromNumber(2))},

            {"10", new("10", "大雨", Level.GetFromNumber(1))},
            {"12", new("12", "大雪", Level.GetFromNumber(1))},
            {"13", new("13", "風雪", Level.GetFromNumber(1))},
            {"14", new("14", "雷", Level.GetFromNumber(1))},
            {"15", new("15", "強風", Level.GetFromNumber(1))},
            {"16", new("16", "波浪", Level.GetFromNumber(1))},
            {"17", new("17", "融雪", Level.GetFromNumber(1))},
            {"18", new("18", "洪水", Level.GetFromNumber(1))},
            {"19", new("19", "高潮", Level.GetFromNumber(1))},
            {"19+", new("19+", "高潮", Level.GetFromNumber(1))},
            {"20", new("20", "濃霧", Level.GetFromNumber(1))},
            {"21", new("21", "乾燥", Level.GetFromNumber(1))},
            {"22", new("22", "なだれ", Level.GetFromNumber(1))},
            {"23", new("23", "低温", Level.GetFromNumber(1))},
            {"24", new("24", "霜", Level.GetFromNumber(1))},
            {"25", new("25", "着氷", Level.GetFromNumber(1))},
            {"26", new("26", "着雪", Level.GetFromNumber(1))},

            {"32", new("32", "暴風雪", Level.GetFromNumber(3))},
            {"33", new("33", "大雨", Level.GetFromNumber(3))},
            {"35", new("35", "暴風", Level.GetFromNumber(3))},
            {"36", new("36", "大雪", Level.GetFromNumber(3))},
            {"37", new("37", "波浪", Level.GetFromNumber(3))},
            {"38", new("38", "高潮", Level.GetFromNumber(3))}
        };

        public static Warning GetFromCode(string code) => warnings[code];
    }
}
