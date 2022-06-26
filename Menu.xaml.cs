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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(Convert.ToInt32(numOfColours.Text),true,this);
            NavigationService.Navigate(game);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt16(numOfColours.Text) >= 4) return;
            numOfColours.Text = (Convert.ToInt16(numOfColours.Text) * 2).ToString();
        }

        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToInt16(numOfColours.Text) <= 1) return;
            numOfColours.Text = (Convert.ToInt16(numOfColours.Text) / 2).ToString();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game(Convert.ToInt32(numOfColours.Text), false, this);
            NavigationService.Navigate(game);
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            try { throw new NotImplementedException();}
            catch (Exception ex) { MessageBox.Show(ex.ToString(),"Warning",MessageBoxButton.OK,MessageBoxImage.Warning);}
        }
    }
}
