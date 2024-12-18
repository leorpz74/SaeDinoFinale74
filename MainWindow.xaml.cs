
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
    public partial class DinoGame : Window
    {
  
        public static BitmapImage PERSONNAGE_DROIT;
        public static BitmapImage PERSONNAGE_GAUCHE;
        public static int VITESSE_PERSO = 15;
        public static int VITESSE_BALLE = 18;
        public static int VITESSE_DINO = 7;
        public static int VITESSE_DINO_VOLANT = 4;
        public static double DELAIS_BALLE = 0.5;
        public DispatcherTimer MINUTERIE;
        public DispatcherTimer MINUTERIEDINO;
        public DispatcherTimer MINUTERIESAUT;
        public DispatcherTimer TPSSCORE;
        public DispatcherTimer MINUTERIETIR;
        public static List<Image> LES_BALLES = new List<Image>();
        public static List<Image> LES_DINOS_TERRE = new List<Image>();
        public static List<Image> LES_DINOS_VOLANT = new List<Image>();
        public static bool BALLE_DROITE = false;
        public static bool BALLE_GAUCHE= false;
        public static bool TIR = false, DROIT, GAUCHE;
        public int APPARITION_TERRE;
        public int APPARITION_VOLANT;
        public static int BORD_DROIT_CANVAS = 1095;
        public static int BORD_GAUCHE_CANVAS = 10;
        public static int LARGEUR_BALLE = 25;
        public static int HAUTEUR_BALLE = 25;
        public static int LARGEUR_DINO_PETIT = 100;
        public static int HAUTEUR_DINO_PETIT = 140;
        public static int LARGEUR_DINO_VOLANT = 90;
        public static int HAUTEUR_DINO_VOLANT = 80;
        public static int Y_POSITION_PERSO = 270;
        public static double Y_POSITION_DINO_VOLANT;
        public static int ALIGNEMENT_Y = 20;
        public static int LIMITE_GAUCHE_CANVAS = 10;
        public static int LIMITE_DROITE_CANVAS = 1100;
        public static int POSITION_APPARITION_DINO_VOLANT = -125;
        public static int LIMITE_DINO_VOLANT = 300;
        public bool TIRE_POSSIBLE = true;
        public static int GRAVITE = 3;
        public static double HAUTEUR_SAUT = 0;
        public static double VITESSE_SAUT = -38;
        public static double HAUTEUR_SOL = 270;
        public double SCORE = 0;
        private static SoundPlayer TIR_BALLE, PERDU, SAUT;
        public static MediaPlayer MUSIQUE;


        public DinoGame()
        {
            InitializeComponent();
            MenuPrincipale menu = new MenuPrincipale(this);
            menu.Show();
            this.Hide();
        }



        public void Jeu(object? sender, EventArgs e)
        {
            //prendre la position du perso 
            double newPositionPN = Canvas.GetLeft(imgPersoDroite);

            //Pour aller à gauche
            if (GAUCHE && !DROIT)
            {
                newPositionPN -= VITESSE_PERSO;
            }
            //Pour aller à Droite
            else if (!GAUCHE && DROIT)
            {
                newPositionPN += VITESSE_PERSO;
            }


            if (newPositionPN <= fond_canvas.ActualWidth - imgPersoDroite.Width && newPositionPN >= 0)
            {
                Canvas.SetLeft(imgPersoDroite, newPositionPN);
            }
            //Pour déplacer les balles
            if (TIR)
            {
                DeplacerBalles();
            }
             
            //methode de verification de colisions
            VerifierCollisions();


            if (Canvas.GetTop(imgPersoDroite) < 270)
            {
                Canvas.SetTop(imgPersoDroite, Canvas.GetTop(imgPersoDroite) + GRAVITE);
            }

        }
        private void Jouer()//Initialise les élement et minuterie du jeu 
        {
            InitPerso();
            InitTimer();
            InitialiserDeplacementDinoTerre();
            InitialiserDeplacementDinoVolant();
            InitTimerTir();
            InitDinoTerre();
            InitDinoVolant();
            InitTimerSaut();
            InitScore();
            InitSon();
            InitMusique();
        }
        public void GameOver() //Méthode qui permet de quitter quand on a perdu
        {
            PERDU.Play();
            MessageBox.Show("Vous avez perdu ! Le dinosaure vous a touché !");
            Environment.Exit(0);
        }

        public void MajScore(object sender, EventArgs e) // méthode Pour mettre à jour le Score toute les secondes
        {
            TPSSCORE.Start();
            SCORE = SCORE + 50;
            this.lblScore.Content = "Score : " + SCORE;

        }
        private void jeu_dino_KeyDown(object sender, KeyEventArgs e)//méthodes qui permet le fonctionnent des différentes touches
        {
            //Pour quiter la page et revenir sur la page du menu principale 
            if (e.Key == Key.E)
            {
                MenuPrincipale mainGameWindow = new MenuPrincipale(this);
                mainGameWindow.Show();
                this.Close();
            
            }

            //Pour ce déplacer vers la droite
            if (e.Key == Key.D)
            {
                DROIT = true;
                imgPersoDroite.Source = PERSONNAGE_DROIT;
                BALLE_DROITE = true;
                BALLE_GAUCHE = false;
            }

            //Pour ce déplacer a gauche
            else if (e.Key == Key.Q)
            {

                GAUCHE = true;
                imgPersoDroite.Source = PERSONNAGE_GAUCHE;
                BALLE_DROITE = false;
                BALLE_GAUCHE = true;
            }

            // Espace pour sauter 
            if (e.Key == Key.Space && HAUTEUR_SAUT == 0)
            {
                SAUT.Play();
                HAUTEUR_SAUT = VITESSE_SAUT;
                MINUTERIESAUT.Start();
            }

            //Code triche qui permet d'arreter les minuterie des dinos pour les tuer et les esquiver plus facilement 
            else if (e.Key == Key.T)
            {
                if (MINUTERIE.IsEnabled && MINUTERIEDINO.IsEnabled)
                {
                    MINUTERIE.Stop();
                    MINUTERIEDINO.Stop();
                }
                else
                {
                    MINUTERIE.Start();
                    MINUTERIEDINO.Start();
                }
            }

            // J pour tirer
            if (e.Key == Key.J && TIRE_POSSIBLE)
            {
                TIR = true;
                TIR_BALLE.Play();
                InitBalle();
                TIRE_POSSIBLE = false;
                MINUTERIETIR.Start();
            }

            // P pour déclencher le jeu 
            if (e.Key == Key.P )
            {
                Jouer();
            }
        }


        private void jeu_dino_KeyUp(object sender, KeyEventArgs e)
        {
            //rend le deplacement à droite plus fluide 
            if (e.Key == Key.D)
            {
                DROIT = false;
            }

            //rend le déplacement à gauche plus fluide 
            else if (e.Key == Key.Q)
            {
                GAUCHE = false;
            }
        }


        //Methode qui permet d'initialiser la musique de fond 
        public static void InitMusique()
        {
            if (DinoGame.MUSIQUE != null)
            {
                MUSIQUE = new MediaPlayer();
                MUSIQUE.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "sons/theme_fond_1.wav"));
                MUSIQUE.MediaEnded += RelanceMusique;
                MUSIQUE.Volume = 0.5;
                MUSIQUE.Play();
            }
        }

        //Methode qui permete de relancer la musique quand elle s'arrête 
        public static void RelanceMusique(object? sender, EventArgs e)
        {
            MUSIQUE.Position = TimeSpan.Zero;
            MUSIQUE.Play();
        }

        //Méthode qui permet d'initialiser le perso qui va à droite où à gauche 
        private static void InitPerso()
        {
            PERSONNAGE_DROIT = new BitmapImage(new Uri("/img/personnage_droite.png", UriKind.Relative));
            PERSONNAGE_GAUCHE = new BitmapImage(new Uri("/img/personnage_gauche.png", UriKind.Relative));
        }

        //Initialise le timer du jeu avec les colistions et les balles qui se déplace
        private void InitTimer()
        {
            MINUTERIE = new DispatcherTimer();
            MINUTERIE.Interval = TimeSpan.FromMilliseconds(16);
            MINUTERIE.Tick += Jeu;
            MINUTERIE.Start();

        }
        
        //Méthode qui permet de créer les sons du jeu 
        private void InitSon()
        {
            TIR_BALLE = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/laser_1.wav")).Stream);
            PERDU = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/gameover.wav")).Stream);
            SAUT = new SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/sons/saut.wav")).Stream);
        }


        //Méthode qui permet d'initialiser le score 
        private void InitScore()
        {
            TPSSCORE = new DispatcherTimer();
            TPSSCORE.Interval = TimeSpan.FromSeconds(1);
            TPSSCORE.Tick += MajScore; // fait augelenter le score de 50 toute les secondes
            TPSSCORE.Start();

        }

        //Méthodes qui permet de créer un timer pour le déplecement des dinosaures Terrestre 
        private void InitialiserDeplacementDinoTerre()
        {
            MINUTERIEDINO = new DispatcherTimer();
            MINUTERIEDINO.Interval = TimeSpan.FromMilliseconds(16);
            MINUTERIEDINO.Tick += DeplacementDinoTerre;
            MINUTERIEDINO.Start();
        }

        //Méthodes qui permet de créer un timer pour le déplecement des dinosaures Volants 
        private void InitialiserDeplacementDinoVolant()
        {
            MINUTERIE = new DispatcherTimer();
            MINUTERIE.Interval = TimeSpan.FromMilliseconds(16);
            MINUTERIE.Tick += DeplacementDinoVolant;
            MINUTERIE.Start();
        }


        //Méthodes qui permet définir le délais entre chaque Tir
        private void InitTimerTir()
        {
            MINUTERIETIR = new DispatcherTimer();
            MINUTERIETIR.Interval = TimeSpan.FromSeconds(DELAIS_BALLE);
            MINUTERIETIR.Tick += TimerDelaisBalle;
            MINUTERIETIR.Stop();
        }


        // Methode qui met de stoper la possibilitée de tirer
        private void TimerDelaisBalle(object sender, EventArgs e)
        {
            TIRE_POSSIBLE = true;
            MINUTERIETIR.Stop();
        }


        //Méthode qui permet au saut de fonctioner 
        private void InitTimerSaut()
        {
            MINUTERIESAUT = new DispatcherTimer();
            MINUTERIESAUT.Interval = TimeSpan.FromMilliseconds(16);
            MINUTERIESAUT.Tick += Saut_Tick;
        }


        private void Saut_Tick(object sender, EventArgs e)
        {
            HAUTEUR_SAUT += GRAVITE;

            double hauteurcanvas = Canvas.GetTop(imgPersoDroite);
            if (hauteurcanvas + HAUTEUR_SAUT >= HAUTEUR_SOL)
            {
                HAUTEUR_SAUT = 0;
                Canvas.SetTop(imgPersoDroite, HAUTEUR_SOL);
                MINUTERIESAUT.Stop();
            }
            else
            {
                Canvas.SetTop(imgPersoDroite, hauteurcanvas + HAUTEUR_SAUT);
            }
        }


        //méthode qui permet de créer la balle 
        private void InitBalle()
        {
            Image balle = new Image();
            balle.Width = LARGEUR_BALLE;
            balle.Height = HAUTEUR_BALLE;

            if (TIR == true && imgPersoDroite.Source == PERSONNAGE_DROIT) // charge la balle du cotée droit l'ajoute à la liste 
            {
                BALLE_DROITE = false;
                BitmapImage imgballe = new BitmapImage(new Uri("pack://application:,,,/img/laser_petitDroit.png"));
                balle.Source = imgballe;
                fond_canvas.Children.Add(balle);
                Canvas.SetLeft(balle, Canvas.GetLeft(imgPersoDroite) + imgPersoDroite.Width / 2 + 15);
                Canvas.SetTop(balle, Canvas.GetTop(imgPersoDroite) + 17);
                LES_BALLES.Insert(0, balle);

            }
            else if (TIR == true && imgPersoDroite.Source == PERSONNAGE_GAUCHE)// charge la balle du coté gauche et l'ajoute à la liste 
            {
                BALLE_GAUCHE = false;
                BitmapImage imgballe = new BitmapImage(new Uri("pack://application:,,,/img/laser_petitGauche.png"));
                balle.Source = imgballe;
                fond_canvas.Children.Add(balle);
                Canvas.SetLeft(balle, Canvas.GetLeft(imgPersoDroite) + imgPersoDroite.Width / 2 - 45);
                Canvas.SetTop(balle, Canvas.GetTop(imgPersoDroite) + 17);
                LES_BALLES.Insert(0, balle);

            }
        }
        public void DeplacerBalles() // méthode qui permet de déplacer la balle 
        {

            for (int i = 0; i < LES_BALLES.Count; i++)
            {
                Image balle = LES_BALLES[i];
                double PositionGauche = Canvas.GetLeft(balle);

                //la balle va à droite 
                if (balle.Source.ToString() == "pack://application:,,,/img/laser_petitDroit.png")
                {
                    Canvas.SetLeft(balle, PositionGauche + VITESSE_BALLE);
                }

                //la balle va à gauche 
                else
                {
                    Canvas.SetLeft(balle, PositionGauche - VITESSE_BALLE);
                }

                //si la balle sort du canvas elle est suprimée
                if (PositionGauche > LIMITE_DROITE_CANVAS || PositionGauche < LIMITE_GAUCHE_CANVAS)
                {
                    Console.WriteLine(PositionGauche);
                    Console.WriteLine("rien");
                    fond_canvas.Children.Remove(balle);
                    LES_BALLES.RemoveAt(i);

                }
            }
        }


        //Méthode qui permet l'apparition du dino 
        public void InitDinoTerre()
        {

            // Variable aléatoire qui permet de définir si le dino apparait à droite ou à gauche du canvas
            Random Apparition = new Random();
            APPARITION_TERRE = Apparition.Next(1, 3);


            // Défini la taille et la possition y du Dino
            Image dino = new Image();
            dino.Width = LARGEUR_DINO_PETIT;
            dino.Height = HAUTEUR_DINO_PETIT;
            Canvas.SetTop(dino, 240);

            if (APPARITION_TERRE == 1)// Le dino apparait à droite est il est ajouté à la liste 
            {
                BitmapImage dinoterre = new BitmapImage(new Uri("pack://application:,,,/img/dino_petit_gauche.png"));
                dino.Source = dinoterre;
                fond_canvas.Children.Add(dino);
                Canvas.SetLeft(dino, BORD_DROIT_CANVAS);
                LES_DINOS_TERRE.Add(dino);

            }
            if (APPARITION_TERRE == 2)// Le dino apparait à gauche  est il est ajouté à la liste 
            {
                BitmapImage dinoterre = new BitmapImage(new Uri("pack://application:,,,/img/dino_petit_droite.png"));
                dino.Source = dinoterre;
                fond_canvas.Children.Add(dino);
                Canvas.SetLeft(dino, BORD_GAUCHE_CANVAS);
                LES_DINOS_TERRE.Add(dino);
            }
        }

        public void DeplacementDinoTerre(object sender, EventArgs e)
        {

            if (LES_DINOS_TERRE.Count > 0)
            {
                Image dino = LES_DINOS_TERRE[0];
                double positionDino = Canvas.GetLeft(dino);

                if (APPARITION_TERRE == 1)// si le dino apparait à droite  il se déplace vers la gauche  
                {
                    Canvas.SetLeft(dino, positionDino - VITESSE_DINO);
                }

                else if (APPARITION_TERRE == 2)// si le dino apparait à gauche il se déplace vers la droite 
                {
                    Canvas.SetLeft(dino, positionDino + VITESSE_DINO);
                }


                // Quand le dino sort du canvas il est suprimer de la liste 
                if (positionDino > LIMITE_DROITE_CANVAS || positionDino < LIMITE_GAUCHE_CANVAS)
                {
                    Console.WriteLine(positionDino);
                    Console.WriteLine("rien");
                    fond_canvas.Children.Remove(dino);
                    LES_DINOS_TERRE.RemoveAt(0);
                    SCORE = SCORE + 1000;
                    InitDinoTerre();

                }
            }

        }



        //Methode d'apparition des dinos volants
        public void InitDinoVolant()
        {
            // variable aléatoire qui fait apparitre le dino dans le canvas 
            Random Apparition = new Random();
            APPARITION_VOLANT = Apparition.Next(200, 1100);

            //Défini la taille et la position Y du dino puis il l'ajoute à la liste 
            Image dinovole = new Image();
            dinovole.Width = LARGEUR_DINO_VOLANT;
            dinovole.Height = HAUTEUR_DINO_VOLANT;
            Canvas.SetTop(dinovole, POSITION_APPARITION_DINO_VOLANT);
            BitmapImage dinovolant = new BitmapImage(new Uri("pack://application:,,,/img/dino_volant_droite.png"));
            dinovole.Source = dinovolant;
            fond_canvas.Children.Add(dinovole);
            Canvas.SetLeft(dinovole, APPARITION_VOLANT);
            LES_DINOS_VOLANT.Add(dinovole);

        }

        //Méthode de dplacements des dinos volants
        public void DeplacementDinoVolant(object sender, EventArgs e)
        {

            if (LES_DINOS_VOLANT.Count > 0)
            {
                //Définition de la vitesse
                Image dino = LES_DINOS_VOLANT[0];
                double positionDinoY = Canvas.GetTop(dino);
                double positiondinoX = Canvas.GetLeft(dino);
                Canvas.SetTop(dino, positionDinoY + VITESSE_DINO_VOLANT);


                //Si position du dino et supérieur à celle du personnage elle suit le personnage à droite 
                if (Canvas.GetLeft(dino) < Canvas.GetLeft(imgPersoDroite))
                {
                    Canvas.SetLeft(dino, positiondinoX +VITESSE_DINO);
                    BitmapImage imgdino = new BitmapImage(new Uri("pack://application:,,,/img/dino_volant_droite.png"));
                    dino.Source = imgdino;
                }
                //Si position du dino et inférieur  à celle du personnage elle suit le personnage à gauche  
                if (Canvas.GetLeft(dino) > Canvas.GetLeft(imgPersoDroite))
                {
                    Canvas.SetLeft(dino, positiondinoX - VITESSE_DINO);
                    BitmapImage imgdino = new BitmapImage(new Uri("pack://application:,,,/img/dino_volant_gauche.png"));
                    dino.Source = imgdino;
                }

                //fait disparaitre le dino volant quand il est trop bas et en initialise un autre 
                if (positionDinoY > LIMITE_DINO_VOLANT)
                {
                    Console.WriteLine(positionDinoY);
                    fond_canvas.Children.Remove(dino);
                    LES_DINOS_VOLANT.RemoveAt(0);
                    InitDinoVolant();
                }
            }

        }
        private void VerifierCollisions()
        {
            // Créer la hitbox du personnaga e
            Rect joueurHitbox = new Rect(Canvas.GetLeft(imgPersoDroite), Canvas.GetTop(imgPersoDroite), imgPersoDroite.Width, imgPersoDroite.Height);


            //verifie si il ya une colistion entre le dino terrestre et le perso pour chaque dino de la liste 
            foreach (var dino in LES_DINOS_TERRE)
            {
                Rect dinoHitbox = new Rect(Canvas.GetLeft(dino), Canvas.GetTop(dino), dino.Width, dino.Height);
                if (joueurHitbox.IntersectsWith(dinoHitbox))
                {
                    MINUTERIE.Stop();
                    MINUTERIEDINO.Stop();
                    TPSSCORE.Tick -= MajScore;
                    GameOver();

                }
            }

            //verifie si il ya une colistion entre le dino volant  et le perso pour chaque dino de la liste 
            foreach (var dino in LES_DINOS_VOLANT)
            {
                Rect dinoHitbox = new Rect(Canvas.GetLeft(dino), Canvas.GetTop(dino), dino.Width, dino.Height);
                if (joueurHitbox.IntersectsWith(dinoHitbox))
                {
                    MINUTERIE.Stop();
                    MINUTERIEDINO.Stop();
                    TPSSCORE.Tick -= MajScore;
                    GameOver();

                }
            }


            //verifie si il ya une colistion entre la balle  et le dino pour chaque balle de la liste 
            for (int i = 0; i < LES_BALLES.Count; i++)
            {
                Image balle = LES_BALLES[i];
                Rect balleHitbox = new Rect(Canvas.GetLeft(balle), Canvas.GetTop(balle), balle.Width, balle.Height);

                for (int j = 0; j < LES_DINOS_TERRE.Count; j++)
                {
                    Image dino = LES_DINOS_TERRE[j];
                    Rect dinoHitbox = new Rect(Canvas.GetLeft(dino), Canvas.GetTop(dino), dino.Width, dino.Height);

                    // si la hitbox touche cela supprime le dino et la balle 
                    if (balleHitbox.IntersectsWith(dinoHitbox))
                    {
                        fond_canvas.Children.Remove(balle);
                        fond_canvas.Children.Remove(dino);
                        LES_BALLES.RemoveAt(i);
                        LES_DINOS_TERRE.RemoveAt(j);
                        InitDinoTerre();
                        SCORE = SCORE + 1000;
                        return;

                    }
                }
            }
        }
       
    }
    
}