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

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Game? game;
        public Menu()
        {
            InitializeComponent();
            TextBlock tb = new TextBlock();
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
            game = new Game(numberOfSuits, isNewGame, this, DestroyGameReference);
            NavigationService.Navigate(game);
        }

        private void MenuLoaded(object sender, RoutedEventArgs e)
        {
            Load.IsEnabled = File.Exists(@"autosave.soli");
        }

        private void HowToPlayClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();

            TextBlock HowToPlay = new()
            {
                Text = "How to play",
                FontSize = 40
            };
            SPInformation.Children.Add(HowToPlay);

            TextBlock a = new()
            {
                Text = "The goal of spider solitaire is, as with all solitaires, to clear the entire deck. This is done by assembling a full suit of cards. " +
                "Each suit consists of 13 cards ranging from ace to king, having the same colour. In order to assemble a suit, all 13 cards must be in order. " +
                "Then the suit is removed from the deck, once all 8 suits are assembled, the game ends in victory. Below are all 4 kinds of suits\n(from the left: clubs, diamonds, spades, hearts)",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(a);

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

            TextBlock c = new()
            {
                Text = "When selecting the number of suits from the main menu, you are selecting how many suits (or colours) will be used in game. This has no " +
                "effect on the total number of cards. The number of total cards is always 104 (8 suits times 13 cards in one suit)\n\n" +
                "To select a card or cards, click on the image of the card you want to select, an animation will show you which card has been selected. You can then click on a column where you would like to place them. If you cannot place them in the selected column or you selected the very same column, the cards will return to their original place. In the game, you are allowed to move only cards that are facing upwards (you can see the suit and value), and you can move this pile only on a card that follows the ascention order " +
                "(7 can only go on top of 8, 3 on top of 4 etc..). You can, however, place a cards of different suit on top of each other. For reference, here is an image of a full suit of spades.",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(c);

            StackPanel d = new()
            {
                Orientation = Orientation.Horizontal,
            };
            SPInformation.Children.Add(d);
            for (int C = 13; C >= 1; C--)
            {
                Image img = new()
                {
                    Width = (C==1)?89:25,
                    Height = 120,
                    Stretch = Stretch.None,
                    Source = new BitmapImage(new Uri(@"assets/" + C.ToString() + "c.png", UriKind.Relative)),
                };
                d.Children.Add(img);
            }

            TextBlock f = new()
            {
                Text = "When you move cards away and a uncovered (blue) card becomes the top most one, it reveals it's suit and value. " +
                "Once you clear out an entire column such as there are no cards remaining and you can see a green outline, that means you can place any card or sequence of cards" +
                " in there, refer to the outline image below.",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(f);

            Image g = new()
            {
                Width = 89,
                Height = 120,
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/card_outline.png", UriKind.Relative)),
            };
            SPInformation.Children.Add(g);
            TextBlock h = new()
            {
                Text = "Since the biggest card is a King, it means that Kings can only be moved into empty columns. On the other hand, since aces are the smallest card," +
                " no card can be places on them. Once you run out of possible moves, you can deal another row of 10 cards by clicking on the blue uncovered cards in the bottom right corner. This will add one card to each column. You cannot have an empty column when dealing new cards however." +
                "\n\nThis is basically it for basic game mechanics, scroll down for button explanation and advanced features.\n\n",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(h);

            TextBlock ButtonExplanation = new()
            {
                Text = "Buttons",
                FontSize = 40
            };
            SPInformation.Children.Add(ButtonExplanation);

            TextBlock i = new()
            {
                Text = "In the game there are 4 buttons, each with their own function.\n",
                TextWrapping = TextWrapping.Wrap,
            };
            SPInformation.Children.Add(i);

            StackPanel buttonsOne = new()
            {
                Orientation = Orientation.Horizontal,
            };
            SPInformation.Children.Add(buttonsOne);
            Image Bimg = new()
            {
                Width = 50,
                Height = 50,
                Margin = new Thickness(0,0,20,0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/home_icon_pressed.png", UriKind.Relative)),
            };
            buttonsOne.Children.Add(Bimg);
            TextBlock buttonOneDesc = new()
            {
                Height = 80,
                Width = 275,
                Text = "This button returns you to the main menu, you can press load game if you wish to continue",
                TextWrapping = TextWrapping.Wrap,
            };
            buttonsOne.Children.Add(buttonOneDesc);

            StackPanel buttonsTwo = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0,10,0,0),
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
            TextBlock buttonTwoDesc = new()
            {
                Height = 80,
                Width = 275,
                Text = "This button restarts the game from the beggining with the same cards.",
                TextWrapping = TextWrapping.Wrap,
            };
            buttonsTwo.Children.Add(buttonTwoDesc);


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
                Margin = new Thickness(0, 0, 20, 0),
                Stretch = Stretch.None,
                Source = new BitmapImage(new Uri(@"assets/back_icon_pressed.png", UriKind.Relative)),
            };
            buttonsThree.Children.Add(BThreeimg);
            TextBlock buttonThreeDesc = new()
            {
                Height = 30,
                Width = 275,
                Text = "This button undos your last move",
                TextWrapping = TextWrapping.Wrap,
            };
            buttonsThree.Children.Add(buttonThreeDesc);


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
            TextBlock buttonFourDesc = new()
            {
                Height = 100,
                Width = 275,
                Text = "This button gives you a hint by highlighting a card or cards that can be picked up and a suggested place to put down.",
                TextWrapping = TextWrapping.Wrap,
            };
            buttonsFour.Children.Add(buttonFourDesc);

            TextBlock Advanced = new()
            {
                Text = "\nAdvanced features",
                FontSize = 40
            };
            SPInformation.Children.Add(Advanced);
        }

        private void StatsClick(object sender, RoutedEventArgs e)
        {
            SPInformation.Children.Clear();
            TextBlock tb = new()
            {
                Text = "Lorem ipsum is not implementum",
            };
            SPInformation.Children.Add(tb);
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
    }
}
