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

namespace Spider_Solitaire
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        const int cardOffset = 20;  //used to render the cards apart from each other

        private int[,] cardDeck = new int[2,8*13]; //8 total columns with 13 cards each
        private int cardNum = 0; //used to iterate throgh the deck while handing out new cards
        List<Card>[] Deck = new List<Card>[10]; // Array of lists containing each column from top to bottom
        private Random random = new Random();   //used to call Random class methods
        private List<Card> Selected = new List<Card>(); //currenly selected card/s
        int Selected_x, Selected_y, NewCardNumber = 1; //indexes of the currently selected card (and cards underneeth)
        int DecksSolved = 0;   //number of solved decks
        public Game(int numberOfColours)
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                Deck[i] = new List<Card>();
            }
            GenerateCards(numberOfColours);
            LayOutStartingCardsRecursive();
        }

        //randomly generates the card deck from with the cards are given out in order
        private void GenerateCards(in int numberOfCoulours)
        {
            bool picked;
            int[] colours = new int[numberOfCoulours*13];
            for (int i = 0; i < numberOfCoulours*13; i++)
            {
                colours[i] = 8/numberOfCoulours;
            }
            for (int i = 0;i < 8*13 ;i++)
            {
                picked = false;
                while(!picked)
                {
                    int rng = random.Next(colours.Length);
                    if (colours[rng] > 0)
                    {
                        if(rng>=3*13) cardDeck[1,i] = 'b';
                        else if(rng>=2*13) cardDeck[1,i] = 'a';
                        else if(rng>=1*13) cardDeck[1,i] = 'b';
                        else cardDeck[1,i] = 'c';
                        cardDeck[0, i] = rng%13+1;
                        colours[rng]--;
                        picked = true;
                    }
                }
            }
        }
        
        // Method to Lay out the cards that the game starts with onto the game field
        private void LayOutStartingCardsRecursive()
        {
            Card card = new Card(cardDeck[0,cardNum],(char)cardDeck[1,cardNum],(cardNum<=43)?false:true);
            if (card == null) return;
            int index = cardNum % 10;
            Deck[index].Add(card);
            card.Image.Width = 89;
            card.Image.Height = 120;
            card.Image.Source = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/" + (card.Visible ? $"{card.Value}{card.Colour}" : "uncovered") + ".png"));
            card.Image.HorizontalAlignment = HorizontalAlignment.Center;
            card.Image.VerticalAlignment = VerticalAlignment.Top;
            card.Image.Visibility = Visibility.Visible;
            card.Image.Margin = new Thickness(0,Deck[index].Count * cardOffset + 5,0,0);
            card.Image.MouseLeftButtonUp += new MouseButtonEventHandler(CardSelect);
            string name = "";   //first letter, lowercase, indicates which column index wise the image is at, and the second one, uppercase
                                //indicates on which position y wise the image (card) is
            name += (char)(index + 97);
            name += (char)(Deck[index].Count + 64);
            card.Image.Name = name;
            SolitaireGrid.Children.Add(card.Image);
            Grid.SetColumn(card.Image, index+1);
            cardNum++;
            if (cardNum < 54) LayOutStartingCardsRecursive();
        }

        //Loads all cards that are being selected into a tmp list "Selected"
        private void CardSelect(object sender, MouseEventArgs e)
        {
            if (Deck == null || Selected.Count > 0) return;
            int x = ((Image)sender).Name[0] - 97;
            int y = ((Image)sender).Name[1] - 65;
            bool valid = Deck[x][y].Visible;
            for (int i = y+1, tmp=1; i < Deck[x].Count && valid; i++, tmp++)
            {
                if (Deck[x][y].Colour != Deck[x][i].Colour || Deck[x][y].Value != Deck[x][i].Value + tmp) valid = false;
            }
            if(valid)
            {
                for (int i = y; i < Deck[x].Count; i++)
                {
                    Deck[x][i].SelectedMove(i + 1); //+1 due to y being an indexer
                    Selected.Add(Deck[x][i]);
                }
                Deck[x].RemoveRange(y, Deck[x].Count-y);
                Selected_y = y;
                Selected_x = x;
            }
            else
            {
                Deck[x][y].InvalidMove(y+1);  //+1 due to y being an indexer
            }
        }

        private void ColumnClick(object sender, MouseButtonEventArgs e)
        {
            if (Selected == null || Selected.Count == 0) return;
            int column_index = ((Grid)sender).Name[3] - '0';
            if((Deck[column_index].Count == 0 || Deck[column_index].Last().Value-1 == Selected[0].Value) && column_index != Selected_x)
            {
                Deck[column_index].AddRange(Selected);
                foreach(var item in Deck[column_index])
                {
                    string name = "";
                    name += (char)(column_index + 97);
                    name += (char)(Deck[column_index].IndexOf(item) + 65);
                    item.Image.Name = name;
                }
                foreach(var item in Deck[column_index])
                {
                    Grid.SetColumn(item.Image, column_index + 1);
                    item.Image.Margin = new Thickness(0, (Deck[column_index].IndexOf(item)+1) * cardOffset + 5,0,0);
                }
            }
            else
            {
                Deck[Selected_x].AddRange(Selected);
                foreach(var item in Deck[Selected_x])
                {
                    item.Image.Margin = new Thickness(0, (Deck[Selected_x].IndexOf(item)+1) * cardOffset + 5, 0, 0);
                }
            }
            Selected.Clear();
            isDeckSolved();
            Refresh();
            if (DecksSolved == 8) MessageBox.Show("Victory");
        }

        private void isDeckSolved()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (var item in Deck[i])
                {
                    if (item.Visible == false || item.Value != 13) continue;
 
                    int index = Deck[i].IndexOf(item);
                    if (index + 13 != Deck[i].Count) continue;
                    for (int j = index+1, value = 12; j < Deck[i].Count; j++, value--)
                    {
                        if (Deck[i][j].Value != value || Deck[i][j].Colour != item.Colour) goto EndOfForeachLoop;
                    }
                    for (int j = index; j < Deck[i].Count; j++)
                    {
                        Deck[i][j].Image.Visibility = Visibility.Hidden;
                    }

                    Image Image = new Image();
                    Image.Width = 89;
                    Image.Height = 120;
                    Image.Source = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/" + $"13{item.Colour}" + ".png"));
                    Image.HorizontalAlignment = HorizontalAlignment.Left;
                    Image.VerticalAlignment = VerticalAlignment.Top;
                    Image.Visibility = Visibility.Visible;
                    Image.Margin = new Thickness(-220 + DecksSolved*15, 0, 0, 0);
                    SolitaireGrid.Children.Add(Image);
                    Grid.SetRow(Image, 1);
                    Grid.SetColumn(Image, 3);

                    Deck[i].RemoveRange(index, 13); //13 cards in full set
                    DecksSolved++;
                    return;
                }
            EndOfForeachLoop:;
            }
        }

        private void NewCardsClick(object sender, MouseButtonEventArgs e)
        {
            if (NewCardNumber > 5) return;
            for (int i = 0; i < 10; i++)
            {
                if (Deck[i].Count > 0) continue;
                MessageBox.Show("You cannot add new card to an empty column");
                return;
            }
            for(int index = 0; index < 10; index++)
            {
                Card card = new Card(cardDeck[0, cardNum], (char)cardDeck[1, cardNum],true);
                card.Image.Width = 89;
                card.Image.Height = 120;
                card.Image.Source = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "assets/" + $"{card.Value}{card.Colour}" + ".png"));
                card.Image.HorizontalAlignment = HorizontalAlignment.Center;
                card.Image.VerticalAlignment = VerticalAlignment.Top;
                card.Image.Visibility = Visibility.Visible;
                card.Image.Margin = new Thickness(0, (Deck[index].Count+1) * cardOffset + 5, 0, 0);
                card.Image.MouseLeftButtonUp += new MouseButtonEventHandler(CardSelect);
                string name = "";
                name += (char)(index + 97);
                name += (char)(Deck[index].Count + 65);
                card.Image.Name = name;
                SolitaireGrid.Children.Add(card.Image);
                Grid.SetColumn(card.Image, index + 1);
                cardNum++;
                Deck[index].Add(card);
            }
            switch (NewCardNumber)
            {
                case 1:
                    new1.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    new2.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    new3.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    new4.Visibility = Visibility.Hidden;
                    break;
                case 5:
                    new5.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
            NewCardNumber++;
            Refresh();
        }

        //makes sure that all cards are up and the correct ones are being shown
        private void Refresh()
        {
            if (Deck == null) return;
            for (int i = 0; i < 10; i++)
            {
                foreach(var item in Deck[i])
                {
                    SolitaireGrid.Children.Remove(item.Image);
                    SolitaireGrid.Children.Add(item.Image);
                }
                if (Deck[i].Count > 0 && Deck[i].Last().Visible == false)
                {
                    Deck[i].Last().Visible = true;
                    Deck[i].Last().GetColour();
                }
            }
        }
        private class Card
        {
            public readonly int Value;
            public readonly char Colour;
            public bool Visible;
            public Image Image;
            public Card(int value, char colour, bool visible)
            {
                Value = value;
                Colour = colour;
                Visible = visible;
                Image = new Image();
            }

            //Moves the card up
            public async Task SelectedMove(int y)
            {
                for (int i = 0; i < 15; i++)
                {
                    Image.Margin = new Thickness(0, y * cardOffset + 5 - i, 0, 0);
                    await Task.Delay(10);
                }
                //Image.Margin = new Thickness(0, y * 20 + 5, 0, 0);
            }

            //Moves the card right, left and back
            public async Task InvalidMove(int y)
            {
                for (int i = 0; i <= 20; i+=2)
                {
                    Image.Margin = new Thickness(i, y * cardOffset + 5, 0, 0);
                    await Task.Delay(2);
                }
                for (int i = 20; i >= -20; i-=2)
                {
                    Image.Margin = new Thickness(i, y * cardOffset + 5, 0, 0);
                    await Task.Delay(2);
                }
                for (int i = -20; i != 0; i+=2)
                {
                    Image.Margin = new Thickness(i, y * cardOffset + 5, 0, 0);
                    await Task.Delay(2);
                }
            }

            public void GetColour()
            {
                Image.Source = new BitmapImage(new Uri(@"assets/" + (Visible ? $"{Value}{Colour}" : "uncovered") + ".png", UriKind.Relative));
            }
        }
    }
}
