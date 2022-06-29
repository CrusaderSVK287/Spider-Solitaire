using System;
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
        const int cardOffset = 20;  //used to render the cards apart from each other
        public Menu _menu;
        public Action _destroy; //used as a delegate from menu.cs, destroys all references to current game for garbage collector to free the memory
        public readonly int _numberOfColours;
        public CommandType LastCommand { get; set; }
        public bool AnimationPlaying { get; set; } = false;   //used to track whether an animation is playing to avoid confusions
        public bool Loading { get; set; } = false;
        private List<Card> Selected { get; set; } = new(); //currenly selected card/s
        int Selected_x { get; set; } //indexes of the currently selected card (and cards underneeth)
        int NewCardNumber { get; set; } = 1;
        int DecksSolved { get; set; } = 0;   //number of solved decks

        private Deck deck = new Deck();
        public Game(int numberOfColours, bool isNewGame, Menu menu, Action Destroy)
        {
            InitializeComponent();
            KeepAlive = false;
            _menu = menu;
            LastCommand = CommandType.select;
            if (isNewGame)
            {
                deck.GenerateCards(numberOfColours);
                _ = deck.LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect, Loading);
            }
            else
            {
                Loading = true;
                deck.LoadDeck();
                _ = deck.LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect, Loading);
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
                    if (!Loading) _ = deck.activeCards[x][i].SelectedMove(i + 1, cardOffset); //+1 due to y being an indexer
                    Selected.Add(deck.activeCards[x][i]);
                }
                deck.activeCards[x].RemoveRange(y, deck.activeCards[x].Count-y);
                Selected_x = x;
            }
            else
            {
                if (!Loading) _ = deck.activeCards[x][y].InvalidMove(y + 1, cardOffset);  //+1 due to y being an indexer
            }
        }

        private void ColumnClick(object sender, MouseButtonEventArgs e)
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
                    name += (char)(deck.activeCards[column_index].IndexOf(item) + 65);
                    item.Image.Name = name;
                }
                foreach(var item in deck.activeCards[column_index])
                {
                    Grid.SetColumn(item.Image, column_index + 1);
                    item.Image.Margin = new Thickness(0, (deck.activeCards[column_index].IndexOf(item)+1) * cardOffset + 5,0,0);
                }
                if (!Loading) Command.LogCommand(new Command(CommandType.move, new string[] { column_index.ToString() }));
                LastCommand = CommandType.move;
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
            IsDeckSolved();
            Refresh();
            SwichHitRegistration(true);
            if (DecksSolved == 8) Victory();
        }

        private void IsDeckSolved()
        {
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
                    for (int j = index; j < deck.activeCards[i].Count; j++)
                    {
                        deck.activeCards[i][j].Image.Visibility = Visibility.Hidden;
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

                    deck.activeCards[i].RemoveRange(index, 13); //13 cards in full set
                    DecksSolved++;
                    return;
                }
            EndOfForeachLoop:;
            }
        }

        public async void NewCardsClick(object sender, MouseButtonEventArgs e)
        {
            if (NewCardNumber > 5 || AnimationPlaying) return;
            for (int i = 0; i < 10; i++)
            {
                if (deck.activeCards[i].Count > 0) continue;
                MessageBox.Show("You cannot add new card to an empty column");
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
                    deck.activeCards[index].Count + 1, index, cardOffset, CardSelect);
                SolitaireGrid.Children.Add(card.Image);
                Grid.SetColumn(card.Image, index + 1);
                deck.cardNum++;
                deck.activeCards[index].Add(card);
                if (!Loading) await Task.Delay(30);
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
        private void Victory()
        {
            if (File.Exists(@"autosave.soli")) File.Delete(@"autosave.soli");
            MessageBox.Show("Victory");
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
            Game game = new Game(_numberOfColours, false, _menu, _destroy);
            if (game != null) NavigationService.Navigate(game);
            _destroy();
        }

        private void RestartClick(object sender, RoutedEventArgs e)
        {
            if (AnimationPlaying || Selected.Count > 0) return;
            MessageBoxResult result = MessageBox.Show("Are you sure? This action will reset the game to the starting point,\nthere is no going back",
                "Information", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            try
            {
                var Lines = File.ReadAllLines(@"autosave.soli");
                File.WriteAllLines(@"autosave.soli", Lines.Take(105).ToArray());
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
            Game game = new Game(_numberOfColours, false, _menu, _destroy);
            if (game != null) NavigationService.Navigate(game);
            _destroy();
        }

        private void HintClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)    //itterates through all the columns
            {
                if (deck.activeCards[i] == null) continue;
                foreach (var item in deck.activeCards[i])   //itterates through the column itself
                {

                    if (item.Visible == false) continue;
                    if (!CardMoveable(deck.activeCards[i], deck.activeCards[i].IndexOf(item))) continue;
                    //checks the last cards of each column accounting for colour
                    for (int j = 0; j < 10; j++)
                    {
                        if (deck.activeCards[j].Count == 0 || j==i) continue;
                        if (deck.activeCards[j].Last().Value == item.Value + 1 &&
                            deck.activeCards[j].Last().Colour == item.Colour)
                        {
                            ShowHintFrames(i, deck.activeCards[i].IndexOf(item), j);
                            return;
                        }
                    }
                    //checks the last cards of each volumn NOT accounting for colour
                    for (int j = 0; j < 10; j++)
                    {
                        if (deck.activeCards[j].Count == 0 || j == i) continue;
                        if (deck.activeCards[j].Last().Value == item.Value + 1)
                        {
                            ShowHintFrames(i, deck.activeCards[i].IndexOf(item), j);
                            return;
                        }
                    }
                    //checks for columns with no cards
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

       /* for (int i = y+1, tmp=1; i<deck.activeCards[x].Count && valid; i++, tmp++)
            {
                if (deck.activeCards[x][y].Colour != deck.activeCards[x][i].Colour 
                    || deck.activeCards[x][y].Value != deck.activeCards[x][i].Value + tmp) valid = false;
            }*/


    private async void ShowHintFrames(int columnIndex, int startingCardIndex, int destinationColumnIndex)
        {
            List<Image> hintFrames = new List<Image>();
            for(int i = startingCardIndex; i < deck.activeCards[columnIndex].Count; i++)
            {
                Image image = new()
                {
                    Width = 95,
                    Height = 120,
                    Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Stretch = Stretch.None,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0, (i+1) * cardOffset + 2, 0, 0)
                };
                SolitaireGrid.Children.Add(image);
                Grid.SetColumn(image, columnIndex+1);
                hintFrames.Add(image);
            }
            hintFrames.Last().Height = 126;
            
            Image imageTwo = new()
            {
                Width = 95,
                Height = 126,
                Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.None,
                IsHitTestVisible = false,
                Margin = new Thickness(0, (deck.activeCards[destinationColumnIndex].Count) * cardOffset + 2, 0, 0)
            };
            SolitaireGrid.Children.Add(imageTwo);
            Grid.SetColumn(imageTwo, destinationColumnIndex + 1);
            hintFrames.Add(imageTwo);

            for (int y = 0; y < 100 && Selected.Count == 0; y++) { await Task.Delay(50); }
            foreach (var item in hintFrames)
            {
                if(item != null && SolitaireGrid != null)SolitaireGrid.Children.Remove(item);
            }
        }
    }
}

/*          List<Image> hintFrames = new List<Image>();
            int i = 0;
            foreach(var item in deck.activeCards[3])
            {
                i++;
                if (item.Visible == false) continue;
                Image image = new()
                {
                    Width = 95,
                    Height = 120,
                    Source = new BitmapImage(new Uri(@"assets/hint_frame.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Top,
                    Stretch = Stretch.None,
                    IsHitTestVisible = false,
                    Margin = new Thickness(0,i * cardOffset +2 ,0,0)
                };
                SolitaireGrid.Children.Add(image);
                Grid.SetColumn(image, 4);
                hintFrames.Add(image);
            }
            hintFrames.Last().Height = 126;
            for(int y = 0; y < 100 && Selected.Count==0 ; y++) { await Task.Delay(50); }
            foreach(var item in hintFrames)
            {
                SolitaireGrid.Children.Remove(item);
            }*/