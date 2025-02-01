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
using System.Diagnostics;

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private readonly Action _ReenableSettingsButton;
        private string CurrentLanguage { get; }
        private string SelectedLanguage { get; set; }
        private List<string> Languages { get; set; }

        public Settings(Action ReenableSettingsButton, string language)
        {
            InitializeComponent();
            CurrentLanguage = language;
            SelectedLanguage = language;
            _ReenableSettingsButton = ReenableSettingsButton;
            Languages = new List<string>();
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(@"settings.txt")) WriteSettingsFile();
            try
            {
                Languages = GetLanguages();
                LoadLanguages();
                string[] lines = File.ReadAllLines(@"settings.txt");
                for (int i = 0; i < 5; i++)
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
                            PlayAnimationsBox.IsChecked = data[1] == "1";
                            break;
                        case 4:
                            LanguageBox.SelectedIndex = SetLanguageIndex(data[1]);
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

            VisualsText.Text = Localisation.SetText(TextType.SettingsVisuals, CurrentLanguage);
            CardSizeDesc.Text = Localisation.SetText(TextType.SettingsCardSize, CurrentLanguage);
            CardSpacingDesc.Text = Localisation.SetText(TextType.SettingsCardSpacing, CurrentLanguage);
            PlayAnimDesc.Text = Localisation.SetText(TextType.SettingsPlayAnimations, CurrentLanguage);
            NoteAboutScrolling.Text = Localisation.SetText(TextType.SettingsNoteAboutScrolling, CurrentLanguage);

            GameplayText.Text = Localisation.SetText(TextType.SettingsGameplay, CurrentLanguage);
            HintModeDesc.Text = Localisation.SetText(TextType.SettingsHintMode, CurrentLanguage);
            LanguageDesc.Text = Localisation.SetText(TextType.SettingsLanguage, CurrentLanguage);

            MiscellaneousText.Text = Localisation.SetText(TextType.SettingsMiscellaneous, CurrentLanguage);
            ResetStatisticsButton.Content = Localisation.SetText(TextType.SettingsResetStats, CurrentLanguage);
            DefaultSettButton.Content = Localisation.SetText(TextType.SettingsDefaultSettings, CurrentLanguage);

            ApplyButton.Content = Localisation.SetText(TextType.SettingsButtonApply, CurrentLanguage);
            CancelButton.Content = Localisation.SetText(TextType.SettingsButtonCancel, CurrentLanguage);

            HintEnabled.Content = Localisation.SetText(TextType.SettingsHintModeItemEnabled, CurrentLanguage);
            HintRestricted.Content = Localisation.SetText(TextType.SettingsHintModeItemRestricted, CurrentLanguage);
            HintDisabled.Content = Localisation.SetText(TextType.SettingsHintModeItemDisabled, CurrentLanguage);
            Title = Localisation.SetText(TextType.WindowSettingsButton, CurrentLanguage);

            RestartOnLanguageChangeDesc.Text = Localisation.SetText(TextType.SettingsRestartOnLangChange, CurrentLanguage);
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
                    "Play_animations= 1",
                    "Language= English"
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

        private bool WriteSettingsFile(int cardSize, int cardSpacing, int hintMode, bool playAnimaton, string language)
        {
            if (!File.Exists(@"settings.txt")) return false;
            try
            {
                string[] data =
                {
                    $"Card_size= {cardSize}",
                    $"Card_spacing= {cardSpacing}",
                    $"Hint_mode= {hintMode}",
                    $"Play_animations= {(playAnimaton?1.ToString():0.ToString())}",
                    $"Language= {language}"
                };
                File.WriteAllLines(@"settings.txt", data);
                SelectedLanguage = language;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CancelButtonClick(new Button(), new RoutedEventArgs());
                return false;
            }
            return true;
        }

        private static List<string> GetLanguages()
        {
            List<string> list = new();

            string[] files = Directory.GetFiles(@"localisation");
            foreach (string file in files)
            {
                if (file.Contains("DEBUG")) continue;
                string[] tmp = file.Split(new char[] { '\\', '.' });
                list.Add(tmp[1]);
            }
            return list;
        }

        private static string ParseLanguage(string langugae)
        {
            return langugae switch
            {
                "Slovencina" => "Slovenčina",
                "Ukrainsky" => "Українська",
                _ => langugae,
            };
        }

        private void LoadLanguages()
        {
            if (Languages == null) return;
            foreach (var item in Languages)
            {
                ComboBoxItem comboBoxItem = new()
                {
                    Content = ParseLanguage(item),
                };
                LanguageBox.Items.Add(comboBoxItem);
            }
        }

        private int SetLanguageIndex(string language)
        {
            if (language == null || Languages == null) return -1;
            return Languages.IndexOf(language);
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
            if (MessageBox.Show(Localisation.SetText(TextType.SettingsResetStatsQuestion, CurrentLanguage), "Note",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;
            Statistics.ResetStatistics();
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
                                 PlayAnimationsBox.IsChecked == true,
                                 Languages[LanguageBox.SelectedIndex])
                == false) return;
            if( SelectedLanguage != CurrentLanguage && RestartOnLanguageChange.IsChecked == true)
            {
                Process.Start(@"Spider Solitaire.exe");
                Environment.Exit(0);
            }
            MessageBox.Show(Localisation.SetText(TextType.SettingsApplyMessage, CurrentLanguage),"Applied",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void LanguageBoxSelected(object sender, RoutedEventArgs e)
        {
            if(LanguageBox.SelectedIndex != Languages.IndexOf(CurrentLanguage))
            {
                RestartOnLanguageChange.Visibility = Visibility.Visible;
                RestartOnLanguageChangeDesc.Visibility = Visibility.Visible;
            }
            else
            {
                RestartOnLanguageChange.Visibility = Visibility.Hidden;
                RestartOnLanguageChangeDesc.Visibility = Visibility.Hidden;
            }
        }
    }
}
