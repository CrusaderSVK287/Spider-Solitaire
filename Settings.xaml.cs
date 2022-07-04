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
using System.Windows.Shapes;
using System.IO;

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private Action _ReenableSettingsButton;

        public Settings(Action ReenableSettingsButton)
        {
            InitializeComponent();
            _ReenableSettingsButton = ReenableSettingsButton;
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(@"settings.txt")) WriteSettingsFile();
            try
            {
                string[] lines = File.ReadAllLines(@"settings.txt");
                for (int i = 0; i < 4; i++)
                {
                    string[] data = lines[i].Split(' ');
                    if (data.Length != 2) throw new FileFormatException();
                    switch (i)
                    {
                        case 0:
                            CardSizeText.Text = data[1];
                            break;
                        case 1:
                            CardSpacingText.Text = data[1];
                            break;
                        case 2:
                            HintModeBox.SelectedIndex = Convert.ToInt32(data[1]);
                            break;
                        case 3:
                            PlayAnimationsBox.IsChecked = (data[1] == "1")?true:false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelButtonClick(new Button(), new RoutedEventArgs());
                return;
            }
        }

        public static bool WriteSettingsFile()
        {
            if (File.Exists(@"settings.txt")) return false; ;
            try
            {
                string[] data =
                {
                    "Card_size= 100",
                    "Card_spacing= 20",
                    "Hint_mode= 0",
                    "Play_animations= 1"
                };
                File.WriteAllLines(@"settings.txt", data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(),"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool WriteSettingsFile(int cardSize, int cardSpacing, int hintMode, bool playAnimaton)
        {
            if (!File.Exists(@"settings.txt")) return false;
            try
            {
                string[] data =
                {
                    $"Card_size= {cardSize}",
                    $"Card_spacing= {cardSpacing}",
                    $"Hint_mode= {hintMode}",
                    $"Play_animations= {(playAnimaton?1.ToString():0.ToString())}"
                };
                File.WriteAllLines(@"settings.txt", data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelButtonClick(new Button(), new RoutedEventArgs());
                return false;
            }
            return true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _ReenableSettingsButton();
        }

        private void CardSizeUpClick(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(CardSizeText.Text);
            if (size >= 200) return;
            size++;
            CardSizeText.Text = size.ToString();
        }

        private void CardSizeDownClick(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(CardSizeText.Text);
            if (size <= 50) return;
            size--;
            CardSizeText.Text = size.ToString();
        }

        private void CardSpacingUpClick(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(CardSpacingText.Text);
            if (size >= 60) return;
            size++;
            CardSpacingText.Text = size.ToString();
        }
        private void CardSpacingDownClick(object sender, RoutedEventArgs e)
        {
            int size = Convert.ToInt32(CardSpacingText.Text);
            if (size <= 10) return;
            size--;
            CardSpacingText.Text = size.ToString();
        }

        private void CardSizeTextMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0) CardSizeUpClick(new Button(), new RoutedEventArgs());
            else if(e.Delta < 0) CardSizeDownClick(new Button(), new RoutedEventArgs());
        }

        private void CardSpacingTextMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0) CardSpacingUpClick(new Button(), new RoutedEventArgs());
            else if (e.Delta < 0) CardSpacingDownClick(new Button(), new RoutedEventArgs());
        }

        private void ResetStatisticsButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will reset all statistics to 0.\nThere is no going back", "Are you sure?",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
        }

        private void DefaultSettButtonClick(object sender, RoutedEventArgs e)
        {
            CardSizeText.Text = "100";
            CardSpacingText.Text = "20";
            PlayAnimationsBox.IsChecked = true;
            HintModeBox.SelectedIndex = 0;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            if (WriteSettingsFile(Convert.ToInt32(CardSizeText.Text),
                                 Convert.ToInt32(CardSpacingText.Text),
                                 HintModeBox.SelectedIndex,
                                 PlayAnimationsBox.IsChecked == true)
                == false) return;
            MessageBox.Show("Settings have been applied.\nYou may now close the window.\nIf you are in game, please reload it in order\nfor changes to take effect","Applied",MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
