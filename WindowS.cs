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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for WSolitaire.xaml
    /// </summary>
    public partial class WindowS : Window
    {
        public string CurrentLanguage { get; }
        public WindowS()
        {
            InitializeComponent();
            _ = ChangeMinMaxButtonContent();
            Settings.WriteSettingsFile();
            Statistics.CreateFile();
            Localisation.LocalisationIntegrityCheck();
            CurrentLanguage = Localisation.GetCurrentLanguage();
        }

        private async Task ChangeMinMaxButtonContent()
        {
            MinMaxButton.Content = (WindowState == WindowState.Normal) ? "🗖" : "🗗";
            await Task.Delay(250);
            _ = ChangeMinMaxButtonContent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.Content = Localisation.SetText(TextType.WindowSettingsButton, CurrentLanguage) + " ⚙";
            Menu menu = new(CurrentLanguage);
            SolitaireFrame.NavigationService.Navigate(menu);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount > 1) MinMaxClick(new Button(),new RoutedEventArgs());
            DragMove();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
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
            Settings settings = new(EnableSettingsButton, CurrentLanguage);
            settings.Owner = this;
            settings.Show();
            SettingsButton.IsEnabled = false;
        }

        private void EnableSettingsButton()
        {
            SettingsButton.IsEnabled = true;
        }
    }
}
