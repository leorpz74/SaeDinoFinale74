using Dino_Saé;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace MyGame
{
    public partial class MenuPrincipale : Window
    {
        private DinoGame _dinoGame; // Référence à la fenêtre DinoGame

        // Constructeur qui reçoit une référence à DinoGame
        public MenuPrincipale(DinoGame dinoGame)
        {
            InitializeComponent();
            _dinoGame = dinoGame;
        }

        private void JouerDinoGame_Click(object sender, RoutedEventArgs e)
        {
            // Réaffiche la fenêtre DinoGame
            _dinoGame.Show();

            // Ferme la fenêtre MenuPrincipale
            this.Close();
        }

        private void Quitter_Click(object sender, RoutedEventArgs e)
        {
            // Ferme complètement l'application
            Application.Current.Shutdown();
        }
        private void Optionregle_Click(object sender, RoutedEventArgs e)
        {
            Optionsregle optionsWindow = new Optionsregle(this); // Passe une référence à MenuPrincipale
            optionsWindow.Show();
            this.Hide(); // Cache le menu principal
        }
    }

}