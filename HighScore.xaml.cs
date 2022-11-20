using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace RalsShooterWindowMenu
{
    /// <summary>
    /// Interaction logic for HighScore.xaml
    /// </summary>
    public partial class HighScore : Window
    {
        private string _name;
        private int _score;
        private int _bajsmackor;
        string _utskrift = "";
        List<HighScore> _highScoreList = new List<HighScore>();
        public string name { get => _name; set => _name = value; }
        public int score { get => _score; set => _score = value; }
        public int bajsmackor { get => _bajsmackor; set => _bajsmackor = value; }
        public string utskrift { get => _utskrift; set => _utskrift = value; }
        public List<HighScore> highScoreList { get => _highScoreList; set => _highScoreList = value; }


        //List<HighScore> highScoreList = highScoreList.OrderBy(o => o.score).ToList();


        public HighScore()
        {
            InitializeComponent();
            readHighScoreFromFile();
            sortHighScore();
            //showHighScoreList();


            foreach (HighScore highScore in highScoreList)
            {
                Label label = new Label();
                label.FontSize = 20;
                label.Content = highScore.name + " " + highScore.score;
                HSStack.Children.Add(label);
            }


        }



        public HighScore(string name, int score, int bajsmackor)
        {
            this.name = name;
            this.score = score;
            this.bajsmackor = bajsmackor;
            highScoreList.Add(this);
            sortHighScore();

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

        public void showHighScoreList()
        {
            foreach (HighScore highScore in highScoreList)
            {
                utskrift += highScore.name + "," + highScore.score + "\n";
            }

            MessageBox.Show(utskrift);
        }

        public int getPlaceInHighScoreList(string name)
        {
            string myName = name;
            int placement = highScoreList.FindIndex(p => p.name == myName);
            return placement;
        }

        public int lowestHighScore()
        {
            int lowestScore = highScoreList.Last().score;
            return lowestScore;
        }

        public void sortHighScore()
        {
            highScoreList.Sort((x, y) => y.score.CompareTo(x.score));
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Menu menu = new Menu();
                this.Hide();
                menu.Show();
            }
        }
    }
}
