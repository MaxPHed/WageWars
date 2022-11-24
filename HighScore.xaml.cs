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
    public partial class HighScoreWindow : Window
    {
        Window parent;
        List<HighScore> highScoreList;

        //List<HighScore> highScoreList = highScoreList.OrderBy(o => o.score).ToList();

        public HighScoreWindow(Window parent, List<HighScore> highScoreList)
        {
            
            this.parent = parent;
            this.highScoreList = highScoreList;

            InitializeComponent();

            
            addHighScoresLabelsOnHighScorePage(highScoreList);
        }

       


        public void addHighScoresLabelsOnHighScorePage(List<HighScore> highScoreList)
        {

            foreach (HighScore highScore in highScoreList)
            {
                Label label = new Label();
                label.FontSize = 20;
                label.Content = highScore.name + " " + highScore.score;
                HSStack.Children.Add(label);
            }
        }
 
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                parent.Show();
            }
        }
    }
}
