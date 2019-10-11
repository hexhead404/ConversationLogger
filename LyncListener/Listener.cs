// <copyright file="Listener.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.LyncListener
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Lync.Model;
    using Microsoft.Lync.Model.Conversation;

    /// <summary>
    /// A class that listens for Lync conversations.
    /// </summary>
    public class Listener : IDisposable
    {
        private readonly ConcurrentDictionary<Conversation, Logger> logs = new ConcurrentDictionary<Conversation, Logger>();

        private LyncClient client;

        /// <summary>
        /// Starts listening for conversations.
        /// </summary>
        public void StartListening()
        {
            this.Connect();
        }

        /// <summary>
        /// Stops listening for conversations.
        /// </summary>
        public void StopListening()
        {
            this.Disconnect();
        }

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
            if (disposing)
            {
                this.Disconnect();
            }
        }

        private void Connect()
        {
            Console.WriteLine("Connecting to client...");
            while (this.client == null)
            {
                try
                {
                    this.client = LyncClient.GetClient();
                    this.client.ClientDisconnected += this.ClientOnClientDisconnected;
                    this.client.ConversationManager.ConversationAdded += this.ConversationAdded;
                    this.client.ConversationManager.ConversationRemoved += this.ConversationRemoved;
                    this.client.ConversationManager.Conversations?.ToList().ForEach(this.AddConversation);
                    Console.WriteLine("Ready");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error:\r\n{ex.Message}");
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                }
            }
        }

        private void ClientOnClientDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Client disconnected");
            this.Disconnect();
            this.Connect();
        }

        private void Disconnect()
        {
            if (this.client != null)
            {
                this.client.ClientDisconnected -= this.ClientOnClientDisconnected;
                this.client.ConversationManager.ConversationAdded -= this.ConversationAdded;
                this.client.ConversationManager.ConversationRemoved -= this.ConversationRemoved;
                this.logs.Keys.ToList().ForEach(this.RemoveConversation);
            }

            this.client = null;
        }

        private void AddConversation(Conversation conversation)
        {
            if (this.logs.TryAdd(conversation, null))
            {
                this.logs[conversation] = new Logger(conversation);
            }
        }

        private void RemoveConversation(Conversation conversation)
        {
            if (this.logs.TryRemove(conversation, out var log))
            {
                log.Dispose();
            }
        }

        private void ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            this.AddConversation(e.Conversation);
        }

        private void ConversationRemoved(object sender, ConversationManagerEventArgs e)
        {
            this.RemoveConversation(e.Conversation);
        }
    }
}
