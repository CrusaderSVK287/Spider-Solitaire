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
using System.IO;
using System.Diagnostics;
using System.Net;

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public string CurrentLanguage { get; }
        public Game? game;
        public Menu(string language)
        {
            InitializeComponent();
            CurrentLanguage = language;
            HowToPlayClick(new Button(),new RoutedEventArgs());
            _ = CheckForUpdate();
        }

        private void DestroyGameReference()
        {
            if (game == null) return;
            game.SolitaireGrid.Children.Clear();
            game.SolitaireGrid = null;
            game = null;
        }

        private void OneSuiteNewGameClick(object sender, RoutedEventArgs e)
        {
            StartGame(1, true);
        }

        private void TwoSuiteNewGameClick(object sender, RoutedEventArgs e)
        {
            StartGame(2, true);
        }

        private void FourSuiteNewGameClick(object sender, RoutedEventArgs e)
        {
            StartGame(4, true);
        }

        private void LoadGameClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"autosave.soli")) StartGame(-1, false);
            else
            {
                Load.IsEnabled = false;
                MessageBox.Show("The save file is missing.","Warning",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }

        private void StartGame(int numberOfSuits, bool isNewGame)
        {
            game = new Game(numberOfSuits, isNewGame, this, DestroyGameReference, CurrentLanguage);
            NavigationService.Navigate(game);
        }

        private void MenuLoaded(object sender, RoutedEventArgs e)
        {
            Load.IsEnabled = File.Exists(@"autosave.soli");
            WelcomeBanner.Text = Localisation.SetText(TextType.MenuWelcomeBanner, CurrentLanguage);
            OneSuit.Content = Localisation.SetText(TextType.MenuOneSuitButton, CurrentLanguage);
            TwoSuit.Content = Localisation.SetText(TextType.MenuTwoSuitButton, CurrentLanguage);
            FourSuit.Content = Localisation.SetText(TextType.MenuFourSuitButton, CurrentLanguage);
            HowToPlay.Content = Localisation.SetText(TextType.MenuHowToPlayButton, CurrentLanguage);
            Stats.Content = Localisation.SetText(TextType.MenuStatisticsButton, CurrentLanguage);
            UpdateButton.Content = Localisation.SetText(TextType.MenuUpdateButton, CurrentLanguage);
        }

        private void HowToPlayClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart01,CurrentLanguage), FontSize = 40 });
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart02, CurrentLanguage), TextWrapping = TextWrapping.Wrap });

            StackPanel b = new()
            {
                Orientation = Orientation.Horizontal,
            };
            SPInformation.Children.Add(b);
            for (int C = 'a'; C <= 'd'; C++)
            {
                Image img = new()
                {
                    Width = 89,
                    Height = 120,
                    Source = new BitmapImage(new Uri(@"assets/13" + Convert.ToChar(C) + ".png", UriKind.Relative)),
                };
                b.Children.Add(img);
            }
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart03, CurrentLanguage), TextWrapping = TextWrapping.Wrap });

            StackPanel d = new()
            {
                Orientation = Orientation.Horizontal,
            };
            SPInformation.Children.Add(d);
            for (int C = 13; C >= 1; C--)
            {
                Image img = new()
                {
                    Width = (C == 1) ? 89 : 25,
                    Height = 120,
                    Stretch = Stretch.None,
                    Source = new BitmapImage(new Uri(@"assets/" + C.ToString() + "c.png", UriKind.Relative)),
                };
                d.Children.Add(img);
            }

            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart04, CurrentLanguage), TextWrapping = TextWrapping.Wrap });

            Image g = new()
            {
                Width = 89,
                Height = 120,
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/card_outline.png", UriKind.Relative)),
            };
            SPInformation.Children.Add(g);
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart05, CurrentLanguage), TextWrapping = TextWrapping.Wrap });

            Border border = new()
            {
                Width = 200,
                Height = 50,
                BorderBrush = Brushes.Gold,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(30),
            };
            SPInformation.Children.Add(border);
            TextBlock button = new()
            {
                Text = "Tutorial",
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Width = 200,
                Height = 50,
                Margin = new Thickness(70, 8, 0, 0),
            };
            button.MouseLeftButtonUp += TutorialGame;
            void TutorialGame(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    File.WriteAllText(@"autosave.soli", "mc ac mc bc jc gc hc mc fc cc jc dc ic ec ic bc dc hc fc kc mc mc lc jc jc bc dc hc ic bc cc hc dc jc ic hc lc ec ac cc ec cc jc kc dc fc dc ec gc gc lc dc kc fc ac jc bc gc gc lc cc ac jc dc lc cc fc ec hc mc lc mc hc cc bc gc mc kc fc ac ec ic fc lc hc fc lc cc ac ic kc kc ec ic ac kc kc ac ic bc bc ec gc gc -null- ".Replace(' ', '\n'));
                    LoadGameClick(new Button(), new RoutedEventArgs());
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            }
            border.Child = button;

            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart06, CurrentLanguage), TextWrapping = TextWrapping.Wrap, FontSize = 40 });
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart07, CurrentLanguage), TextWrapping = TextWrapping.Wrap });

            StackPanel buttonsOne = new()
            {
                Orientation = Orientation.Horizontal,
            };
            SPInformation.Children.Add(buttonsOne);
            Image Bimg = new()
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0, 0, 20, 0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/home_icon_pressed.png", UriKind.Relative)),
            };
            buttonsOne.Children.Add(Bimg);
            buttonsOne.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart08, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Height = 80, Width = 275 });


            StackPanel buttonsTwo = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0),
            };
            SPInformation.Children.Add(buttonsTwo);
            Image BTwoimg = new()
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0, 0, 20, 0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/restart_icon_pressed.png", UriKind.Relative)),
            };
            buttonsTwo.Children.Add(BTwoimg);
            buttonsTwo.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart09, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Height = 50, Width = 275 });


            StackPanel buttonsThree = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0),
            };
            SPInformation.Children.Add(buttonsThree);
            Image BThreeimg = new()
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0, 20, 20, 0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/back_icon_pressed.png", UriKind.Relative)),
            };
            buttonsThree.Children.Add(BThreeimg);
            buttonsThree.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart10, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Height = 30, Width = 275 });


            StackPanel buttonsFour = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0),
            };
            SPInformation.Children.Add(buttonsFour);
            Image BFourimg = new()
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0, 0, 20, 0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/hint_icon_pressed.png", UriKind.Relative)),
            };
            buttonsFour.Children.Add(BFourimg);
            buttonsFour.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart11, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Height = 100, Width = 275 });

            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart12, CurrentLanguage), TextWrapping = TextWrapping.Wrap, FontSize = 40 });
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart13, CurrentLanguage), TextWrapping = TextWrapping.Wrap });
        }

        private void StatsClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();
            string[]? data = Statistics.GetStats();
            if(data == null)
            {
                MessageBox.Show("Error opening statistics file.\nDelete the file and/or restart the application","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                HowToPlayClick(new Button(), new RoutedEventArgs());
                return;
            }
            TextBlock title = new()
            {
                Text = "Statistics",
                FontSize = 40,
            };

            int percentageOne = (int) (100.0d / Convert.ToDouble(data[(int)StatisticType.OneSuitGamesStarted]) * Convert.ToDouble(data[(int)StatisticType.OneSuitGamesWon]));
            int percentageTwo = (int)(100.0d / Convert.ToDouble(data[(int)StatisticType.TwoSuitGamesStarted]) * Convert.ToDouble(data[(int)StatisticType.TwoSuitGamesWon]));
            int percentageFour = (int)(100.0d / Convert.ToDouble(data[(int)StatisticType.FourSuitGamesStarted]) * Convert.ToDouble(data[(int)StatisticType.FourSuitGamesWon]));
            if (data[(int)StatisticType.OneSuitGamesStarted] == "0") percentageOne = 0;
            if (data[(int)StatisticType.TwoSuitGamesStarted] == "0") percentageTwo = 0;
            if (data[(int)StatisticType.FourSuitGamesStarted] == "0") percentageFour = 0;

            SPInformation.Children.Add(title);
            TextBlock stats = new()
            {
                Text = "\n---General---\n" +
                $"Games started: {data[(int)StatisticType.GamesStarted]}\n" +
                $"Games won: {data[(int)StatisticType.GamesWon]}\n" +
                $"Cards moved: {data[(int)StatisticType.CardsMoved]}\n" +
                $"Suits assembled: {data[(int)StatisticType.SuitsAssembled]}\n" +
                $"Suits of spades assembled: {data[(int)StatisticType.SuitSpadesAssembled]}\n" +
                $"Suits of hearts assembled: {data[(int)StatisticType.SuitHeartsAssembled]}\n" +
                $"Suits of clubs assembled: {data[(int)StatisticType.SuitClubsAssembled]}\n" +
                $"Suits of diamonds assembled: {data[(int)StatisticType.SuitDiamondsAssembled]}\n" +
                $"Hints taken: {data[(int)StatisticType.HintsTaken]}\n" +
                $"\n---One suit games---\n" +
                $"One suit games played: {data[(int)StatisticType.OneSuitGamesStarted]}\n" +
                $"One suit games won: {data[(int)StatisticType.OneSuitGamesWon]}\n" +
                $"One suit games win percentage: {percentageOne}%\n" +
                $"\n---Two suit games---\n" +
                $"Two suit games played: {data[(int)StatisticType.TwoSuitGamesStarted]}\n" +
                $"Two suit games won: {data[(int)StatisticType.TwoSuitGamesWon]}\n" +
                $"Two suit games win percentage: {percentageTwo}%\n" +
                $"\n---Four suit games---\n" +
                $"Four suit games played: {data[(int)StatisticType.FourSuitGamesStarted]}\n" +
                $"Four suit games won: {data[(int)StatisticType.FourSuitGamesWon]}\n" +
                $"Four suit games win percentage: {percentageFour}%\n",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(stats);
        }

        private void GitHubClick(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/CrusaderSVK287/Spider-Solitaire",
                UseShellExecute = true
            };
            Process.Start(psi);
        }


        //Handles app update checking
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Would you like to download and install the version {LatestVersion} update automatically?\n" +
                                                      "Press yes to install automatically\n" +
                                                      "Press no to be redirected to the download page and download the update manually\n" +
                                                      "Press cancel to cancel the update", "Auto Update", MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                                                      MessageBoxResult.Yes);
            if (result == MessageBoxResult.Cancel) return;
            if (result == MessageBoxResult.No)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://github.com/CrusaderSVK287/Spider-Solitaire/releases/latest",
                    UseShellExecute = true
                };
                Process.Start(psi);
                return;
            }

            string[] files = { "SpiderSolitaireUpdater.deps.json", "SpiderSolitaireUpdater.dll", "SpiderSolitaireUpdater.exe", "SpiderSolitaireUpdater.pdb", "SpiderSolitaireUpdater.runtimeconfig.json" };
            foreach (string file in files)
            {
                if (File.Exists(@""+file)) continue;
                result = MessageBox.Show("It seems you do not have the updater installed or it's damaged.\nWould you like to be redirected to it's repository in order to download it?", "Updater not installed",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(result == MessageBoxResult.Yes)
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "https://github.com/CrusaderSVK287/Spider-Solitaire-Updater/releases/tag/v0.1.0",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                return;
            }
            File.WriteAllText(@"args.txt", LatestVersion);
            Process.Start("SpiderSolitaireUpdater.exe");
            Environment.Exit(0);
        }

        private string? LatestVersion { get; set; }

        //checks the games repository for the newest version number
        private async Task CheckForUpdate()
        {
            string version = "";
            await Task.Run(() => 
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/CrusaderSVK287/Spider-Solitaire/releases/latest", "tmp.html");
                };
                foreach (var line in File.ReadAllLines(@"tmp.html"))
                {
                    if (!line.Contains("<title>Release Spider Solitaire version")) continue;
                    version = line.Trim();
                    File.Delete(@"tmp.html");
                    break;
                }
                foreach (var line in version.Split(' '))
                {
                    if (line == null || line.Length == 0) continue;
                    if (line[0] >= '0' && line[0] <= '9') { version = line; break; }
                }
            });

            int[] latestVersion = new int[3];
            int[] currentVersion = new int[3];

            int tmpIndex = 0;
            foreach (var item in version.Split('.'))
            {
                latestVersion[tmpIndex++] = Convert.ToInt32(item);
            }
            tmpIndex = 0;
            foreach (var item in VersionText.Text.Split(new char[] { ' ', '.'}))
            {
                if (item == null || item.Length == 0 || item.Contains("Version")) continue;
                currentVersion[tmpIndex++] = Convert.ToInt32(item);
            }
            LatestVersion = $"{latestVersion[0]}.{latestVersion[1]}.{latestVersion[2]}";
            if (!IsUpToDate(currentVersion,latestVersion)) UpdateButton.Visibility = Visibility.Visible;
        }

        //compares current app version with the latest
        private static bool IsUpToDate(int[] current, int[] latest)
        {
            //major
            if (current[0] < latest[0]) return false;

            //minor
            if (current[1] < latest[1]) return false;

            //bugfix
            if (current[2] < latest[2]) return false;
            
            return true;
        }
    }
}
