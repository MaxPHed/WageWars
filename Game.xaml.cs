using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;

namespace RalsShooterWindowMenu
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Game : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer(DispatcherPriority.Render);
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();
        static DirectoryInfo currentdirectory = new DirectoryInfo(".");
        SoundPlayer gunSound = new SoundPlayer(currentdirectory.FullName + "\\Sounds" + "\\Guns.Wav");
        MediaPlayer backgroundMusic = new MediaPlayer();
        MediaPlayer moneySound = new MediaPlayer();
        MediaPlayer poopSound = new MediaPlayer();
        MediaPlayer highScoreSound = new MediaPlayer();
        MediaPlayer gameOverSound = new MediaPlayer();
        MediaPlayer floskelSound = new MediaPlayer();
        MediaPlayer pensionSound = new MediaPlayer();
        MediaPlayer floskelHit = new MediaPlayer();
        MediaPlayer floskelDead = new MediaPlayer();



        Random rand = new Random();
        List<HighScore> highScoreList;
        bool timerOn;
        bool newHighScore;
        int moneyCounter = 50;
        int pooCounter = 110;

        int pensionCounter = 900;
        int pensionSpawnRate = 900;
        int pensionHealth = 3;
        int playerSpeed = 14;
        int moneySpawnRate = 50;
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
            backgroundMusic.Open(new Uri(@"Sounds/DangerZone.mp3", UriKind.Relative));
            backgroundMusic.Play();
        }


        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            moneyCounter -= 1;
            pooCounter -= 1;
            pensionCounter -= 1;


            scoreText.Content = "Förhindrad lönehöjning: " + (score) + " kr";
            damageText.Content = "Lönehöjning " + (damage) + "kr";
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

                foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                {
                    if (y is Rectangle && (string)y.Tag == "money")
                    {
                        itemRemover.Add(y);
                        score += 100;
                    }
                    if (y is Rectangle && (string)y.Tag == "55")
                    {
                        if(pensionHealth>1)
                        {
                            pensionHit(y, 1);
                        }
                    }
                }
            }
        }

        private void progressBarLook()
        {
            if (pBar.Value == pBar.Maximum)
            {
                pBar.Opacity = 1;
                progressPoop.Height = 20;
                progessBarFullLabel.Visibility = Visibility.Visible;
            }
            if (pBar.Value != pBar.Maximum)
            {
                progessBarFullLabel.Visibility = Visibility.Hidden;
                progressPoop.Height = 15;
                pBar.Opacity = 0.5;
                pBar.Foreground = Brushes.Gold;

            }
        }

        private void checkGameOverAndIncreaseSpeed()
        {
            if (score > 500)
            {
                moneySpawnRate = 20;
                enemySpeed = 11;
            }
            if (score > 1000)
            {
                moneySpawnRate = 20;
                enemySpeed = 12;
            }
            if (score > 2000)
            {
                moneySpawnRate = 19;
                enemySpeed = 13;
            }
            if (score > 4000)
            {
                moneySpawnRate = 18;
                enemySpeed = 14;
                pensionSpawnRate = 800;
            }
            if (score > 8000)
            {
                moneySpawnRate = 17;
                pensionSpawnRate = 700;
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
            else if (moveRight == true && Canvas.GetLeft(player) + 90 < (MyCanvas.ActualWidth + (player.Width / 2)))//Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
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

                        if (y is Rectangle && (string)y.Tag == "money")
                        {
                            bulletHitObject(bulletHitBox, x, y, 100);
                        }
                        if (y is Rectangle && (string)y.Tag == "poo")
                        {
                            bulletHitObject(bulletHitBox, x, y, -50);
                        }
                        if (y is Rectangle && (string)y.Tag == "55")
                        {
                            Rect hitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(hitBox))
                            {
                                itemRemover.Add(x);
                                pensionHit(y, 1);

                            }
                        }
                    }
                }

                if (x is Rectangle && (string)x.Tag == "money")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);
                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        damage += 100;
                        moneySound.Open(new Uri(@"Sounds/Money.wav", UriKind.Relative));
                        moneySound.Play();
                    }
                    objectHitPlayer(x, 50);
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
                    objectHitPlayer(x, 50);
                }
                if (x is Rectangle && (string)x.Tag == "poo")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + pooSpeed);
                    if (Canvas.GetTop(x) > 750)
                    {
                        itemRemover.Add(x);
                        bajsMackor += 1;
                        pBar.Value += 1;
                        poopSound.Open(new Uri(@"Sounds/Poop.wav", UriKind.Relative));
                        poopSound.Play();

                    }
                    objectHitPlayer(x, 0);
                }
            }
        }

        private void pensionHit(Rectangle y, int damage)
        {
            pensionHealth = pensionHealth - damage;
            floskelHit.Open(new Uri(@"Sounds/Hit_Floskel.wav", UriKind.Relative));
            floskelHit.Play();
            y.Stroke = Brushes.Gold;
            y.StrokeThickness = y.StrokeThickness + 1;
            y.Width = y.Width - 10;
            y.Height = y.Height - 10;
            if (pensionHealth <= 0)
            {
                floskelDead.Open(new Uri(@"Sounds/FloskelDead.wav", UriKind.Relative));
                floskelDead.Play();
                itemRemover.Add(y);
                score += 500;
                pensionHealth = 3;
            }
        }

        private void objectHitPlayer(Rectangle x, int points)
        {
            Rect objectHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

            if (playerHitBox.IntersectsWith(objectHitBox))
            {
                itemRemover.Add(x);
                damage += points;
            }
        }

        private void bulletHitObject(Rect bulletHitBox, Rectangle x, Rectangle y, int v)
        {
            Rect hitBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

            if (bulletHitBox.IntersectsWith(hitBox))
            {
                itemRemover.Add(x);
                itemRemover.Add(y);
                score += v;
            }
        }

        public void checkHighScore()
        {
            int placement;
            HighScore highScore = new HighScore(highScoreList);
            if (score > highScore.lowestHighScore())
            {
                backgroundMusic.Volume = 0.1;
                highScoreSound.Open(new Uri(@"Sounds/Highscore_1.wav", UriKind.Relative));
                highScoreSound.Play();
                newHighScore = true;
                placement = highScore.getPlaceInHighScoreList(score);

                string content1 = "NEW HIGH SCORE!\n Your current rank: " + placement;
                string content2 = "Press enter to add your name";
                addLabelToGrid(content1, 40, 0);
                addLabelToGrid(content2, 30, 2);
            }
            else
            {
                gameOverSound.Open(new Uri(@"Sounds/Game_over_1.wav", UriKind.Relative));
                backgroundMusic.Stop();
                gameOverSound.Play();
                string content1 = "Game Over";
                string content2 = "Press ENTER";
                addLabelToGrid(content1, 40, 0);
                addLabelToGrid(content2, 30, 2);
            }
        }

        public void addGameOverTextToCanvas()
        {
            string content =
                "Du förhindrade " + score + " kr i lönehöjning!";
            addLabelToGrid(content, 20, 1);
        }
        public void addLabelToGrid(string content, int size, int row)
        {
            Label label = new Label();
            label.FontSize = size;
            label.Content = content;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Foreground = Brushes.Gold;
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
                    gameTimer.Stop();
                    backgroundMusic.Stop();
                    this.Close();
                    menu.Show();
                }
                if (timerOn == false)
                {
                    if (newHighScore == true)
                    {
                        backgroundMusic.Volume = 0.5;
                        newHighScore = false;
                        NewHighScore newhighScoreName = new NewHighScore(this, highScoreList, score, bajsMackor);
                        this.Hide();
                        newhighScoreName.Show();
                    }
                    else if (newHighScore == false)
                    {
                        Menu menu = new Menu();
                        this.Close();
                        backgroundMusic.Stop();
                        menu.Show();
                    }
                }

            }
            if (e.Key == Key.Up)
            {
                if (timerOn)
                {
                    if (pBar.Value >= pBar.Maximum)
                    {
                        floskelSound.Open(new Uri(@"Sounds/Floskel_1.wav", UriKind.Relative));

                        floskelSound.Play();
                        floskelAlive = true;
                        ImageBrush playerImage = new ImageBrush();
                        playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/edstrom_2.jpg"));
                        player.Fill = playerImage;

                        pBar.Value = 0;

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
                if (timerOn)
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
                    gunSound.Play();
                }

            }
        }
        private void makeObjects()
        {
            if (moneyCounter < 0)
            {
                makeEnemy("money", "pack://application:,,,/Images/money.png");
                moneyCounter = moneySpawnRate;
            }

            if (pooCounter < 0)
            {
                makeEnemy("poo", "pack://application:,,,/Images/pooop.png");
                pooCounter = 55;
            }
            if (pensionCounter < 0)
            {
                makeEnemy("55", "pack://application:,,,/Images/55.jpg");
                pensionSound.Open(new Uri(@"Sounds/pensionIncoming.wav", UriKind.Relative));
                pensionSound.Play();
                pensionCounter = pensionSpawnRate;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            backgroundMusic.Stop();
        }

        private void makeEnemy(string type, string filePath)
        {
            ImageBrush enemySprite = new ImageBrush();
            enemySprite.ImageSource = new BitmapImage(new Uri(filePath));
            Rectangle newEnemy = new Rectangle
            {
                Tag = type,
                Height = 50,
                Width = 56,
                Fill = enemySprite
            };
            
            Canvas.SetTop(newEnemy, 0);
            Canvas.SetLeft(newEnemy, rand.Next(30, 430));
            MyCanvas.Children.Add(newEnemy);
        }

    }
}
