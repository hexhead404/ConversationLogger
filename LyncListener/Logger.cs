// <copyright file="Logger.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.LyncListener
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ConversationLogger.Common;
    using Microsoft.Lync.Model;
    using Microsoft.Lync.Model.Conversation;
    using Microsoft.Lync.Model.Conversation.Sharing;

    using Conversation = Microsoft.Lync.Model.Conversation.Conversation;

    /// <summary>
    /// A class that logs a Lync conversation.
    /// </summary>
    public class Logger : IDisposable
    {
        private readonly ConcurrentDictionary<Participant, List<Modality>> modalities = new ConcurrentDictionary<Participant, List<Modality>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="conversation">The conversation to log.</param>
        public Logger(Conversation conversation)
        {
            this.Conversation = conversation.AssertParamterNotNull(nameof(conversation));
            this.Conversation.ParticipantAdded += this.ParticipantAdded;
            this.Conversation.ParticipantRemoved += this.ParticipantRemoved;
            this.Conversation.Participants?.ToList().ForEach(this.AddParticipant);
            this.ConversationId = this.GenerateConversationId($"{this.Conversation.Properties[ConversationProperty.Id]}");
            this.ConversationLog = File.Exists(this.LogPath)
                ? this.LogPath.Deserialize<Common.Conversation>()
                : new Common.Conversation { Id = this.ConversationId };
        }

        /// <summary>
        /// Gets the <see cref="Conversation"/>.
        /// </summary>
        public Conversation Conversation { get; }

        /// <summary>
        /// Gets the conversation identifier.
        /// </summary>
        public string ConversationId { get; }

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        public string LogPath => Path.Combine(Constants.LogFolder, $"{this.ConversationId}.xml");

        private Common.Conversation ConversationLog { get; }

        private Contact Self => this.Conversation?.SelfParticipant?.Contact;

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Provides the opportunity for sub-classes to dispose of resources.
        /// </summary>
        /// <param name="disposing">Whether this method was called from the <see cref="Dispose"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.Conversation != null)
            {
                this.Conversation.ParticipantAdded -= this.ParticipantAdded;
                this.Conversation.ParticipantRemoved -= this.ParticipantRemoved;
                this.modalities.Keys.ToList().ForEach(this.RemoveParticipant);
            }
        }

        private string GenerateConversationId(string value)
        {
            // Replace spaces, and illegal chars (0-31 plus some others) with hex equivalents
            var replace = Enumerable.Range(0, 33).Select(i => (char)i).Concat("<>:\"/\\|?*".ToArray());
            var clean = value.Select(c => replace.Contains(c) ? Convert.ToByte(c).ToString("x") : $"{c}");

            return string.Join(string.Empty, clean);
        }

        private void ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            this.AddParticipant(e.Participant);
        }

        private void AddParticipant(Participant participant)
        {
            var mods = new List<Modality>();
            if (participant?.Modalities != null && this.modalities.TryAdd(participant, mods))
            {
                foreach (var mod in participant.Modalities.Values.OfType<InstantMessageModality>())
                {
                    mods.Add(mod);
                    mod.InstantMessageReceived += this.MessageReceived;
                }

                foreach (var mod in participant.Modalities.Values.OfType<ContentSharingModality>())
                {
                    mods.Add(mod);
                    mod.ContentAdded += this.ContentAdded;
                }
            }
        }

        private void ParticipantRemoved(object sender, ParticipantCollectionChangedEventArgs e)
        {
            this.RemoveParticipant(e.Participant);
        }

        private void RemoveParticipant(Participant participant)
        {
            if (participant != null && this.modalities.TryRemove(participant, out var mods))
            {
                mods.OfType<InstantMessageModality>().ToList().ForEach(m => m.InstantMessageReceived -= this.MessageReceived);
                mods.OfType<ContentSharingModality>().ToList().ForEach(m => m.ContentAdded -= this.ContentAdded);
            }
        }

        private void MessageReceived(object sender, MessageSentEventArgs e)
        {
            if (sender is InstantMessageModality imm)
            {
                var contact = imm?.Participant?.Contact;
                var text = e.Text?.Trim();
                if (contact != null && !string.IsNullOrEmpty(text))
                {
                    var msg = new Message
                    {
                        Direction = contact == this.Self ? MessageDirection.Outgoing : MessageDirection.Incoming,
                        Contact = contact.GetContactInformation(ContactInformationType.DisplayName)?.ToString(),
                        ContactEmail = contact.GetContactInformation(ContactInformationType.PrimaryEmailAddress).ToString(),
                        Text = text,
                    };
                    Console.WriteLine(msg.ToString());
                    this.ConversationLog.Messages = this.ConversationLog.Messages.Append(msg).ToArray();
                    this.ConversationLog.Serialize(this.LogPath);
                }
            }
        }

        private void ContentAdded(object sender, ContentCollectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Item?.Title) && sender is ContentSharingModality csm && csm.Participant?.Contact is Contact contact)
            {
                var msg = new Message
                {
                    Direction = contact == this.Self ? MessageDirection.Outgoing : MessageDirection.Incoming,
                    Contact = contact.GetContactInformation(ContactInformationType.DisplayName)?.ToString(),
                    ContactEmail = contact.GetContactInformation(ContactInformationType.PrimaryEmailAddress).ToString(),
                    AttachmentPath = $"{this.ConversationId}.{e.Item.Title}",
                };
                msg.Text = $"{msg.Contact} sent an attachment {e.Item.Title}";

                try
                {
                    var path = Path.Combine(Constants.LogFolder, msg.AttachmentPath);
                    Console.WriteLine($"Attempting to download file to {path}");
                    e.Item.BeginDownloadFile(path, this.ContentDownloadCallback, path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Download error: {ex.Message}");
                    throw;
                }
            }
        }

        private void ContentDownloadCallback(IAsyncResult ar)
        {
            Console.WriteLine("Download complete");
        }
    }
}
