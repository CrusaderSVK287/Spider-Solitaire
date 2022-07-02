﻿using System;
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
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for WSolitaire.xaml
    /// </summary>
    public partial class WindowS : Window
    {
        public WindowS()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount > 1) MinMaxClick(new Button(),new RoutedEventArgs());
            DragMove();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MinMaxClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                MinMaxButton.Content = "🗗";
            }
            else
            {
                WindowState = WindowState.Normal;
                MinMaxButton.Content = "🗖";
            }
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented", "Warning", MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}
