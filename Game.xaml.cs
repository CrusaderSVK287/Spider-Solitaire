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

        private Random random = new Random();   //used to call Random class methods
        private List<Card> Selected = new List<Card>(); //currenly selected card/s
        int Selected_x, NewCardNumber = 1; //indexes of the currently selected card (and cards underneeth)
        int DecksSolved = 0;   //number of solved decks

        private Deck deck = new Deck();
        public Game(int numberOfColours)
        {
            InitializeComponent();
            deck.GenerateCards(numberOfColours);
            deck.LayOutStartingCardsRecursive(cardOffset,SolitaireGrid,CardSelect);
        }


        


        //Loads all cards that are being selected into a tmp list "Selected"
        private void CardSelect(object sender, MouseEventArgs e)
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
                for (int i = y; i < deck.activeCards[x].Count; i++)
                {
                    _ = deck.activeCards[x][i].SelectedMove(i + 1, cardOffset); //+1 due to y being an indexer
                    Selected.Add(deck.activeCards[x][i]);
                }
                deck.activeCards[x].RemoveRange(y, deck.activeCards[x].Count-y);
                Selected_x = x;
            }
            else
            {
                _ = deck.activeCards[x][y].InvalidMove(y + 1, cardOffset);  //+1 due to y being an indexer
            }
        }

        private void ColumnClick(object sender, MouseButtonEventArgs e)
        {
            if (Selected == null || Selected.Count == 0) return;
            int column_index = ((Grid)sender).Name[3] - '0';
            if((deck.activeCards[column_index].Count == 0 || deck.activeCards[column_index].Last().Value-1 == Selected[0].Value) 
                && column_index != Selected_x)
            {
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
            isDeckSolved();
            Refresh();
            if (DecksSolved == 8) MessageBox.Show("Victory");
        }

        private void isDeckSolved()
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

        public void NewCardsClick(object sender, MouseButtonEventArgs e)
        {
            if (NewCardNumber > 5) return;
            for (int i = 0; i < 10; i++)
            {
                if (deck.activeCards[i].Count > 0) continue;
                MessageBox.Show("You cannot add new card to an empty column");
                return;
            }
            for(int index = 0; index < 10; index++)
            {
                Card card = new Card(deck.values[deck.cardNum], (char)deck.colors[deck.cardNum],true,
                    deck.activeCards[index].Count + 1, index, cardOffset, CardSelect);
                SolitaireGrid.Children.Add(card.Image);
                Grid.SetColumn(card.Image, index + 1);
                deck.cardNum++;
                deck.activeCards[index].Add(card);
            }
            Image[] newCardImages = { new1, new2, new3, new4, new5 };
            newCardImages[NewCardNumber-1].Visibility = Visibility.Hidden;
            NewCardNumber++;
            Refresh();
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
    }
}
