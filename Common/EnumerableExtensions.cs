// <copyright file="EnumerableExtensions.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension method for IEnumerables
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Appends <paramref name="item"/> to the end of <paramref name="enumerable"/> 
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="enumerable">The enumerable</param>
        /// <param name="item">The item to append</param>
        /// <returns>The resulting enumerable</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
        {
            return (enumerable ?? Enumerable.Empty<T>()).Concat(new[] { item });
        }

        /// <summary>
        /// Prepends <paramref name="item"/> to the beginning of <paramref name="enumerable"/> 
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="enumerable">The enumerable</param>
        /// <param name="item">The item to prepend</param>
        /// <returns>The resulting enumerable</returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T item)
        {
            return new[] { item }.Concat(enumerable ?? Enumerable.Empty<T>());
        }
    }
}
