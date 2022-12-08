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
        string name = "";
        int indexCounter = 0;
        Label labelFixed = new Label();
        Label labelPending = new Label();
        int newScore;
        int bajsMackor;
        int placement;
        List<HighScore> highScoreList;
        private bool BlinkOn = false;
        WrapPanel newScorePanel = new WrapPanel();


        string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Å", "Ä", "Ö" };



        public NewHighScore(Window game, List<HighScore> highScoreList, int newScore, int bajsMackor)
        {
            InitializeComponent();
            this.highScoreList = highScoreList;
            parent = game;
            HighScore highscore = new HighScore(highScoreList);
            placement = highscore.getPlaceInHighScoreList(newScore);

            this.newScore = newScore;

            sortAllHighScores(placement);
            labelPending.FontSize = 24;
            newScorePanel.Height = 45;
            newScorePanel.Children.Add(labelPending);
            labelPending.Content = alphabet[0];
            this.bajsMackor = bajsMackor;

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Loaded);
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();

        }

        private void sortAllHighScores(int placement)
        {
            int place = 1;
            foreach (HighScore highscore in highScoreList)
            {

                if (highscore.score >= newScore)
                {
                    AddLabelToStack(highscore, place);
                    place++;
                }
            }

            Label newScoreLabel = new Label();
            newScoreLabel.FontSize = 24;
            labelFixed.FontSize = 24;
            labelFixed.Foreground = Brushes.Gold;
            labelFixed.FontWeight = FontWeights.Bold;
            newScoreLabel.Foreground = Brushes.Gold;
            newScoreLabel.FontWeight = FontWeights.Bold;
            newScoreLabel.Content = placement.ToString();
            newScoreLabel.Content += "  "+ newScore.ToString();

            newScorePanel.Children.Add(newScoreLabel);
            newScorePanel.Children.Add(labelFixed);
            HSStack.Children.Add(newScorePanel);
            place = placement + 1;

            foreach (HighScore highscore in highScoreList)
            {
                if (highscore.score < newScore)
                {
                    if(place == 11)
                    {
                        break;
                    }
                    else
                    {
                        AddLabelToStack(highscore, place);
                        place++;
                    }
                    
                }
            }

        }

        private void AddLabelToStack(HighScore highscore, int place)
        {
            WrapPanel panel = new WrapPanel();
            Label formerScoresLabel = new Label();
            Label labelScore = new Label();
            formerScoresLabel.Content = place.ToString();
            formerScoresLabel.Content += "  " + highscore.score;

            formerScoresLabel.Content += "  " + highscore.name;
            formerScoresLabel.Foreground = Brushes.White;
            panel.Children.Add(formerScoresLabel);
            HSStack.Children.Add(panel);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (BlinkOn)
            {
                labelPending.Foreground = Brushes.Gold;
                labelPending.Background = Brushes.Black;
            }
            else
            {
                labelPending.Foreground = Brushes.Black;
                labelPending.Background = Brushes.Gold;
            }
            BlinkOn = !BlinkOn;
        }
        private void addLetter()
        {
            letter = alphabet[indexCounter];
            name += letter;
            labelFixed.Content = name;
            labelFixed.FontSize = 24;
            labelFixed.Foreground = Brushes.Gold;
            letter = alphabet[0];
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if ((indexCounter >= 0) && (indexCounter <= 27))
                {
                    indexCounter++;
                }
                else
                {
                    indexCounter = 0;
                }
                labelPending.Content = alphabet[indexCounter];
                labelPending.FontSize = 30;

                HSStack.UpdateLayout();
            }
            if (e.Key == Key.Down)
            {
                if (indexCounter == 0)
                {
                    indexCounter = 28;
                }
                else if (indexCounter <= 29)
                {
                    indexCounter--;
                }
                labelPending.Content = alphabet[indexCounter];
                HSStack.UpdateLayout();
            }
            if (e.Key == Key.Enter)
            {
                if (name.Length < 3)
                {
                    addLetter();

                }
                if (name.Length >= 3)
                {
                    highScoreList.Add(new HighScore(name, newScore, bajsMackor));
                    Menu menu = new Menu(highScoreList);
                    
                    parent.Close();

                    this.Close();
                    menu.Show();
                }
            }
        }
    }
}
