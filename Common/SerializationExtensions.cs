// <copyright file="SerializationExtensions.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Serialization extention methods
    /// </summary>
    public static class SerializationExtensions
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
        
        /// <summary>
        /// Deserializes the contents of an XML file into an instance of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="path">The file path</param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Deserialize<T>(this string path)
        {
            Serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t));
            using (var reader = new StringReader(GetFileContents(path, TimeSpan.FromMilliseconds(2000))))
            {
                return (T)Serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t)).Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes <paramref name="instance"/> to XML and saves it to <paramref name="path"/>
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="instance">The instance to serialize</param>
        /// <param name="path">The file path</param>
        public static void Serialize<T>(this T instance, string path)
        {
            using (var writer = new StringWriter())
            {
                using (var xwriter = XmlWriter.Create(writer, WriterSettings))
                {
                    Serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t)).Serialize(xwriter, instance);
                    PutFileContents(path, writer.ToString(), Encoding.UTF8, TimeSpan.FromMilliseconds(1000));
                }
            }
        }

        private static string GetFileContents(string path, TimeSpan maxWaitTime)
        {
            var contents = string.Empty;
            var endWait = DateTime.Now.Add(maxWaitTime);
            while (string.IsNullOrEmpty(contents))
            {
                try
                {
                    contents = File.ReadAllText(path);
                }
                catch
                {
                    if (DateTime.Now > endWait)
                    {
                        throw;
                    }

                    Task.Delay(100).Wait();
                }
            }

            return contents;
        }

        private static void PutFileContents(string path, string contents, Encoding encoding, TimeSpan maxWaitTime)
        {
            var endWait = DateTime.Now.Add(maxWaitTime);
            while (true)
            {
                try
                {
                    File.WriteAllText(path, contents, encoding);
                    return;
                }
                catch
                {
                    if (DateTime.Now > endWait)
                    {
                        throw;
                    }

                    Task.Delay(100).Wait();
                }
            }
        }
    }
}
