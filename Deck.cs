using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace Spider_Solitaire
{
    internal class Deck
    {
        public int[] values = new int[8 * 13];  //8 total columns with 13 cards each
        public char[] colors = new char[8 * 13];
        public int cardNum = 0; //used to iterate throgh the deck while handing out new cards

        public List<Card>[] activeCards = new List<Card>[10];   //array of lists containing currently held out cards

        private Random random = new Random();

        public Deck()
        {
            for(int i = 0; i < 10; i++) activeCards[i] = new List<Card>();
            cardNum = 0;
        }

        //randomly generates the card deck from with the cards are given out in order
        public void GenerateCards(in int numberOfCoulours)
        {
            if (File.Exists(@"autosave.soli")) File.Delete(@"autosave.soli");
            bool picked;
            int[] coloursPool = new int[numberOfCoulours * 13];
            for (int i = 0; i < numberOfCoulours * 13; i++) coloursPool[i] = 8 / numberOfCoulours;

            for (int i = 0; i < 8 * 13; i++)
            {
                picked = false;
                while (!picked)
                {
                    int rng = random.Next(coloursPool.Length);
                    if (coloursPool[rng] > 0)
                    {
                        if (rng >= 3 * 13) colors[i] = 'b';
                        else if (rng >= 2 * 13) colors[i] = 'a';
                        else if (rng >= 1 * 13) colors[i] = 'd';
                        else colors[i] = 'c';
                        values[i] = rng % 13 + 1;
                        coloursPool[rng]--;
                        picked = true;
                    }
                }

                try { File.AppendAllText(@"autosave.soli", $"{Convert.ToChar(values[i] + 96)}{colors[i]}\n"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            try { File.AppendAllText(@"autosave.soli", "-null-\n"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            if (!IsValidDeck()) GenerateCards(numberOfCoulours);
        }

        //method determines whether the generated deck is valid according to a generation ruleset
        private bool IsValidDeck()
        {
            //3 or more of the same card beside each other
            for(int i = 0; i < (8*13)-2; i++)
            {
                if (colors[i] == colors[i + 1] && colors[i] == colors[i + 2] &&
                   values[i] == values[i + 1] && values[i] == values[i + 2]) return false;
            }

            //4 or more same cards in the same row (starting from the first row, fifth card)
            for (int i = 4; i < 8 * 13; i+=10)  //jumps from line to line
            {
                for (int j = i; j < i+7; j++)  //itterates thrugh current line 
                {
                    int count = 1;
                    for (int k = j+1; k < i+10; k++)    //itterates through the rest of the line
                    {
                        if (colors[k] == colors[j] && values[k] == values[j]) count++;
                    }
                    if (count > 3) return false;
                }
            }

            //6 or more of the same values of cards in the same row (starting from the first row, fifth card)
            for (int i = 4; i < 8 * 13; i += 10)  //jumps from line to line
            {
                for (int j = i; j < i + 5; j++)  //itterates thrugh current line 
                {
                    int count = 1;
                    for (int k = j + 1; k < i + 10; k++)    //itterates through the rest of the line
                    {
                        if (values[k] == values[j]) count++;
                    }
                    if (count > 5) return false;
                }
            }

            //max 3 kings allowed after 80th card
            for (int i = 76, count = 0; i < 8*13; i++)
            {
                if(values[i]==13) count++;
                if(count > 3) return false;
            }
            return true;
        }

        // Method to Lay out the cards that the game starts with onto the game field
        public async Task LayOutStartingCardsRecursive(int cardOffset, Grid SolitaireGrid, MouseButtonEventHandler CardSelect, bool Loading)
        {
            int index = cardNum % 10;
            Card card = new (values[cardNum], colors[cardNum], (cardNum <= 43) ? /*false*/false : true, 
                activeCards[index].Count+1,index, cardOffset, CardSelect);
            if (card == null) return;
            activeCards[index].Add(card);
            SolitaireGrid.Children.Add(card.Image);
            Grid.SetColumn(card.Image, index + 1);
            if(!Loading)await Task.Delay(10);
            cardNum++;
            if (cardNum < 54) await LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect, Loading);
        }

        //loads the currently saved deck, if there is any
        public void LoadDeck()
        {
            try
            {
                int line = 1;
                foreach (var item in File.ReadAllLines(@"autosave.soli"))
                {
                    if (line != 105 && item.Length != 2) throw new FileFormatException();
                    if (line == 105 && item.Length != 6 && !item.Contains("-null-\n")) throw new FileFormatException();
                    if (line == 105) break;

                    values[line - 1] = Convert.ToInt32(item[0])-96;
                    colors[line - 1] = item[1];

                    line++;
                }
                if (line < 105) throw new FileFormatException();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Error", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
        }

        public static void LoadCommands(Action<object, MouseButtonEventArgs> CardSelect,
                                 Action<object,MouseButtonEventArgs> ColumnClick,
                                 Action<object,MouseButtonEventArgs> NewCardsClick)
        {
            try
            {
                int line = 0;
                foreach (var item in File.ReadAllLines(@"autosave.soli"))
                {
                    if (++line <= 105) continue;
                    switch (item[0])
                    {
                        case 'S': Command.ExecuteSelect(CardSelect, new string[] { $"{item[1]}{item[2]}" });
                            break;
                        case 'M': Command.ExecuteMove(ColumnClick, new string[] { $"col{item[1]}" });
                            break;
                        case 'A': Command.ExecuteAdd(NewCardsClick);
                            break;
                        default:
                            throw new FileFormatException();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
