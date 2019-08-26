
namespace ConversationLogger.Viewer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using Common;

    /// <summary>
    /// A view model class for conversation logs
    /// </summary>
    public class ConversationLogsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ConversationLogViewModel> logs = new ObservableCollection<ConversationLogViewModel>();
        private readonly FileSystemWatcher watcher;

        private bool disposed;
        private ConversationLogViewModel currentLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationLogsViewModel"/> class
        /// </summary>
        public ConversationLogsViewModel()
        {
            this.watcher = new FileSystemWatcher(Constants.LogFolder, "*.xml") { IncludeSubdirectories = false };
            this.watcher.Renamed += WatcherOnRenamed;
            this.watcher.Changed += this.WatcherOnOtherChange;
            this.watcher.Created += this.WatcherOnOtherChange;
            this.watcher.Deleted += this.WatcherOnOtherChange;

            var files = Directory.GetFiles(Constants.LogFolder, "*.xml", SearchOption.TopDirectoryOnly).ToArray();
            this.watcher.EnableRaisingEvents = true;

            this.Logs = CollectionViewSource.GetDefaultView(this.logs);
            this.Search = new SearchViewModel(this);
            this.AddOrUpdateLog(files);
            this.CurrentLog = this.logs.FirstOrDefault();
        }
        
        /// <summary>
        /// Gets the <see cref="SearchViewModel"/> used for searching converstion logs
        /// </summary>
        public SearchViewModel Search { get; }

        /// <summary>
        /// Gets the available log files
        /// </summary>
        public ICollectionView Logs { get; }

        /// <summary>
        /// Gets or sets the current log file
        /// </summary>
        public ConversationLogViewModel CurrentLog
        {
            get => currentLog;
            set
            {
                currentLog = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status message
        /// </summary>
        public string StatusMessage { get; private set; } = "Loading conversation files";

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
                this.watcher.EnableRaisingEvents = false;
                this.watcher.Renamed -= WatcherOnRenamed;
                this.watcher.Changed -= this.WatcherOnOtherChange;
                this.watcher.Created -= this.WatcherOnOtherChange;
                this.watcher.Deleted -= this.WatcherOnOtherChange;
                this.watcher.Dispose();
                this.Search.Dispose();
                this.currentLog?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void WatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() => this.RemoveLog(e.OldFullPath));
            Application.Current?.Dispatcher?.Invoke(() => this.AddOrUpdateLog(e.FullPath));
        }

        private void WatcherOnOtherChange(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    Application.Current?.Dispatcher?.Invoke(() => this.RemoveLog(e.FullPath));
                    break;
                default:
                    Application.Current?.Dispatcher?.Invoke(() => this.AddOrUpdateLog(e.FullPath));
                    break;
            }
        }

        private void RemoveLog(params string[] paths)
        {
            if (paths == null)
            {
                return;
            }

            lock (this.logs)
            {
                foreach (var log in this.logs.Where(x => paths.ContainsIgnoreCase(x.Path)).ToList())
                {
                    this.logs.Remove(log);
                    log.Dispose();
                }
            }
            this.UpdateStatus();
        }

        private void AddOrUpdateLog(params string[] paths)
        {
            if (paths == null)
            {
                return;
            }

            lock (this.logs)
            {
                foreach (var path in paths)
                {
                    var log = this.logs.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                    if (log == null)
                    {
                        log = new ConversationLogViewModel(path);
                        int i;
                        for (i = 0; i < this.logs.Count && this.logs[i].Started > log.Started; i++)
                        {
                        }
                        this.logs.Insert(i, log);
                    }
                    else
                    {
                        log.LoadConversation();
                    }
                }
            }
            if (!string.IsNullOrEmpty(this.Search.Filter))
            {
                this.Search.ApplyLogFilter();
            }
            this.UpdateStatus();
        }
        
        private void UpdateStatus()
        {
            lock (this.logs)
            {
                this.StatusMessage = $"{this.logs.Count} conversations, {this.logs.Sum(x => x.Messages.OfType<MessageViewModel>().Count())} messages";
                if (!string.IsNullOrEmpty(this.Search.Filter))
                {
                    this.StatusMessage += $", {this.logs.Sum(x => x.Messages.OfType<MessageViewModel>().Count(m => m.IsFilterMatch))} filter matches";
                }
                this.NotifyPropertyChanged(nameof(this.StatusMessage));
            }
        }
    }
}
