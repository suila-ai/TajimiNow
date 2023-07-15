using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow
{
    internal static class Utils
    {
        public static IEnumerable<int> Indexes<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Select((e, i) => (i, e)).Where(e => predicate(e.e)).Select(e => e.i);
        }
    }
}
