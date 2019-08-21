
namespace ConversationLogger.Common
{
    using System;
    using System.IO;

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
    }
}
