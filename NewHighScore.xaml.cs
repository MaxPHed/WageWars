using System;
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
using System.Windows.Threading;

namespace RalsShooterWindowMenu
{
    /// <summary>
    /// Interaction logic for NewHighScore.xaml
    /// </summary>
    public partial class NewHighScore : Window
    {
        Window parent;
        string letter = "A";
        string name ="";
        int indexCounter = 0;
        Label labelFixed = new Label();
        Label labelPending = new Label();
        int score;
        int bajsMackor;
        List<HighScore> highScoreList;
        private bool BlinkOn = false;


        string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Å", "Ä", "Ö" };

        

        public NewHighScore(Window game, List<HighScore> highScoreList, int score, int bajsMackor)
        {
            InitializeComponent();
            this.highScoreList = highScoreList;
            parent = game;
            HSStack.Children.Add(labelFixed);
            HSStack.Children.Add(labelPending);
            labelPending.Content = alphabet[0];
            this.score = score;
            this.bajsMackor = bajsMackor;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();

        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (BlinkOn)
            {
                labelPending.Foreground = Brushes.Black;
                labelPending.Background = Brushes.White;
            }
            else
            {
                labelPending.Foreground = Brushes.White;
                labelPending.Background = Brushes.Black;
            }
            BlinkOn = !BlinkOn;
        }
        private void addLetter()
        {
            letter = alphabet[indexCounter];
            name += letter;
            labelFixed.Content = name;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if ((indexCounter >= 0) && (indexCounter <=27) )
                {
                    indexCounter++;
                }
                else
                {
                    indexCounter = 0;
                }
                labelPending.Content = alphabet[indexCounter];
                HSStack.UpdateLayout();
            }
            if (e.Key == Key.Down)
            {
                if (indexCounter == 0)
                {
                    indexCounter =28;
                }
                else if(indexCounter <= 29)
                {
                    indexCounter--;
                }
                labelPending.Content = alphabet[indexCounter];
                HSStack.UpdateLayout();
            }
            if (e.Key == Key.Enter)
            {
                if(name.Length < 3)
                {
                    addLetter();

                }
                if (name.Length >= 3)
                {
                    highScoreList.Add(new HighScore(name, score, bajsMackor));
                    Menu menu = new Menu(highScoreList);
                    this.Close();
                    menu.Show();
                }
            }
        }
        public void writeNameToFile()
        {
            DirectoryInfo currentdirectory = new DirectoryInfo(".");
            string filePath = currentdirectory.FullName + "\\Files" + @"\Name.txt";
            File.WriteAllText(filePath, name);
        }
    }
}
