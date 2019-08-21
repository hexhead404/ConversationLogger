
namespace ConversationLogger.Common
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    public class Conversation
    {
        /// <summary>
        /// Gets or sets the conversation identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> that the conversion started
        /// </summary>
        [XmlElement]
        public DateTime Started { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public Message[] Messages { get; set; }
    }
}