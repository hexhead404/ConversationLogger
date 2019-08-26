using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversationLogger.Common
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public static class SerializationExtensions
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
        
        public static T Deserialize<T>(this string path)
        {
            Serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t));
            using (var reader = new StringReader(GetFileContents(path, TimeSpan.FromMilliseconds(2000))))
            {
                return (T)Serializers.GetOrAdd(typeof(T), t => new XmlSerializer(t)).Deserialize(reader);
            }
        }

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
