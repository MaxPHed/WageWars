using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WageWars
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {

        int _highLight = 1;
        public int HighLight { get => _highLight; set => _highLight = value; }
        public bool IntroPlaying { get => introPlaying; set => introPlaying = value; }

        List<HighScore> highScoreList = new List<HighScore>();
        MediaPlayer backgroundMusic = new MediaPlayer();
        bool introPlaying = false;



        public Menu()
        {

            InitializeComponent();
            readHighScoreFromFile();
            highLightFrame();
            backgroundMusic.Open(new Uri(@"Sounds/TopGunTheme.mp3", UriKind.Relative));
            backgroundMusic.Play();
            backgroundMusic.MediaEnded += new EventHandler(Media_Ended);
        }

        private void Media_Ended(object sender, EventArgs e)
        {
            backgroundMusic.Open(new Uri(@"Sounds/TopGunTheme.mp3", UriKind.Relative));
            backgroundMusic.Play();
        }

        public Menu(List<HighScore> highScoreList)
        {
            InitializeComponent();
            this.highScoreList = highScoreList;
            sortHighScoreList();
            writeHighScoreToFile();
            highLightFrame();
            backgroundMusic.Open(new Uri(@"Sounds/TopGunTheme.mp3", UriKind.Relative));
            backgroundMusic.Play();
        }
        public void sortHighScoreList()
        {
            highScoreList.Sort((x, y) => y.score.CompareTo(x.score));
            if(highScoreList.Count > 10)
            {
                highScoreList.RemoveAt(10);
            }
        }
        private void highLightFrame()
        {
            if (HighLight == 1)
            {
                NGLine.StrokeThickness = 8;
                NGLine.Stroke = Brushes.Gold;
                HSLine.StrokeThickness = 3;
                HSLine.Stroke = Brushes.RoyalBlue;
                HTPLine.StrokeThickness = 3;
                HTPLine.Stroke = Brushes.RoyalBlue;
                ILine.StrokeThickness = 3;
                ILine.Stroke = Brushes.RoyalBlue;
            }
            if (HighLight == 2)
            {
                NGLine.StrokeThickness = 3;
                NGLine.Stroke = Brushes.RoyalBlue;
                HSLine.StrokeThickness = 8;
                HSLine.Stroke = Brushes.Gold;
                HTPLine.StrokeThickness = 3;
                HTPLine.Stroke = Brushes.RoyalBlue;
                ILine.StrokeThickness = 3;
                ILine.Stroke = Brushes.RoyalBlue;

            }
            if (HighLight == 3)
            {
                NGLine.StrokeThickness = 3;
                NGLine.Stroke = Brushes.RoyalBlue;
                HSLine.StrokeThickness = 3;
                HSLine.Stroke = Brushes.RoyalBlue;
                HTPLine.StrokeThickness = 8;
                HTPLine.Stroke = Brushes.Gold;
                ILine.StrokeThickness = 3;
                ILine.Stroke = Brushes.RoyalBlue;
            }
            if (HighLight == 4)
            {
                NGLine.StrokeThickness = 3;
                NGLine.Stroke = Brushes.RoyalBlue;
                HSLine.StrokeThickness = 3;
                HSLine.Stroke = Brushes.RoyalBlue;
                HTPLine.StrokeThickness = 3;
                HTPLine.Stroke = Brushes.RoyalBlue;
                ILine.StrokeThickness = 8;
                ILine.Stroke = Brushes.Gold;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Down)
            {
                if (HighLight == 1)
                {
                    HighLight = 2;
                }
                else if (HighLight == 2)
                {
                    HighLight = 3;

                }
                else if (HighLight == 3)
                {
                    HighLight = 4;

                }
                else if (HighLight == 4)
                {
                    HighLight = 1;

                }
            }
            if (e.Key == Key.Up)
            {
                if (HighLight == 1)
                {
                    HighLight = 4;
                }
                else if (HighLight == 2)
                {
                    HighLight = 1;

                }
                else if (HighLight == 3)
                {
                    HighLight = 2;
                }
                else if (HighLight == 4)
                {
                    HighLight = 3;
                }
            }
            if (e.Key == Key.Enter)
            {
                if (HighLight == 1)
                {
                    Game game = new Game(highScoreList);
                    this.Close();
                    backgroundMusic.Stop();
                    game.ShowDialog();
                }
                else if (HighLight == 2)
                {
                    HighScoreWindow highScoreWindow = new HighScoreWindow(this, highScoreList);
                    this.Hide();
                    //backgroundMusic.Stop();
                    highScoreWindow.Show();

                }
                else if (HighLight == 3)
                {
                    HowToPlay howToPlay = new HowToPlay(this);
                    this.Hide();
                    howToPlay.Show();

                }
                else if (HighLight == 4)
                {
                    Intro intro = new Intro(this);
                    intro.Show();
                    this.Hide();
                }
            }
            highLightFrame();
        }
        public void writeHighScoreToFile()
        {
            DirectoryInfo currentdirectory = new DirectoryInfo(".");
            string filePath = currentdirectory.FullName + "\\Files" + @"\HighScore.txt";
            string[] arrLine = new string[10];
            string hs;
            for (int i = 0; i < 10; i++)
            {
                hs = highScoreList[i].name + "," + highScoreList[i].score + "," + highScoreList[i].bajsmackor;
                arrLine[i] = hs;
            }
            File.WriteAllLines(filePath, arrLine); //Skriver en ny textfil med arrayen vi skapade två rader upp.
        }
        public void readHighScoreFromFile()
        {
            DirectoryInfo currentdirectory = new DirectoryInfo(".");
            string filePath = currentdirectory.FullName + "\\Files" + @"\HighScore.txt";
            string[] readLine = File.ReadAllLines(filePath);

            foreach (string hsLine in readLine)
            {
                if (String.IsNullOrEmpty(hsLine))
                {
                    break;
                }
                string[] hsData = hsLine.Split(','); //Varje string som separeras med ett komma blir en string i denna array.
                string name = hsData[0]; //Platsen för var i arrayen avgör vilken typ av string det är.
                int score = Convert.ToInt32(hsData[1]);
                int bajsmackor = Convert.ToInt32(hsData[2]);

                highScoreList.Add(new HighScore(name, score, bajsmackor));
            }
        }
    }
}
