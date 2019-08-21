
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageViewModel"/> class
        /// </summary>
        /// <param name="message"></param>
        public MessageViewModel(Message message)
        {
            message.AssertParamterNotNull(nameof(message));

            this.TimeStamp = message.TimeStamp;
            this.Contact = message.Contact;
            this.ContactEmail = message.ContactEmail;
            this.Direction = message.Direction;
            this.Text = message.Text?.Trim();
        }

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

        /// <inheritdoc />
        public override string ToString() => $"[{this.TimeStamp:g}] {this.Contact}: {this.Text}";
    }
}
