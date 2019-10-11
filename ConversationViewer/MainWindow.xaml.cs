// <copyright file="MainWindow.xaml.cs" company="Hexhead404">
// Copyright (c) Hexhead404. All rights reserved.
// </copyright>

namespace ConversationLogger.Viewer
{
    using System.Windows;
    using ConversationLogger.Viewer.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

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
