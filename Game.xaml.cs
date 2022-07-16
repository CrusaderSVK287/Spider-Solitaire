﻿using System;
using System.Collections.Generic;
using System.IO;
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

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        private int cardOffset { get; set; }  //used to render the cards apart from each other
        private readonly Menu _menu;
        private readonly Action _destroy; //used as a delegate from menu.cs, destroys all references to current game for garbage collector to free the memory
        private readonly int _numberOfColours;
        private readonly Settings settings;
        public string CurrentLanguage { get; }
        private CommandType LastCommand { get; set; }
        private bool AnimationPlaying { get; set; } = false;   //used to track whether an animation is playing to avoid confusions
        private bool Loading { get; set; } = false;
        private List<Card> Selected { get; set; } = new(); //currenly selected card/s
        private int Selected_x { get; set; } //indexes of the currently selected card (and cards underneeth)
        private int NewCardNumber { get; set; } = 1;
        private int DecksSolved { get; set; } = 0;   //number of solved decks
        private int RemainingHints { get; set; }    //number of remaining hints

        private Deck deck = new Deck();
        public Game(int numberOfColours, bool isNewGame, Menu menu, Action Destroy, string language)
        {
            InitializeComponent();
            KeepAlive = false;
            _menu = menu;
            LastCommand = CommandType.select;
            settings = new();
            cardOffset = settings.CardSpacing;
            CurrentLanguage = language;
            RemainingHints = GetHints();
            LayOutCardOutlines();
            if (isNewGame)
            {
                deck.GenerateCards(numberOfColours);
                Statistics.IncreaseStat(StatisticType.GamesStarted);
                if (numberOfColours == 1) Statistics.IncreaseStat(StatisticType.OneSuitGamesStarted);
                if (numberOfColours == 2) Statistics.IncreaseStat(StatisticType.TwoSuitGamesStarted);
                if (numberOfColours == 4) Statistics.IncreaseStat(StatisticType.FourSuitGamesStarted);

                _ = deck.LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect, Loading, settings.PlayAnimations, settings.CardSizeFactor);
            }
            else
            {
                Loading = true;
                deck.LoadDeck();
                _ = deck.LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect, Loading, settings.PlayAnimations, settings.CardSizeFactor);
                Deck.LoadCommands(CardSelect,ColumnClick,NewCardsClick);
                Loading = false;
            }
            _numberOfColours = numberOfColours;
            _destroy = Destroy;
        }

        //Loads all cards that are being selected into a tmp list "Selected"
        private void CardSelect(object sender, MouseButtonEventArgs e)
        {
            if (deck.activeCards == null || Selected.Count > 0) return;
            int x = ((Image)sender).Name[0] - 97;
            int y = ((Image)sender).Name[1] - 65;

            char n = ((Image)sender).Name[2];
            while (n > 'A')
            {
                y += 26;    //-64 so it's not 65-65=0 in case of 'A'
                n--;
            }
            
            bool valid = deck.activeCards[x][y].Visible;
            for (int i = y+1, tmp=1; i < deck.activeCards[x].Count && valid; i++, tmp++)
            {
                if (deck.activeCards[x][y].Colour != deck.activeCards[x][i].Colour 
                    || deck.activeCards[x][y].Value != deck.activeCards[x][i].Value + tmp) valid = false;
            }
            if(valid)
            {
                SwichHitRegistration(false);
                for (int i = y; i < deck.activeCards[x].Count; i++)
                {
                    if (!Loading) _ = deck.activeCards[x][i].SelectedMove(i + 1, cardOffset, settings.PlayAnimations); //+1 due to y being an indexer
                    Selected.Add(deck.activeCards[x][i]);
                }
                deck.activeCards[x].RemoveRange(y, deck.activeCards[x].Count-y);
                Selected_x = x;
            }
            else
            {
                if (!Loading && settings.PlayAnimations) _ = deck.activeCards[x][y].InvalidMove(y + 1, cardOffset);  //+1 due to y being an indexer
            }
        }

        //Handles movement of cards from one column to another
        private async void ColumnClick(object sender, MouseButtonEventArgs e)
        {
            if (Selected == null || Selected.Count == 0) return;
            int column_index = ((Grid)sender).Name[3] - '0';
            if((deck.activeCards[column_index].Count == 0 || deck.activeCards[column_index].Last().Value-1 == Selected[0].Value) 
                && column_index != Selected_x)
            {

                if (!Loading) Command.LogCommand(new Command(CommandType.select, new string[] { Selected[0].Image.Name }));

                deck.activeCards[column_index].AddRange(Selected);
                foreach(var item in deck.activeCards[column_index])
                {
                    string name = "";
                    name += (char)(column_index + 97);
                    char a = (char)(deck.activeCards[column_index].IndexOf(item) + 65);
                    char b = 'A';
                    while (a > 'Z')
                    {
                        a -= (char)26;
                        b++;
                    }
                    name += $"{a}{b}";

                    item.Image.Name = name;
                }
                foreach(var item in deck.activeCards[column_index])
                {
                    Grid.SetColumn(item.Image, column_index + 1);
                    item.Image.Margin = new Thickness(0, (deck.activeCards[column_index].IndexOf(item)+1) * cardOffset + 5,0,0);
                }
                if (!Loading) Command.LogCommand(new Command(CommandType.move, new string[] { column_index.ToString() }));
                LastCommand = CommandType.move;
                if (!Loading) Statistics.IncreaseStat(StatisticType.CardsMoved, Selected.Count);
            }
            else
            {
                deck.activeCards[Selected_x].AddRange(Selected);
                foreach(var item in deck.activeCards[Selected_x])
                {
                    item.Image.Margin = new Thickness(0, (deck.activeCards[Selected_x].IndexOf(item)+1) * cardOffset + 5, 0, 0);
                }
            }
            Selected.Clear();
            await IsSuitAssembled();
            Refresh();
            SwichHitRegistration(true);
            if (DecksSolved == 8) Victory();
        }

        //Checks whether a deck is solved
        private async Task IsSuitAssembled()
        {
            while(AnimationPlaying) {if(!Loading) await Task.Delay(1); }
            AnimationPlaying = true;
            for (int i = 0; i < 10; i++) //iterates through all columns
            {
                foreach (var item in deck.activeCards[i])
                {
                    if (item.Visible == false || item.Value != 13) continue;
 
                    int index = deck.activeCards[i].IndexOf(item);
                    if (index + 13 != deck.activeCards[i].Count) continue;
                    for (int j = index+1, value = 12; j < deck.activeCards[i].Count; j++, value--)
                    {
                        if (deck.activeCards[i][j].Value != value || deck.activeCards[i][j].Colour != item.Colour) goto EndOfForeachLoop;
                    }

                    for (int j = deck.activeCards[i].Count-1; j >= index; j--)
                    {
                        if(!Loading && settings.PlayAnimations) await Task.Delay(25);
                        SolitaireGrid.Children.Remove(deck.activeCards[i][j].Image);
                    }

                    Image Image = new Image
                    {
                        Width = 89,
                        Height = 120,
                        Source = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/" + $"13{item.Colour}" + ".png")),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Visibility = Visibility.Visible,
                        Margin = new Thickness(-220 + DecksSolved * 15, 0, 0, 0)
                    };
                    SolitaireGrid.Children.Add(Image);
                    Grid.SetRow(Image, 1);
                    Grid.SetColumn(Image, 3);

                    char c = deck.activeCards[i].Last().Colour; //used by statistics, hence why it's here before removerange

                    deck.activeCards[i].RemoveRange(index, 13); //13 cards in full set
                    DecksSolved++;
                    if (RemainingHints >= 0) RemainingHints++;
                    AnimationPlaying = false;
                    if (!Loading)
                    {
                        Statistics.IncreaseStat(StatisticType.SuitsAssembled);
                        if (c == 'a') Statistics.IncreaseStat(StatisticType.SuitClubsAssembled);
                        if (c == 'b') Statistics.IncreaseStat(StatisticType.SuitDiamondsAssembled);
                        if (c == 'c') Statistics.IncreaseStat(StatisticType.SuitSpadesAssembled);
                        if (c == 'd') Statistics.IncreaseStat(StatisticType.SuitHeartsAssembled);
                    }

                    return;
                }
            EndOfForeachLoop:;
            }
            AnimationPlaying = false;
        }

        //Handles dealing of new row of cards
        public async void NewCardsClick(object sender, MouseButtonEventArgs e)
        {
            if (NewCardNumber > 5 || AnimationPlaying) return;
            for (int i = 0; i < 10; i++)
            {
                if (deck.activeCards[i].Count > 0) continue;
                InformationBox.Text = Localisation.SetText(TextType.GameInformationTextEmptyColumn, CurrentLanguage);
                await Task.Delay(10000);
                InformationBox.Text = " ";
                return;
            }
            AnimationPlaying = true;
            if (!Loading) Command.LogCommand(new Command(CommandType.add, null));

            Image[] newCardImages = { new1, new2, new3, new4, new5 };
            newCardImages[NewCardNumber - 1].Visibility = Visibility.Hidden;
            NewCardNumber++;

            for (int index = 0; index < 10; index++)
            {
                Card card = new Card(deck.values[deck.cardNum], deck.colors[deck.cardNum],true,
                    deck.activeCards[index].Count + 1, index, cardOffset, CardSelect, settings.CardSizeFactor);
                SolitaireGrid.Children.Add(card.Image);
                Grid.SetColumn(card.Image, index + 1);
                deck.cardNum++;
                deck.activeCards[index].Add(card);
                if (!Loading && settings.PlayAnimations) await Task.Delay(30);
            }
            Refresh();
            AnimationPlaying = false;
            LastCommand = CommandType.add;
        }

        //makes sure that all cards are up and the correct ones are being shown
        private void Refresh()
        {
            if (deck.activeCards == null) return;
            for (int i = 0; i < 10; i++)
            {
                foreach(var item in deck.activeCards[i])
                {
                    SolitaireGrid.Children.Remove(item.Image);
                    SolitaireGrid.Children.Add(item.Image);
                }
                if (deck.activeCards[i].Count > 0 && deck.activeCards[i].Last().Visible == false)
                {
                    deck.activeCards[i].Last().Visible = true;
                    deck.activeCards[i].Last().GetColour();
                }
            }
        }

        //switches hittestvisible property of cards
        private void SwichHitRegistration(bool hit)
        {
            for (int i = 0; i < 10; i++)
            {
                foreach(var item in deck.activeCards[i])
                {
                    item.Image.IsHitTestVisible = hit;
                }
            }
        }

        //handels victory "screen"
        private async void Victory()
        {
            if (File.Exists(@"autosave.soli")) File.Delete(@"autosave.soli");
            Statistics.IncreaseStat(StatisticType.GamesWon);
            if (deck.colors.Any(e => e=='b' || e=='a')) Statistics.IncreaseStat(StatisticType.FourSuitGamesWon);
            else if (deck.colors.Any(e => e == 'd')) Statistics.IncreaseStat(StatisticType.TwoSuitGamesWon);
            else if (deck.colors.Any(e => e == 'c')) Statistics.IncreaseStat(StatisticType.OneSuitGamesWon);

            Hint.IsEnabled = false;
            Back.IsEnabled = false;
            Restart.IsEnabled = false;
            Exit.IsEnabled = false;
            await Task.Delay(500);
            VictoryText.Text = Localisation.SetText(TextType.GameVictoryText, CurrentLanguage) + "!";
            VictoryText.Visibility = Visibility.Visible;
            await Task.Delay(5000);
            NavigationService.Navigate(_menu);
            _destroy();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_menu);
            _destroy();
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            if (LastCommand == CommandType.select || AnimationPlaying || Selected.Count > 0) return;
            try
            {
                var Lines = File.ReadAllLines(@"autosave.soli");
                File.WriteAllLines(@"autosave.soli", Lines.Take(Lines.Length - (int)LastCommand).ToArray());
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
            Game game = new Game(_numberOfColours, false, _menu, _destroy, CurrentLanguage);
            if (game != null) NavigationService.Navigate(game);
            _destroy();
        }

        private void RestartClick(object sender, RoutedEventArgs e)
        {
            if (AnimationPlaying || Selected.Count > 0) return;
            MessageBoxResult result = MessageBox.Show("This action will reset the game to the starting point,\nthere is no going back",
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            try
            {
                var Lines = File.ReadAllLines(@"autosave.soli");
                File.WriteAllLines(@"autosave.soli", Lines.Take(105).ToArray());
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Restart", MessageBoxButton.OK, MessageBoxImage.Warning); }
            Game game = new Game(_numberOfColours, false, _menu, _destroy, CurrentLanguage);
            if (game != null) NavigationService.Navigate(game);
            _destroy();
        }

        private void HintClick(object sender, RoutedEventArgs e)
        {
            if (Selected.Count > 0 || AnimationPlaying) return;

            if (settings.HintMode == 2) return;
            HintBoxUpdate();
            if(RemainingHints == 0) return;
            if (!Loading) Statistics.IncreaseStat(StatisticType.HintsTaken);
            RemainingHints--;
            //Parent = same color, value +1, Half-Parent = different color, value +1.
            //internal method, checks whether a card doesnt already lay on it's "parent" card (e.g. 7A is under 8A),
            //this is to prevent really unhelpfull hints

            static bool LaysOnParentCard(List<Card> pile, Card child)
            {
                if (pile.Count < 2) return false;
                Card? previous = Deck.PreviousCard(pile, child);
                return previous != null && previous.Visible && IsParent(child,previous);
            }

            static bool LaysOnHalfParentCard(List<Card> pile, Card child)
            {
                if (pile.Count < 2) return false;
                Card? previous = Deck.PreviousCard(pile, child);
                return previous != null && previous.Visible && IsHalfParent(child, previous);
            }

            static bool IsParent(Card child, Card parent)
            {
                return child.Colour==parent.Colour && child.Value == parent.Value-1;
            }

            static bool IsHalfParent(Card child, Card parent)
            {
                return child.Value == parent.Value - 1;
            }

            //checks the last cards of each column accounting for colour
            for (int i = 0; i < 10; i++)    //itterates through all the columns
            {
                if (deck.activeCards[i] == null) continue;
                foreach (var item in deck.activeCards[i])   //itterates through the column itself
                {
                    if (item.Visible == false) continue;
                    if (!CardMoveable(deck.activeCards[i], deck.activeCards[i].IndexOf(item))) continue;
                    for (int j = 0; j < 10; j++)
                    {
                        if (deck.activeCards[j].Count == 0 || j == i) continue;
                        if (deck.activeCards[j].Last().Value == item.Value + 1 &&
                            deck.activeCards[j].Last().Colour == item.Colour)
                        {
                            if (LaysOnParentCard(deck.activeCards[i], item)) goto EndOfForeachLoopOne;
                            ShowHintFrames(i, deck.activeCards[i].IndexOf(item), j);
                            return;
                        }
                    }
                }
                EndOfForeachLoopOne:;
            }

            //checks the last cards of each volumn NOT accounting for colour
            for (int i = 0; i < 10; i++)    //itterates through all the columns
            {
                if (deck.activeCards[i] == null) continue;
                foreach (var item in deck.activeCards[i])   //itterates through the column itself
                {

                    if (item.Visible == false) continue;
                    if (!CardMoveable(deck.activeCards[i], deck.activeCards[i].IndexOf(item))) continue;
                    for (int j = 0; j < 10; j++)
                    {
                        if (deck.activeCards[j].Count == 0 || j == i) continue;
                        if (deck.activeCards[j].Last().Value == item.Value + 1)
                        {
                            if (LaysOnParentCard(deck.activeCards[i], item) || 
                                (LaysOnHalfParentCard(deck.activeCards[i],item) && IsHalfParent(item,deck.activeCards[j].Last()))
                                ) goto EndOfForeachLoopTwo;
                            ShowHintFrames(i, deck.activeCards[i].IndexOf(item), j);
                            return;
                        }
                    }
                }
                EndOfForeachLoopTwo:;
            }

            //checks for columns with no cards
            for (int i = 0; i < 10; i++)    //itterates through all the columns
            {
                if (deck.activeCards[i] == null || deck.activeCards[i].Count==0) continue;
                if (CardMoveable(deck.activeCards[i], 0)) continue;
                foreach (var item in deck.activeCards[i])   //itterates through the column itself
                {

                    if (item.Visible == false) continue;
                    if (!CardMoveable(deck.activeCards[i], deck.activeCards[i].IndexOf(item))) continue;
                    for (int j = 0; j < 10; j++)
                    {
                        if (deck.activeCards[j].Count == 0 && j!=i)
                        {
                            ShowHintFrames(i, deck.activeCards[i].IndexOf(item), j);
                            return;
                        }
                    }
                }
            }

            //check whether the player can add new cards
            if(NewCardNumber <= 5)
            {
                ShowHintFrames(NewCardNumber);
                return;
            }

            //no possible meaningfull move was found
            MessageBoxResult result = MessageBox.Show(Localisation.SetText(TextType.GameNoMoreMovesPossible, CurrentLanguage)
                , "No more moves",MessageBoxButton.YesNo,MessageBoxImage.Information);
            if(result == MessageBoxResult.Yes) RestartClick(new Image(), new RoutedEventArgs());
        }

        //Updates the information on amount of hints
        private async void HintBoxUpdate()
        {
            if (RemainingHints == 0)
            {
                HintBox.Text = Localisation.SetText(TextType.GameHintTextBoxNoMoreHints, CurrentLanguage);
            }
            if (RemainingHints > 0)
            {
                HintBox.Text = $"{Localisation.SetText(TextType.GameHintTextBoxYouHave, CurrentLanguage)} {RemainingHints-1} {Localisation.SetText(TextType.GameHintTextBoxRemaining, CurrentLanguage)}";
            }
            await Task.Delay(5000);
            HintBox.Text = " ";
        }

        //determines whether the current card can be moved
        private static bool CardMoveable(List<Card> pile,int startingIndex)
        {
            for (int i = startingIndex+1, sub = 1; i < pile.Count; i++, sub++)
            {
                if (pile[i].Colour != pile[startingIndex].Colour ||
                    pile[i].Value != pile[startingIndex].Value - sub) return false;
            }
            return true;
        }

        //Renders the gold hint frames based on column and starting index
        private async void ShowHintFrames(int columnIndex, int startingCardIndex, int destinationColumnIndex)
        {
            List<Image> hintFrames = new List<Image>();
            for(int i = startingCardIndex; i < deck.activeCards[columnIndex].Count; i++)
            {
                Image image = new()
                {
                    Width = Convert.ToInt32(95.0f * settings.CardSizeFactor),
                    Height = Convert.ToInt32(120.0f * settings.CardSizeFactor),
                    Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Stretch = Stretch.UniformToFill,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, (i+1) * cardOffset + 2, 0, 0)
                };
                SolitaireGrid.Children.Add(image);
                Grid.SetColumn(image, columnIndex+1);
                hintFrames.Add(image);
            }
            hintFrames.Last().Height = Convert.ToInt32(126.0f * settings.CardSizeFactor);

            int yMargin = (deck.activeCards[destinationColumnIndex].Count == 0) ? 1 : deck.activeCards[destinationColumnIndex].Count;
            Image imageTwo = new()
            {
                Width = Convert.ToInt32(95.0f * settings.CardSizeFactor),
                Height = Convert.ToInt32(126.0f * settings.CardSizeFactor),
                Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.UniformToFill,
                IsHitTestVisible = false,
                Margin = new Thickness(0, yMargin * cardOffset + 2, 0, 0)
            };
            SolitaireGrid.Children.Add(imageTwo);
            Grid.SetColumn(imageTwo, destinationColumnIndex + 1);
            hintFrames.Add(imageTwo);

            for (int y = 0; y < 100 && Selected.Count == 0 && !AnimationPlaying; y++) { await Task.Delay(25); }
            foreach (var item in hintFrames)
            {
                if(item != null && SolitaireGrid != null)SolitaireGrid.Children.Remove(item);
            }
        }


        //Renders gold hint frames based on how many new rows have been dealt
        private async void ShowHintFrames(int newCardNumber)
        {
            Image image = new()
            {
                Width = 95,
                Height = 126,
                Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Stretch = Stretch.None,
                IsHitTestVisible = false,
                Margin = new Thickness((-5*(5-newCardNumber))-3, 0, 0, 0)
            };
            SolitaireGrid.Children.Add(image);
            Grid.SetColumn(image, 10);
            Grid.SetRow(image, 1);

            for (int y = 0; y < 100 && Selected.Count == 0 && !AnimationPlaying; y++) { await Task.Delay(25); }
            if (image != null && SolitaireGrid != null) SolitaireGrid.Children.Remove(image);
        }

        //Lays out the dark green outlines
        private void LayOutCardOutlines()
        {
            for (int i = 0; i < 10; i++)
            {
                Image image = new()
                {
                    Width = Convert.ToInt32(89.0f * settings.CardSizeFactor),
                    Height = Convert.ToInt32(120.0f * settings.CardSizeFactor),
                    Source = new BitmapImage(new Uri(@"assets/card_outline.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Stretch = Stretch.UniformToFill,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, cardOffset + 5, 0, 0)
                };
                SolitaireGrid.Children.Add(image);
                Grid.SetColumn(image, i + 1);
            }
        }


        //handles the button changing background
        private async void SPButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            while(SPButtons.IsMouseOver == true)
            {
                if (Exit.IsMouseOver == true) InformationBox.Text = Localisation.SetText(TextType.GameInformationTextButtonsHome,CurrentLanguage);
                else if (Restart.IsMouseOver == true) InformationBox.Text = Localisation.SetText(TextType.GameInformationTextButtonsRestart, CurrentLanguage);
                else if (Back.IsMouseOver == true) InformationBox.Text = Localisation.SetText(TextType.GameInformationTextButtonsUndo, CurrentLanguage);
                else if (Hint.IsMouseOver == true) InformationBox.Text = Localisation.SetText(TextType.GameInformationTextButtonsHint, CurrentLanguage);
                await Task.Delay(50);
            }
            InformationBox.Text = " ";
        }

        private class Settings
        {
            public float CardSizeFactor { get; set; }
            public int CardSpacing { get; set; }
            public bool PlayAnimations { get; set; }
            public int HintMode { get; set; }

            public Settings()
            {
                LoadSettings();
            }

            private bool LoadSettings()
            {
                if (!File.Exists(@"settings.txt")) return false;
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
                                CardSizeFactor = (float)Convert.ToDouble(data[1]) / 100.0f;
                                break;
                            case 1:
                                CardSpacing = Convert.ToInt32(data[1]);
                                break;
                            case 2:
                                HintMode = Convert.ToInt32(data[1]);
                                break;
                            case 3:
                                PlayAnimations = (data[1] == "1") ? true : false;
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
        }

        private int GetHints()
        {
            if (settings.HintMode == 2) Hint.IsEnabled = false;
            return (settings.HintMode == 0) ? -1 : 3;
        }
    }
}
