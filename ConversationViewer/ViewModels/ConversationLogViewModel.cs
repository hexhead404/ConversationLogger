
namespace ConversationLogger.Viewer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using Common;

    /// <summary>
    /// A view model for conversation log
    /// </summary>
    public class ConversationLogViewModel : ViewModelBase
    {
        private readonly ObservableCollection<MessageViewModel> messages = new ObservableCollection<MessageViewModel>();

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationLogViewModel"/> class
        /// </summary>
        /// <param name="path">The log file path</param>
        public ConversationLogViewModel(string path)
        {
            this.Path = path.AssertParamterFileExists(nameof(path));
            this.Messages = CollectionViewSource.GetDefaultView(this.messages);
            this.CopyCommand = new Command(this, o => this.AreMessagesSelected, o => this.CopySelectedMessagesToClipboard());
            this.LoadConversation();
        }

        /// <summary>
        /// Gets or sets the conversation identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the log file path
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the conversation titme
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> that the conversation started
        /// </summary>
        public DateTime Started { get; private set; }

        /// <summary>
        /// Gets the messages
        /// </summary>
        public ICollectionView Messages { get; }

        /// <summary>
        /// Gets a value indicating whether message(s) are selected
        /// </summary>
        public bool AreMessagesSelected => this.Messages.OfType<MessageViewModel>().Any(m => m.IsSelected);

        /// <summary>
        /// Gets a command to copy selected messages to the clipboard
        /// </summary>
        public Command CopyCommand { get; private set; }
        
        /// <summary>
        /// Loads the conversation from disk
        /// </summary>
        public void LoadConversation()
        {
            var log = this.Path.Deserialize<Conversation>();
            this.Id = log.Id;
            this.Title = $"Conversation started {log.Started:G}";
            this.Started = log.Started;
            this.NotifyPropertyChanged(nameof(this.Title));
            this.NotifyPropertyChanged(nameof(this.Started));

            this.messages.ToList().ForEach(this.RemoveMessage);
            log.Messages.OrderBy(m => m.TimeStamp).ToList().ForEach(this.AddMessage);
            this.NotifyPropertyChanged(nameof(this.Messages));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                this.messages.ToList().ForEach(this.RemoveMessage);
                this.CopyCommand.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AddMessage(Message message)
        {
            var vm = new MessageViewModel(this, message);
            vm.PropertyChanged += this.MessageOnPropertyChanged;
            this.messages.Add(vm);
        }

        private void RemoveMessage(MessageViewModel message)
        {
            this.messages.Remove(message);
            message.PropertyChanged -= this.MessageOnPropertyChanged;
            message.Dispose();
        }

        private void CopySelectedMessagesToClipboard()
        {
            var lines = this.messages.Where(m => m.IsSelected).Select(m => $"{m}");

            Clipboard.SetText(string.Join("\r\n", lines));
        }

        private void MessageOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MessageViewModel.IsSelected))
            {
                this.NotifyPropertyChanged(nameof(this.AreMessagesSelected));
            }
        }
    }
}
