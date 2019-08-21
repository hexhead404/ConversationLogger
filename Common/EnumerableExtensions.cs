
namespace ConversationLogger.Common
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
        {
            return (enumerable ?? Enumerable.Empty<T>()).Concat(new[] { item });
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T item)
        {
            return new[] { item }.Concat(enumerable ?? Enumerable.Empty<T>());
        }
    }
}
