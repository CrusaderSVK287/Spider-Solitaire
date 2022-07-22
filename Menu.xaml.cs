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
        private bool Tutorial { get; set; }
        public Menu(string language)
        {
            InitializeComponent();
            CurrentLanguage = language;
            Tutorial = false;
            _ = CheckForUpdate();
        }

        private void DestroyGameReference()
        {
            if (game == null) return;
            game.SolitaireGrid.Children.Clear();
            game.SolitaireGrid = null;
            game = null;
            
            GC.Collect();
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
            if(game != null && !Tutorial)
            {
                NavigationService.Navigate(game);
                return;
            }
            if (File.Exists(@"autosave.soli")) StartGame(-1, false);
            else
            {
                Load.IsEnabled = false;
                MessageBox.Show("The save file is missing.","Warning",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }

        private void StartGame(int numberOfSuits, bool isNewGame)
        {
            if (game != null) DestroyGameReference();
            game = new Game(numberOfSuits, isNewGame, this, CurrentLanguage);
            NavigationService.Navigate(game);
        }

        private void MenuLoaded(object sender, RoutedEventArgs e)
        {
            Load.IsEnabled = File.Exists(@"autosave.soli");
            WelcomeBanner.Text = Localisation.SetText(TextType.MenuWelcomeBanner, CurrentLanguage);
            OneSuit.Content = Localisation.SetText(TextType.MenuOneSuitButton, CurrentLanguage);
            TwoSuit.Content = Localisation.SetText(TextType.MenuTwoSuitButton, CurrentLanguage);
            FourSuit.Content = Localisation.SetText(TextType.MenuFourSuitButton, CurrentLanguage);
            Load.Content = Localisation.SetText(TextType.MenuLoadGameButton, CurrentLanguage);
            HowToPlay.Content = Localisation.SetText(TextType.MenuHowToPlayButton, CurrentLanguage);
            Stats.Content = Localisation.SetText(TextType.MenuStatisticsButton, CurrentLanguage);
            UpdateButton.Content = Localisation.SetText(TextType.MenuUpdateButton, CurrentLanguage);
            Licence.Content = Localisation.SetText(TextType.MenuLicenceButton, CurrentLanguage);
            if (File.Exists(@"note.md")) ReadNote();
            else HowToPlayClick(new Button(), new RoutedEventArgs());
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
                    Tutorial = true;
                    LoadGameClick(new Button(), new RoutedEventArgs());
                    Tutorial = false;
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
            buttonsOne.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart08, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Width = 275 });


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
            buttonsTwo.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart09, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Width = 275 });


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
            buttonsThree.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart10, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Width = 275, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });


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
            buttonsFour.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart11, CurrentLanguage), TextWrapping = TextWrapping.Wrap, Width = 275 });

            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart12, CurrentLanguage), TextWrapping = TextWrapping.Wrap, FontSize = 40 });
            SPInformation.Children.Add(new TextBlock() { Text = Localisation.SetText(TextType.MenuSPInformationHowToPlayPart13, CurrentLanguage), TextWrapping = TextWrapping.Wrap });
            InformationScrollViewer.ScrollToTop();
        }

        private void StatsClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();
            string[]? data = Statistics.GetStats();
            if(data == null)
            {
                MessageBox.Show(Localisation.SetText(TextType.MenuSPInformationStatisticsErrorOpeningFile,CurrentLanguage),"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                HowToPlayClick(new Button(), new RoutedEventArgs());
                return;
            }
            TextBlock title = new()
            {
                Text = Localisation.SetText(TextType.MenuSPInformationStatisticsTitle, CurrentLanguage),
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
                Text = "\n---"+Localisation.SetText(TextType.MenuSPInformationStatisticsGeneral,CurrentLanguage)+"---\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsGamesStarted, CurrentLanguage)}: {data[(int)StatisticType.GamesStarted]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsGamesWon, CurrentLanguage)}: {data[(int)StatisticType.GamesWon]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsCardsMoved, CurrentLanguage)}: {data[(int)StatisticType.CardsMoved]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsSuitsAssembled, CurrentLanguage)}: {data[(int)StatisticType.SuitsAssembled]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsSpadesAssembled, CurrentLanguage)}: {data[(int)StatisticType.SuitSpadesAssembled]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsHeartsAssembled, CurrentLanguage)}: {data[(int)StatisticType.SuitHeartsAssembled]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsClubsAssembled, CurrentLanguage)}: {data[(int)StatisticType.SuitClubsAssembled]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsDiamondsAssembled, CurrentLanguage)}: {data[(int)StatisticType.SuitDiamondsAssembled]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsHintsTaken, CurrentLanguage)}: {data[(int)StatisticType.HintsTaken]}\n" +
                $"\n---{Localisation.SetText(TextType.MenuSPInformationStatisticsOneSuitGames, CurrentLanguage)}---\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsOneSuitGamesPlayed, CurrentLanguage)}: {data[(int)StatisticType.OneSuitGamesStarted]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsOneSuitGamesWon, CurrentLanguage)}: {data[(int)StatisticType.OneSuitGamesWon]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsOneSuitGamesPercentage, CurrentLanguage)}: {percentageOne}%\n" +
                $"\n---{Localisation.SetText(TextType.MenuSPInformationStatisticsTwoSuitGames, CurrentLanguage)}---\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsTwoSuitGamesPlayed, CurrentLanguage)}: {data[(int)StatisticType.TwoSuitGamesStarted]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsTwoSuitGamesWon, CurrentLanguage)}: {data[(int)StatisticType.TwoSuitGamesWon]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsTwoSuitGamesPercentage, CurrentLanguage)}: {percentageTwo}%\n" +
                $"\n---{Localisation.SetText(TextType.MenuSPInformationStatisticsFourSuitGames, CurrentLanguage)}---\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsFourSuitGamesPlayed, CurrentLanguage)}: {data[(int)StatisticType.FourSuitGamesStarted]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsFourSuitGamesWon, CurrentLanguage)}: {data[(int)StatisticType.FourSuitGamesWon]}\n" +
                $"{Localisation.SetText(TextType.MenuSPInformationStatisticsFourSuitGamesPercentage, CurrentLanguage)}: {percentageFour}%\n",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(stats);
            InformationScrollViewer.ScrollToTop();
        }

        private void LicenceClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();
            TextBlock title = new()
            {
                Text = Localisation.SetText(TextType.MenuLicenceButton, CurrentLanguage),
                FontSize = 40,
            };

            TextBlock licence = new()
            {
                Text = "\nMIT License\n\n" +
                "Copyright (c) 2022 Lukáš Belán\n\n" +
                "Permission is hereby granted, free of charge, to any person obtaining a copy " +
                "of this software and associated documentation files (the \"Software\"), to deal " +
                "in the Software without restriction, including without limitation the rights " +
                "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell " +
                "copies of the Software, and to permit persons to whom the Software is " +
                "furnished to do so, subject to the following conditions:\n\n" +
                "The above copyright notice and this permission notice shall be included in all " +
                "copies or substantial portions of the Software.\n\n" +
                "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR " +
                "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, " +
                "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE " +
                "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER " +
                "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, " +
                "OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE " +
                "SOFTWARE.",
                TextWrapping = TextWrapping.Wrap,
            };

            SPInformation.Children.Add(title);
            SPInformation.Children.Add(licence);
            InformationScrollViewer.ScrollToTop();
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
            MessageBoxResult result = MessageBox.Show($"{Localisation.SetText(TextType.UpdateBoxUpdateAutomaticallyQuestionPart1,CurrentLanguage)} {LatestVersion} " +
                                                         Localisation.SetText(TextType.UpdateBoxUpdateAutomaticallyQuestionPart1, CurrentLanguage), 
                                                         "Auto Update", MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
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
                result = MessageBox.Show(Localisation.SetText(TextType.UpdateBoxUpdateAutomaticallyUpdaterNotInstalled, CurrentLanguage), "Updater failed",
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
            if (current[1] < latest[1] &&
                current[0] <= latest[0]) return false;

            //bugfix
            if (current[2] < latest[2] &&
                current[1] <= latest[1] &&
                current[0] <= latest[0]) return false;
            
            return true;
        }

        //Read an update note if there is any
        private void ReadNote()
        {
            try
            {
                var data = File.ReadAllLines(@"note.md");
                SPInformation.Children.Clear();
                foreach (var item in data)
                {
                    int fontSize = 10;
                    string text = "ERROR";
                     if (item.StartsWith("# "))
                    {
                        fontSize = 40;
                        text = item[2..];
                    }
                     else if (item.StartsWith("## "))
                    {
                        fontSize = 30;
                        text = item[3..];
                    }
                     else if (item.StartsWith("* "))
                    {
                        fontSize = 20;
                        text = " ● " + item[2..];
                    }
                     else
                    {
                        text = item;
                        fontSize = 20;
                    }
                    SPInformation.Children.Add(new TextBlock() { FontSize = fontSize, Text = text, TextWrapping = TextWrapping.Wrap });
                }
                File.Delete(@"note.md");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
        }
    }
}
