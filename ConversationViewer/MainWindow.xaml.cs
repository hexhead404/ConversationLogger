namespace ConversationLogger.Viewer
{
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewmodel = new ConversationLogsViewModel();
            this.DataContext = viewmodel;
            this.Unloaded += (s, a) => viewmodel.Dispose();
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
