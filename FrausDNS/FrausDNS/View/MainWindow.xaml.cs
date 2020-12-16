using FrausDNS.ViewModel;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;


namespace FrausDNS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainWindowViewModel;


        /// <summary>
        /// Creates an instance of the Main Window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            mainWindowViewModel = new MainWindowViewModel();

            // Not part of the View Model. Simply something on the UI. MVVM still maintained.
            ((INotifyCollectionChanged)CapturedRequests.Items).CollectionChanged += (s, e) =>
            {
                if (CapturedRequests.Items.Count > 1)
                    CapturedRequests.ScrollIntoView(CapturedRequests.Items[CapturedRequests.Items.Count - 1]);
            };


            DataContext = mainWindowViewModel;
        }

        /// <summary>
        /// Changes the tab when a row is double clicked in the DNS Capture Window.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Double click mouse button arugments.</param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Tabs.SelectedIndex = 1;
            e.Handled = true;
        }

        /// <summary>
        /// Tells if the window was closed. 
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Overrided function that handles the OnClosed event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;

            if (!mainWindowViewModel.serverStopped)
                mainWindowViewModel.StopServer.Execute(null);
        }

    }
}
