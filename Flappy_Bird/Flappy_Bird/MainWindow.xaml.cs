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
using System.Windows.Threading;

namespace Flappy_Bird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        double gravity = 0.8; // Jačina gravitacije
        double birdVelocity = 0; // Brzina ptice prema dole
        List<Rectangle> pipes = new List<Rectangle>();
        double pipeSpeed = 5; // Brzina pomeranja cijevi
        Random random = new Random();
        int score = 0;
        double gap = 150;
        double pipeSpacing = 150;
        public MainWindow()
        {
            InitializeComponent();

            Panel.SetZIndex(ScoreText, 1);
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GeneratePipes();
        }


        private void GameLoop(object sender, EventArgs e)
        {
            birdVelocity += gravity;
            Canvas.SetTop(Bird, Canvas.GetTop(Bird) + birdVelocity);

            if (Canvas.GetTop(Bird) + Bird.Height >= GameCanvas.ActualHeight)
            {
                Canvas.SetTop(Bird, GameCanvas.ActualHeight - Bird.Height);
                gameTimer.Stop();
                ScoreText.Text = $"Score: {score} - Game Over! Press R to restart";
            }

            if (Canvas.GetTop(Bird) < 0)
            {
                Canvas.SetTop(Bird, 0);
            }

            for (int i = 0; i < pipes.Count; i += 2)
            {
                Rectangle topPipe = pipes[i];
                Rectangle bottomPipe = pipes[i + 1];

                Canvas.SetLeft(topPipe, Canvas.GetLeft(topPipe) - pipeSpeed);
                Canvas.SetLeft(bottomPipe, Canvas.GetLeft(bottomPipe) - pipeSpeed);

                // Proveri koliziju sa pticom
                Rect birdHitBox = new Rect(Canvas.GetLeft(Bird), Canvas.GetTop(Bird), Bird.Width, Bird.Height);
                Rect topPipeHitBox = new Rect(Canvas.GetLeft(topPipe), Canvas.GetTop(topPipe), topPipe.Width, topPipe.Height);
                Rect bottomPipeHitBox = new Rect(Canvas.GetLeft(bottomPipe), Canvas.GetTop(bottomPipe), bottomPipe.Width, bottomPipe.Height);

                if (birdHitBox.IntersectsWith(topPipeHitBox) || birdHitBox.IntersectsWith(bottomPipeHitBox))
                {
                    gameTimer.Stop();
                    ScoreText.Text = $"Score: {score} - Game Over! Press R to restart";
                }

                // Resetuj cijevi kada prođu van leve strane ekrana
                if (Canvas.GetLeft(topPipe) < -topPipe.Width)
                {
                    // Generiši nasumičnu visinu za gornju cijev, ali sa većim gap-om
                    double pipeTopHeight = random.Next(50, (int)(GameCanvas.ActualHeight - gap - 50));

                    // Pomeri cijevi unazad u jednakom razmaku
                    Canvas.SetLeft(topPipe, Canvas.GetLeft(topPipe) + pipeSpacing * pipes.Count / 2);
                    Canvas.SetTop(topPipe, 0);
                    topPipe.Height = pipeTopHeight;

                    Canvas.SetLeft(bottomPipe, Canvas.GetLeft(bottomPipe) + pipeSpacing * pipes.Count / 2);
                    Canvas.SetTop(bottomPipe, pipeTopHeight + gap);
                    bottomPipe.Height = GameCanvas.ActualHeight - pipeTopHeight - gap;

                    // Povećaj rezultat jer je ptica prošla kroz cijevi
                    score++;
                    ScoreText.Text = $"Score: {score}";
                }
            }
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                birdVelocity = -12; // Brzina prema gore kada se pritisne razmak
            }
            else if (e.Key == Key.R)
            {
                ResetGame(); // Pokreni reset igre kada se pritisne "R"
            }
        }

        
        private void GeneratePipes()
        {

            // Generiši više parova cijevi u pravilnim intervalima
            for (int i = 0; i < 4; i++)
            {
                double pipeTopHeight = random.Next(50, (int)(GameCanvas.ActualHeight - gap - 50));

                Rectangle topPipe = new Rectangle
                {
                    Width = 50,
                    Height = pipeTopHeight,
                    Fill = Brushes.Green
                };

                Rectangle bottomPipe = new Rectangle
                {
                    Width = 50,
                    Height = GameCanvas.ActualHeight - pipeTopHeight - gap,
                    Fill = Brushes.Green
                };

                // Postavi početnu poziciju svakog para cijevi u pravilnim razmacima
                Canvas.SetLeft(topPipe, GameCanvas.ActualWidth + i * pipeSpacing);
                Canvas.SetTop(topPipe, 0);

                Canvas.SetLeft(bottomPipe, GameCanvas.ActualWidth + i * pipeSpacing);
                Canvas.SetTop(bottomPipe, pipeTopHeight + gap);

                // Dodaj cijevi na platno i u listu
                GameCanvas.Children.Add(topPipe);
                GameCanvas.Children.Add(bottomPipe);
                pipes.Add(topPipe);
                pipes.Add(bottomPipe);
            }
        }




        private void ResetGame()
        {
            // Resetuj rezultat i prikaži ga
            score = 0;
            ScoreText.Text = "Score: 0";

            // Resetuj poziciju ptice
            birdVelocity = 0;
            Canvas.SetTop(Bird, 200); // Vraća pticu na početnu poziciju

            // Resetuj poziciju cijevi
            for (int i = 0; i < pipes.Count; i += 2)
            {
                Rectangle topPipe = pipes[i];
                Rectangle bottomPipe = pipes[i + 1];

                double gap = 150;
                double pipeTopHeight = random.Next(50, (int)(GameCanvas.ActualHeight - gap - 50));

                Canvas.SetLeft(topPipe, GameCanvas.ActualWidth + i * 200); // Razmak između setova cijevi
                Canvas.SetTop(topPipe, 0);
                topPipe.Height = pipeTopHeight;

                Canvas.SetLeft(bottomPipe, GameCanvas.ActualWidth + i * 200);
                Canvas.SetTop(bottomPipe, pipeTopHeight + gap);
                bottomPipe.Height = GameCanvas.ActualHeight - pipeTopHeight - gap;
            }

            // Ponovo pokreni tajmer igre
            gameTimer.Start();
        }



    }
}
