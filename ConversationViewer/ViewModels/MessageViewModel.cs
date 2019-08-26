
namespace ConversationLogger.Viewer.ViewModels
{
    using System;
    using Common;

    /// <summary>
    /// A view model class for conversation messages
    /// </summary>
    public class MessageViewModel : ViewModelBase
    {
        private bool isSelected;
        private bool isFilterMatch;
        private bool isCurrentMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageViewModel"/> class
        /// </summary>
        /// <param name="conversation">The conversation that the message belongs to</param>
        /// <param name="message">The <see cref="Message"/> that this view model is based upon</param>
        public MessageViewModel(ConversationLogViewModel conversation, Message message)
        {
            this.Conversation = conversation.AssertParamterNotNull(nameof(conversation));
            this.Conversation.Messages.CurrentChanged += this.ConversationMessagesOnCurrentChanged;
            this.TimeStamp = message.AssertParamterNotNull(nameof(message)).TimeStamp;
            this.Contact = message.Contact;
            this.ContactEmail = message.ContactEmail;
            this.Direction = message.Direction;
            this.Text = message.Text?.Trim();
        }

        /// <summary>
        /// Gets the <see cref="ConversationLogViewModel"/> this message belongs to
        /// </summary>
        public ConversationLogViewModel Conversation { get; }

        /// <summary>
        /// Gets the message timestamp
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Gets the contact name
        /// </summary>
        public string Contact  { get; }

        /// <summary>
        /// Gets the contact email address
        /// </summary>
        public string ContactEmail  { get; }

        /// <summary>
        /// Gets the message direction
        /// </summary>
        public MessageDirection Direction  { get; }

        /// <summary>
        /// Gets the message text
        /// </summary>
        public string Text  { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the message is selected
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != this.isSelected)
                {
                    isSelected = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message is included in the filtered results
        /// </summary>
        public bool IsFilterMatch
        {
            get => isFilterMatch;
            private set
            {
                if (value != this.isFilterMatch)
                {
                    isFilterMatch = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsCurrentMessage
        {
            get => isCurrentMessage;
            private set
            {
                if (value != this.isCurrentMessage)
                {
                    isCurrentMessage = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool TryMatchFilter(string filter)
        {
            this.IsFilterMatch = !string.IsNullOrEmpty(filter)
                && (this.Contact.ContainsStringIgnoreCase(filter) || this.Text.ContainsStringIgnoreCase(filter));

            return this.IsFilterMatch;
        }

        /// <inheritdoc />
        public override string ToString() => $"[{this.TimeStamp:g}] {this.Contact}: {this.Text}";

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.Conversation?.Messages != null)
            {
                this.Conversation.Messages.CurrentChanged -= this.ConversationMessagesOnCurrentChanged;
            }
            base.Dispose(disposing);
        }

        private void ConversationMessagesOnCurrentChanged(object sender, EventArgs e)
        {
            if (sender == this.Conversation.Messages)
            {
                this.IsCurrentMessage = this.Conversation.Messages.CurrentItem == this;
            }
        }
    }
}
