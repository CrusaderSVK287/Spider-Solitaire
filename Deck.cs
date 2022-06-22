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

namespace Spider_Solitaire
{
    internal class Deck
    {
        public int[] values = new int[8 * 13];  //8 total columns with 13 cards each
        public int[] colors = new int[8 * 13];
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
            bool picked;
            int[] coloursPool = new int[numberOfCoulours * 13];
            for (int i = 0; i < numberOfCoulours * 13; i++)
            {
                coloursPool[i] = 8 / numberOfCoulours;
            }
            for (int i = 0; i < 8 * 13; i++)
            {
                picked = false;
                while (!picked)
                {
                    int rng = random.Next(coloursPool.Length);
                    if (coloursPool[rng] > 0)
                    {
                        if (rng >= 3 * 13) colors[i] = 'b';    //y == 1 && 1 == color
                        else if (rng >= 2 * 13) colors[i] = 'a';
                        else if (rng >= 1 * 13) colors[i] = 'b';
                        else colors[i] = 'c';
                        values[i] = rng % 13 + 1;  //y==0 && 0 == value
                        coloursPool[rng]--;
                        picked = true;
                    }
                }
            }
        }

        // Method to Lay out the cards that the game starts with onto the game field
        public void LayOutStartingCardsRecursive(int cardOffset, Grid SolitaireGrid, MouseButtonEventHandler CardSelect)
        {
            int index = cardNum % 10;
            Card card = new Card(values[cardNum], (char)colors[cardNum], (cardNum <= 43) ? false : true, 
                activeCards[index].Count+1,index, cardOffset, CardSelect);
            if (card == null) return;
            activeCards[index].Add(card);
            SolitaireGrid.Children.Add(card.Image);
            Grid.SetColumn(card.Image, index + 1);
            cardNum++;
            if (cardNum < 54) LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect);
        }
    }
}
