
namespace ConversationLogger.Common
{
    using System;
    using System.Xml.Serialization;

    public class Message
    {
        [XmlAttribute] 
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        [XmlAttribute] 
        public MessageDirection Direction { get; set; } = MessageDirection.Incoming;

        [XmlAttribute] 
        public string Contact { get; set; }

        [XmlAttribute] 
        public string ContactEmail { get; set; }

        public string Text { get; set; }
    }
}
