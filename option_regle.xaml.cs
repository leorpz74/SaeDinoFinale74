using Dino_Saé;
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
using System.Windows.Shapes;

namespace MyGame
{
    public partial class Optionsregle : Window
    {
        private MenuPrincipale _menuPrincipale;

        public Optionsregle(MenuPrincipale menuPrincipale)
        {
            InitializeComponent();
            _menuPrincipale = menuPrincipale; // Stocke la référence vers MenuPrincipale
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                _menuPrincipale.Show(); // Réaffiche le menu principal
                this.Close();           // Ferme la fenêtre OptionsRules
            }
        }


        /*private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DinoGame.musique != null)
            {
                DinoGame.musique.Volume = VolumeSlider.Value;
            }

        }*/
    }
}