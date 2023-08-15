using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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

        public static async ValueTask<TSource?> MaxByAsync<TSource, TKey>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            CancellationToken cancellationToken = default
        ) where TKey : IComparable<TKey>
        {
            return await source.AggregateAsync((a, b) => keySelector(a).CompareTo(keySelector(b)) >= 0 ? a : b, cancellationToken);
        }

        public static string ToHex(this Color color) => (color.ToArgb() & 0xffffff).ToString("x6");
    }
}
