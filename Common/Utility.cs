// <copyright file="Utility.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Utility extension methods.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Asserts that the specified value is not null.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of value to check.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentException">When the value is null.</exception>
        public static T AssertParamterNotNull<T>(this T value, string name)
        {
            return value != null ? value 
                : throw new ArgumentException($"Parameter '{name}' cannot be null", name);
        }

        /// <summary>
        /// Asserts that the specified value is not null or empty.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentException">When the value is null or empty.</exception>
        public static string AssertParamterNotNullOrEmpty(this string value, string name)
        {
            return !string.IsNullOrEmpty(value) ? value 
                : throw new ArgumentException($"Parameter '{name}' cannot be null or empty", name);
        }

        /// <summary>
        /// Asserts that the specified file path exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentException">When the path does not exist.</exception>
        public static string AssertParamterFileExists(this string path, string name)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path) ? path 
                : throw new ArgumentException($"File '{path}' doesn't exist", name);
        }

        /// <summary>
        /// Tests whether strings are equal, ignoring case.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of value to check.</typeparam>
        /// <param name="instance">The string instance.</param>
        /// <param name="value">The value to compare the instance with.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentException">When <paramref name="instance"/> is null.</exception>
        public static bool EqualsIgnoreCase(this string instance, string value)
        {
            return instance.AssertParamterNotNull(nameof(instance)).Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a collection of strings contains the specified value, ignoring case.
        /// </summary>
        /// <param name="collection">The collection of strings.</param>
        /// <param name="value">The value.</param>
        /// <returns>Whether <paramref name="collection"/> contains <paramref name="value"/>, ignoring case.</returns>
        /// <exception cref="ArgumentException">When <paramref name="collection"/> is null</exception>
        public static bool ContainsIgnoreCase(this IEnumerable<string> collection, string value)
        {
            return collection.AssertParamterNotNull(nameof(collection)).Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether an instance of a string contains the specified substring value, ignoring case.
        /// </summary>
        /// <param name="instance">The instance of a string.</param>
        /// <param name="substring">The substring to find.</param>
        /// <returns>Whether <paramref name="instance"/> contains <paramref name="substring"/>, ignoring case.</returns>
        /// <exception cref="ArgumentException">When <paramref name="instance"/> is null</exception>
        public static bool ContainsStringIgnoreCase(this string instance, string substring)
        {
            return instance.AssertParamterNotNull(nameof(instance)).IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
