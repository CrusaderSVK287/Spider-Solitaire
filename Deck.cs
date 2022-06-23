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
        public async Task LayOutStartingCardsRecursive(int cardOffset, Grid SolitaireGrid, MouseButtonEventHandler CardSelect)
        {
            int index = cardNum % 10;
            Card card = new Card(values[cardNum], (char)colors[cardNum], (cardNum <= 43) ? false : true, 
                activeCards[index].Count+1,index, cardOffset, CardSelect);
            if (card == null) return;
            activeCards[index].Add(card);
            SolitaireGrid.Children.Add(card.Image);
            Grid.SetColumn(card.Image, index + 1);
            await Task.Delay(10);
            cardNum++;
            if (cardNum < 54) await LayOutStartingCardsRecursive(cardOffset, SolitaireGrid, CardSelect);
        }
    }
}
