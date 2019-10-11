// <copyright file="Conversation.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Common
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// A class that represents an instant message conversation
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Gets or sets the conversation identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that the conversion started
        /// </summary>
        [XmlElement]
        public DateTime Started { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the messages
        /// </summary>
        public Message[] Messages { get; set; }
    }
}