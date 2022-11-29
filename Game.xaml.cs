using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

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
        List<HighScore> highScoreList;
        bool timerOn;
        bool newHighScore;
        int enemyCounter = 50;
        int pooCounter = 110;
        int pensionCounter = 900;
        int playerSpeed = 15;
        int limit = 50;
        int score = 0;
        int damage = 0;
        int enemySpeed = 10;
        int pooSpeed = 10;
        int pensionSpeed = 10;
        int bajsMackor = 0;
        bool pensionLeft = false;
        bool floskelAlive = false;

        Rect playerHitBox;
        public Game(List<HighScore> highScoreList)
        {
            InitializeComponent();
            this.highScoreList = highScoreList;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            timerOn = true;
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
            pensionCounter -= 1;


            scoreText.Content = "Förhindrad lönehöjning: " + (score) + " kr";
            damageText.Content = "Lönehöjning " + (damage) + "kr";
            pooText.Content = "Bajsmackor igenomsläppta " + bajsMackor;
            makeObjects();
            checkMovement();
            progressBarLook();

            checkIfBulletHit();
            floskelKill();
            moveFloskel();
            removeItems();
            checkGameOverAndIncreaseSpeed();
        }

        private void floskelKill()
        {
            if (floskelAlive == true)

            {
                ImageBrush playerImage = new ImageBrush();
                playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/hulk.png"));
                player.Fill = playerImage;
                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    if (y is Rectangle && (string)y.Tag == "enemy")
                    {
                        itemRemover.Add(y);
                        score += 100;
                    }
                    if (y is Rectangle && (string)y.Tag == "55")
                    {
                        itemRemover.Add(y);
                        score += 1000;
                    }
                }
            }
        }

        private void progressBarLook()
        {
            if (pBar.Value == pBar.Maximum)
            {
                pBar.Opacity = 1;
            }
        }

        private void checkGameOverAndIncreaseSpeed()
        {
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

            if (damage > 4500)
            {
                gameTimer.Stop();
                timerOn = false;
                damageText.Foreground = Brushes.Red;

                addGameOverTextToCanvas();
                checkHighScore();
                //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                //Application.Current.Shutdown();
            }
        }

        private void removeItems()
        {
            foreach (Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);
            }
        }

        private void moveFloskel()
        {
            foreach (Image floskel in MyCanvas.Children.OfType<Image>().ToList())
            {
                Canvas.SetTop(floskel, Canvas.GetTop(floskel) - 10);

                if (Canvas.GetTop(floskel) < 10)
                {
                    MyCanvas.Children.Remove(floskel);
                    floskelAlive = false;
                    ImageBrush playerImage = new ImageBrush();
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/edstrom.jpg"));
                    player.Fill = playerImage;
                }
            }
        }

        private void checkMovement()
        {
            if (moveLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(player) + 90 < (MyCanvas.ActualWidth + (player.Width / 2)))//Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }
        }

        private void makeObjects()
        {
            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (pooCounter < 0)
            {
                MakePoo();
                pooCounter = 55;
            }
            if (pensionCounter < 0)
            {
                MakePension();
                pensionCounter = 900;
            }
        }

        private void checkIfBulletHit()
        {
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
                        if (y is Rectangle && (string)y.Tag == "55")
                        {
                            Rect pensionHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(pensionHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score += 500;
                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);
                    if (Canvas.GetTop(x) > 750)
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
                if (x is Rectangle && (string)x.Tag == "55")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + (pensionSpeed / 2));
                    if (Canvas.GetLeft(x) < 28)
                    {
                        pensionLeft = false;
                    }
                    if (pensionLeft == false)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + (pensionSpeed));
                    }
                    if (Canvas.GetLeft(x) > 512)
                    {
                        pensionLeft = true;
                    }
                    if (pensionLeft == true)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - (pensionSpeed));
                    }

                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        damage += 1000;
                    }

                    Rect pensionHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(pensionHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 50;
                    }

                }
                if (x is Rectangle && (string)x.Tag == "poo")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + pooSpeed);
                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        bajsMackor += 1;
                        pBar.Value += 1;

                    }

                    Rect pooHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(pooHitBox))
                    {
                        itemRemover.Add(x);
                    }

                }

            }
        }

        public void checkHighScore()
        {
            //HighScoreWindow highscore = new HighScoreWindow();
            int placement;
            HighScore highScore = new HighScore(highScoreList);
            if (score > highScore.lowestHighScore())
            {
                newHighScore = true;
                placement = highScore.getPlaceInHighScoreList(score);

                string content1 = "NEW HIGH SCORE!\n Your current rank: " + placement;
                string content2 = "Press enter to add your name";
                addLabelToGrid(content1, 40, 0);
                addLabelToGrid(content2, 30, 2);
            }
            else
            {
                string content1 = "Too low score for High Score ";
                string content2 = "Press enter to go back to main menu";
                addLabelToGrid(content1, 40, 0);
                addLabelToGrid(content2, 30, 2);
            }
        }

        public void addGameOverTextToCanvas()
        {
            string content = "Spelet är slut. Du släppte igenom " + bajsMackor + " bajsmackor \r\npå pilotkollektivet och" +
                "förhindrade " + score + " i lönehöjning!";
            addLabelToGrid(content, 20, 1);
        }
        public void addLabelToGrid(string content, int size, int row)
        {
            Label label = new Label();
            label.FontSize = size;
            label.Content = content;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            if (content.StartsWith("NEW H"))
            {
                ControlTemplate controlTemplate = new ControlTemplate();
                controlTemplate = (ControlTemplate)Resources["GlowingLabel"];
                label.Template = controlTemplate;

            }
            Grid.SetRow(label, row);
            GameGrid.Children.Add(label);
            //Panel.SetZIndex(label, 0);

        }

        public string readNameFromFile()
        {
            DirectoryInfo currentdirectory = new DirectoryInfo(".");
            string filePath = currentdirectory.FullName + "\\Files" + @"\Name.txt";
            string name = File.ReadAllText(filePath);
            return name;
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
                if (timerOn == true)
                {
                    Menu menu = new Menu();
                    this.Close();
                    menu.Show();
                }
                if (timerOn == false)
                {
                    if (newHighScore == true)
                    {
                        newHighScore = false;
                        NewHighScore newhighScoreName = new NewHighScore(this, highScoreList, score, bajsMackor);
                        this.Close();
                        newhighScoreName.Show();
                    }
                    else if (newHighScore == false)
                    {
                        Menu menu = new Menu();
                        this.Close();
                        menu.Show();
                    }
                }

            }
            if (e.Key == Key.Up)
            {
                if (pBar.Value >= pBar.Maximum)
                {
                    floskelAlive = true;

                    pBar.Value = 0;
                    pBar.Opacity = 0.5;
                    pBar.Foreground = Brushes.Yellow;
                    Image floskel = new Image();
                    floskel.Source = new BitmapImage(new Uri(@"/Images/Floskel2.png", UriKind.Relative));
                    floskel.Width = 240;
                    floskel.Stretch = Stretch.Uniform;
                    if (Canvas.GetLeft(player) > 300)
                    {
                        Canvas.SetLeft(floskel, 300);
                    }
                    else
                    {
                        Canvas.SetLeft(floskel, Canvas.GetLeft(player) + player.Width);
                    }
                    
                    Canvas.SetTop(floskel, Canvas.GetTop(player) - 40);
                    MyCanvas.Children.Add(floskel);

                }
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

            Canvas.SetTop(newEnemy, 0);
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

            Canvas.SetTop(newPoo, 0);
            Canvas.SetLeft(newPoo, rand.Next(30, 430));
            MyCanvas.Children.Add(newPoo);
        }
        private void MakePension()
        {
            ImageBrush p55Sprite = new ImageBrush();
            p55Sprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/55.png"));
            Rectangle newPension = new Rectangle
            {
                Tag = "55",
                Height = 50,
                Width = 56,
                Fill = p55Sprite
            };

            Canvas.SetTop(newPension, 0);
            Canvas.SetLeft(newPension, rand.Next(90, 400));
            MyCanvas.Children.Add(newPension);
        }
    }
}
