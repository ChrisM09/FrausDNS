using Local_Dns_Spoofer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Local_Dns_Spoofer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainWindowViewModel;


        public MainWindow()
        {
            InitializeComponent();

            mainWindowViewModel = new MainWindowViewModel();

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
    }
}
