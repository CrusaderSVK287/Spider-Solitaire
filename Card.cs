using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Spider_Solitaire
{
    internal class Card
    {
        public readonly int Value;
        public readonly char Colour;
        public bool Visible;
        public Image Image;
        public Card(int value, char colour, bool visible, int y, int x, int offset, MouseButtonEventHandler CardSelect)
        {
            Value = value;
            Colour = colour;
            Visible = visible;
            Image = new Image();
            CreateImage(y,x,offset,CardSelect);
        }

        //inicialised image properties
        private void CreateImage(int y, int x, int offset, MouseButtonEventHandler CardSelect)
        {
            GetColour();
            Image.Width = 89;
            Image.Height = 120;
            Image.HorizontalAlignment = HorizontalAlignment.Center;
            Image.VerticalAlignment = VerticalAlignment.Top;
            Image.Visibility = Visibility.Visible;
            Image.Margin = new Thickness(0, y * offset + 5, 0, 0);   //y == activeCards[index].Count
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(CardSelect);
            string name = "";   //first letter, lowercase, indicates which column index wise the image is at, and the second one, uppercase
                                //indicates on which position y wise the image (card) is
            name += (char)(x + 97); //index == x
            name += (char)(y + 64);
            Image.Name = name;
        }

        //Moves the card up
        public async Task SelectedMove(int y, int cardOffset)
        {
            for (int i = 0; i < 15; i+=2)
            {
                Image.Margin = new Thickness(0, y * cardOffset + 5 - i, 0, 0);
                await Task.Delay(1);
            }
        }

        //Moves the card right, left and back
        public async Task InvalidMove(int y, int cardOffset)
        {
            for (int i = 0; i <= 20; i += 2)
            {
                Image.Margin = new Thickness(i, y * cardOffset + 5, 0, 0);
                await Task.Delay(2);
            }
            for (int i = 20; i >= -20; i -= 2)
            {
                Image.Margin = new Thickness(i, y * cardOffset + 5, 0, 0);
                await Task.Delay(2);
            }
            for (int i = -20; i != 0; i += 2)
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
