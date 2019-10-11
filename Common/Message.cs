// <copyright file="Message.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A class that represents an instant message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the date and time the message was sent
        /// </summary>
        [XmlAttribute] 
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the message direction
        /// </summary>
        [XmlAttribute] 
        public MessageDirection Direction { get; set; } = MessageDirection.Incoming;

        /// <summary>
        /// Gets or sets the contact name
        /// </summary>
        [XmlAttribute] 
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets the contact email address
        /// </summary>
        [XmlAttribute] 
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the path to an attachment
        /// </summary>
        [XmlAttribute] 
        public string AttachmentPath { get; set; }

        /// <summary>
        /// Gets or sets the message text
        /// </summary>
        public string Text { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{(this.Direction == MessageDirection.Incoming ? "-->" : "<--")} [{this.TimeStamp:g}] {this.Contact}: {this.Text}";
        }
    }
}
