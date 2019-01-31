using Microsoft.Win32;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using MedianFilter;

namespace MedianFilterProject
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {   
            InitializeComponent();
            DataContext = new MedianFilterViewModel();
        } 

        private void open_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
