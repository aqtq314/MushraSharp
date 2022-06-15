using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonKit.Utils
{
    public static class Enumerable
    {
        public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
        {
            using IEnumerator<TSource> e = source.GetEnumerator();

            if (e.MoveNext())
            {
                List<TSource> chunkBuilder = new();
                while (true)
                {
                    do
                    {
                        chunkBuilder.Add(e.Current);
                    }
                    while (chunkBuilder.Count < size && e.MoveNext());

                    yield return chunkBuilder.ToArray();

                    if (chunkBuilder.Count < size || !e.MoveNext())
                    {
                        yield break;
                    }
                    chunkBuilder.Clear();
                }
            }
        }
    }
}
