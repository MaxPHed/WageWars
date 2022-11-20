using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
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

namespace RalsShooterWindowMenu
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Game : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        int enemyCounter = 50;
        int pooCounter = 100;
        int playerSpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10;
        int pooSpeed = 10;
        int bajsMackor = 0;

        Rect playerHitBox;
        public Game()
        {
            InitializeComponent();
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/purple.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/edstrom.jpg"));
            player.Fill = playerImage;
        }
        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            enemyCounter -= 1;
            pooCounter -= 1;

            scoreText.Content = "Förhindrad lönehöjning: " + (score) + " kr";
            damageText.Content = "Lönehöjning " + (damage) + "kr";
            pooText.Content = "Bajsmackor igenomsläppta " + bajsMackor;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (pooCounter < 0)
            {
                MakePoo();
                pooCounter = 50;
            }

            if (moveLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(player) + 90 < (MyCanvas.ActualWidth + (player.Width / 2)))//Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemRemover.Add(x);
                    }

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score += 100;
                            }

                        }
                    }
                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "poo")
                        {
                            Rect pooHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(pooHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score -= 50;
                            }

                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);
                    if (Canvas.GetTop(x) > 600)
                    {
                        itemRemover.Add(x);
                        damage += 100;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 50;
                    }

                }
                if (x is Rectangle && (string)x.Tag == "poo")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + pooSpeed);
                    if (Canvas.GetTop(x) > 600)
                    {
                        itemRemover.Add(x);
                        bajsMackor += 1;
                    }

                    Rect pooHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(pooHitBox))
                    {
                        itemRemover.Add(x);
                    }

                }

            }

            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }

            if (score > 500)
            {
                limit = 20;
                enemySpeed = 11;
            }
            if (score > 1000)
            {
                limit = 20;
                enemySpeed = 12;
            }
            if (score > 2000)
            {
                limit = 20;
                enemySpeed = 15;
            }

            if (damage > 100)
            {
                gameTimer.Stop();
                //damageText.Content = "Damage: 100";
                damageText.Foreground = Brushes.Red;
                //MessageBox.Show("Du har förhindrat " + score + " kr i lönehöjning" + Environment.NewLine + "öch släppt igenom " + bajsMackor + " bajsmackor till pilotkollektivet!");
                //MessageBox.Show("Däremot är lönehöjningen redan upp i 4500 kr, vilket är helt oaccaptabelt. Du får därmed sparken!");
                gameOver();
                checkHighScore();
                
                //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                //Application.Current.Shutdown();
            }
        }

        public void checkHighScore()
        {
            HighScore highscore = new HighScore();
            int placement;
            if (score > highscore.lowestHighScore())
            {
                highscore.highScoreList.Add(new HighScore("Maximus", score, bajsMackor));
                highscore.sortHighScore();
                placement = highscore.getPlaceInHighScoreList("Maximus");
                highscore.writeHighScoreToFile();
                MessageBox.Show(placement.ToString());
            }
            else
            {
                MessageBox.Show("To low score to be on highscorelist");
            }
            highscore.showHighScoreList();
        }

        public void gameOver()
        {
            Label label = new Label();
            label.FontSize = 20;
            label.Content = "Spelet är slut. Du släppte igenom " + bajsMackor + " bajsmackor på pilotkollektivet \r\n" +
                "Du förhindrade " + score + " i lönehöjning!";
            Canvas.SetTop(label, 300);
            Canvas.SetLeft(label, 100);
            MyCanvas.Children.Add(label);
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
            if (e.Key == Key.Enter)
            {
                Menu menu = new Menu();
                this.Hide();
                menu.Show();
            }

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }
            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red,
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                MyCanvas.Children.Add(newBullet);
            }
        }

        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();
            enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/money.png"));
            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430));
            MyCanvas.Children.Add(newEnemy);
        }
        private void MakePoo()
        {
            ImageBrush pooSprite = new ImageBrush();
            pooSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/pooop.png"));
            Rectangle newPoo = new Rectangle
            {
                Tag = "poo",
                Height = 50,
                Width = 56,
                Fill = pooSprite
            };

            Canvas.SetTop(newPoo, -100);
            Canvas.SetLeft(newPoo, rand.Next(30, 430));
            MyCanvas.Children.Add(newPoo);
        }
    }
}
