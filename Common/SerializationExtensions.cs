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
            using (var reader = new StringReader(File.ReadAllText(path)))
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
                    File.WriteAllText(path, writer.ToString(), Encoding.UTF8);
                }
            }
        }
    }
}
