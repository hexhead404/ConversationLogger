
namespace ConversationLogger.Viewer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Common;

    /// <summary>
    /// A view model class for conversation logs
    /// </summary>
    public class ConversationLogsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ConversationLogViewModel> logs = new ObservableCollection<ConversationLogViewModel>();
        private readonly FileSystemWatcher watcher;

        private ConversationLogViewModel currentLogFile;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationLogsViewModel"/> class
        /// </summary>
        public ConversationLogsViewModel()
        {
            this.watcher = new FileSystemWatcher(Constants.LogFolder, "*.xml");
            this.watcher.IncludeSubdirectories = false;
            this.watcher.Renamed += WatcherOnRenamed;
            this.watcher.Changed += this.WatcherOnOtherChange;
            this.watcher.Created += this.WatcherOnOtherChange;
            this.watcher.Deleted += this.WatcherOnOtherChange;

            var files = Directory.GetFiles(Constants.LogFolder, "*.xml", SearchOption.TopDirectoryOnly).ToArray();
            this.watcher.EnableRaisingEvents = true;
            this.AddOrUpdateLog(files);

            this.Logs = new ReadOnlyObservableCollection<ConversationLogViewModel>(this.logs);
            this.CurrentLog = this.Logs.FirstOrDefault();
        }

        /// <summary>
        /// Gets the available log files
        /// </summary>
        public ReadOnlyObservableCollection<ConversationLogViewModel> Logs { get; }

        /// <summary>
        /// Gets or sets the current log file
        /// </summary>
        public ConversationLogViewModel CurrentLog
        {
            get => this.currentLogFile;
            set
            {
                if (value != this.currentLogFile)
                {
                    this.CurrentConversation?.Dispose();

                    this.currentLogFile = value;
                    this.CurrentConversation = new ConversationLogViewModel(value.Path);
                    this.NotifyPropertyChanged(nameof(this.CurrentConversation));
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current conversation
        /// </summary>
        public ConversationLogViewModel CurrentConversation { get; private set; }

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
                this.CurrentConversation?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void WatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            this.RemoveLog(e.OldFullPath);
            this.AddOrUpdateLog(e.FullPath);
        }

        private void WatcherOnOtherChange(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    this.RemoveLog(e.FullPath);
                    break;
                default:
                    this.AddOrUpdateLog(e.FullPath);
                    break;
            }
        }

        private void RemoveLog(params string[] paths)
        {
            lock (this.logs)
            {
                foreach (var path in paths ?? Enumerable.Empty<string>())
                {
                    var log = this.logs.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                    if (log != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() => this.logs.Remove(log));
                        log.Dispose();
                    }
                }
            }
        }

        private void AddOrUpdateLog(params string[] paths)
        {
            lock (this.logs)
            {
                foreach (var path in paths ?? Enumerable.Empty<string>())
                {
                    var log = this.logs.FirstOrDefault(x => x.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                    if (log == null)
                    {
                        int i;
                        for (i = 0; i < this.logs.Count && StringComparer.OrdinalIgnoreCase.Compare(this.logs[i].Path, path) < 0; i++)
                        {
                        }
                        Application.Current?.Dispatcher?.Invoke(() => this.logs.Insert(i, new ConversationLogViewModel(path)));
                    }
                    else
                    {
                        Application.Current?.Dispatcher?.Invoke(() => log.LoadConversation());
                    }
                }
            }
        }
    }
}
