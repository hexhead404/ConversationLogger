
namespace ConversationLogger.Viewer.ViewModels
{
    using System.Linq;
    using Common;

    public class SearchViewModel : ViewModelBase
    {
        private readonly ConversationLogsViewModel owner;
        private string filter;

        private enum SearchDirection
        {
            Forward = 0,
            Backward = 1
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class
        /// </summary>
        /// <param name="logs">The <see cref="ConversationLogsViewModel"/> that will be searched</param>
        public SearchViewModel(ConversationLogsViewModel logs)
        {
            this.owner = logs.AssertParamterNotNull(nameof(logs));
            this.owner.Logs.Filter = this.LogFilter;
            this.NextMatchCommand = new Command(this, o => !string.IsNullOrEmpty(this.Filter), o => this.SetFilteredMessage(SearchDirection.Forward));
            this.PrevMatchCommand = new Command(this, o => !string.IsNullOrEmpty(this.Filter), o => this.SetFilteredMessage(SearchDirection.Backward));
            this.ClearFilterCommand = new Command(this, o => !string.IsNullOrEmpty(this.Filter), o => this.Filter = string.Empty);
        }

        /// <summary>
        /// Gets or sets the search filter
        /// </summary>
        public string Filter
        {
            get => this.filter;
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    this.NotifyPropertyChanged();
                    this.ApplyLogFilter();
                }
            }
        }

        /// <summary>
        /// Gets a command to find the next filter match
        /// </summary>
        public Command NextMatchCommand { get; }
        
        /// <summary>
        /// Gets a command to find the previous filter match
        /// </summary>
        public Command PrevMatchCommand { get; }

        /// <summary>
        /// Gets a command to clear the search filter
        /// </summary>
        public Command ClearFilterCommand { get; }

        /// <summary>
        /// Applies the filter to the logs
        /// </summary>
        internal void ApplyLogFilter()
        {
            this.owner.Logs.Refresh();
            this.SetFilteredMessage(SearchDirection.Forward);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.NextMatchCommand.Dispose();
                this.PrevMatchCommand.Dispose();
                this.ClearFilterCommand.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LogFilter(object obj)
        {
            var vm = (ConversationLogViewModel)obj;
            var messages = vm.Messages.OfType<MessageViewModel>().ToList();

            messages.ForEach(m => m.TryMatchFilter(this.filter));

            return string.IsNullOrEmpty(this.filter) || messages.Any(x => x.IsFilterMatch);
        }

        private void SetFilteredMessage(SearchDirection direction)
        {
            var filtered = direction == SearchDirection.Forward
                ? this.owner.Logs.OfType<ConversationLogViewModel>()
                : this.owner.Logs.OfType<ConversationLogViewModel>().Reverse();
            var lookup = filtered.ToDictionary(
                f => f, 
                f => direction == SearchDirection.Forward
                    ? f.Messages.OfType<MessageViewModel>().Where(m => m.IsFilterMatch).ToList()
                    : f.Messages.OfType<MessageViewModel>().Where(m => m.IsFilterMatch).Reverse().ToList());
            try
            {
                if (this.owner.CurrentLog != null && lookup.TryGetValue(this.owner.CurrentLog, out var matches))
                {
                    // In the current log, search from the current item
                    var match = matches.Contains(this.owner.CurrentLog.Messages.CurrentItem) 
                        ? matches.SkipWhile(m => !m.Equals(this.owner.CurrentLog.Messages.CurrentItem)).Skip(1).FirstOrDefault() 
                        : matches.FirstOrDefault();
                    if (match != null)
                    {
                        this.owner.CurrentLog.Messages.MoveCurrentTo(match);
                        return;
                    }
                }

                // Get the first match in the other files
                foreach (var pair in lookup.Where(x => !x.Key.Equals(this.owner.CurrentLog)))
                {
                    // Just get either the first or last match in the file
                    var match = pair.Value.FirstOrDefault();
                    if (match != null)
                    {
                        this.owner.CurrentLog = pair.Key;
                        this.owner.CurrentLog.Messages.MoveCurrentTo(match);
                        return;
                    }
                }
            }
            finally
            {
                // CurrentItem only applies to the current log
                lookup.Keys.Where(x => !x.Equals(this.owner.CurrentLog)).ToList().ForEach(x => x.Messages.MoveCurrentTo(null));
            }
        }
    }
}
