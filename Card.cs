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
        public bool Visible { get; set; }
        public Image Image;
        private bool AnimationPlaying { get; set; } = false;
        public Card(int value, char colour, bool visible, int y, int x, int offset, MouseButtonEventHandler CardSelect, float Scale)
        {
            Value = value;
            Colour = colour;
            Visible = visible;
            Image = new Image();
            CreateImage(y,x,offset,CardSelect, Scale);
        }

        public Card(int value, char colour, bool visible)
        {
            Value = value;
            Colour = colour;
            Visible = visible;
            Image = new Image();
        }

        //inicialised image properties
        private void CreateImage(int y, int x, int offset, MouseButtonEventHandler CardSelect, float Scale)
        {
            GetColour();
            Image.Width = Convert.ToInt32(89.0f*Scale);
            Image.Height = Convert.ToInt32(120.0f*Scale);
            Image.HorizontalAlignment = HorizontalAlignment.Center;
            Image.VerticalAlignment = VerticalAlignment.Top;
            Image.Visibility = Visibility.Visible;
            Image.Margin = new Thickness(0, y * offset + 5, 0, 0);   //y == activeCards[index].Count
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(CardSelect);
            string name = "";   //first letter, lowercase, indicates which column index wise the image is at, and the second one, uppercase
                                //indicates on which position y wise the image (card) is
            name += (char)(x + 97); //index == x

            char a = (char)(y + 64);
            char b = 'A';
            while(a > 'Z')
            {
                a -= (char)26;
                b++;
            }
            name+=$"{a}{b}";

            Image.Name = name;
        }

        //Moves the card up
        public async Task SelectedMove(int y, int cardOffset, bool playAnimation)
        {
            if (AnimationPlaying == true) return;
            AnimationPlaying = true;
            for (int i = 0; i < 15; i+=2)
            {
                Image.Margin = new Thickness(0, y * cardOffset + 5 - i, 0, 0);
                if(playAnimation) await Task.Delay(1);
            }
            AnimationPlaying = false;
        }

        //Moves the card right, left and back
        public async Task InvalidMove(int y, int cardOffset)
        {
            if (AnimationPlaying == true) return;
            AnimationPlaying = true;
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
            AnimationPlaying = false;
        }

        public void GetColour()
        {
            Image.Source = new BitmapImage(new Uri(@"assets/" + (Visible ? $"{Value}{Colour}" : "uncovered") + ".png", UriKind.Relative));
        }

        public static void RemoveEventHandlers(Card card, MouseButtonEventHandler handler)
        {
            card.Image.MouseLeftButtonUp -= handler;
        }
    }
}
