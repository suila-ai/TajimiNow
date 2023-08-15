using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Jma.Warning
{
    internal class Level : IComparable<Level>
    {
        public int Number { get; }
        public string Name { get; }
        public Color BackgroundColor { get; }
        public Color ForegroundColor { get; }

        private Level(int number, string name, Color backgroundColor, Color foregroundColor)
        {
            Number = number;
            Name = name;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
        }

        public int CompareTo(Level? other)
        {
            if (other == null) return 1;
            return Number.CompareTo(other.Number);
        }

        private static readonly Dictionary<int, Level> levels = new()
        {
            { 1, new(1, "注意報", Color.FromArgb(0xf2, 0xe7, 0x00), Color.Black) },
            { 2, new(2, "警報", Color.FromArgb(0xff, 0x28, 0x00), Color.White) },
            { 3, new(3, "特別警報", Color.FromArgb(0xaa, 0x00, 0xaa), Color.White) }
        };

        public static Level GetFromNumber(int number) => levels[number];
    }
}
