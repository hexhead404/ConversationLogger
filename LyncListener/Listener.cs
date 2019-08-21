
namespace ConversationLogger.LyncListener
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Lync.Model;
    using Microsoft.Lync.Model.Conversation;

    public class Listener : IDisposable
    {
        private readonly ConcurrentDictionary<Conversation, Logger> logs = new ConcurrentDictionary<Conversation, Logger>();

        private LyncClient client;

        public void StartListening()
        {
            this.Connect();
        }

        public void StopListening()
        {
            this.Disconnect();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
                    client = LyncClient.GetClient();
                    client.ClientDisconnected += ClientOnClientDisconnected;
                    client.ConversationManager.ConversationAdded += this.ConversationAdded;
                    client.ConversationManager.ConversationRemoved += this.ConversationRemoved;
                    client.ConversationManager.Conversations?.ToList().ForEach(this.AddConversation);
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
            this.Disconnect();;
            this.Connect();
        }

        private void Disconnect()
        {
            if (client != null)
            {
                client.ClientDisconnected -= ClientOnClientDisconnected;
                client.ConversationManager.ConversationAdded -= this.ConversationAdded;
                client.ConversationManager.ConversationRemoved -= this.ConversationRemoved;
                this.logs.Keys.ToList().ForEach(this.RemoveConversation);
            }

            client = null;
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
