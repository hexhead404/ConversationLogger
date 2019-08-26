
namespace ConversationLogger.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class Utility
    {
        public static T AssertParamterNotNull<T>(this T value, string name)
        {
            return value != null ? value 
                : throw new ArgumentException($"Parameter '{name}' cannot be null", name);
        }

        public static string AssertParamterNotNullOrEmpty(this string value, string name)
        {
            return !string.IsNullOrEmpty(value) ? value 
                : throw new ArgumentException($"Parameter '{name}' cannot be null or empty", name);
        }

        public static string AssertParamterFileExists(this string value, string name)
        {
            return !string.IsNullOrEmpty(value) && File.Exists(value) ? value 
                : throw new ArgumentException($"File '{value}' doesn't exist", name);
        }

        public static bool EqualsIgnoreCase(this string instance, string value)
        {
            return instance.AssertParamterNotNull(nameof(instance)).Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static int IndexIgnoreCase(this string instance, string value)
        {
            return instance.AssertParamterNotNull(nameof(instance)).IndexOf(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> instance, string value)
        {
            return instance.AssertParamterNotNull(nameof(instance)).Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        public static bool ContainsStringIgnoreCase(this string instance, string value)
        {
            return instance.AssertParamterNotNull(nameof(instance)).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
