using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WageWars
{
    public class HighScore
    {
        List<HighScore> highScoreList;
        

        private string _name;
        private int _score;
        private int _bajsmackor;
        string _utskrift = "";
        //public List<HighScore> highScoreList { get => _highScoreList; set => _highScoreList = value; }
     
        public string name { get => _name; set => _name = value; }
        public int score { get => _score; set => _score = value; }
        public int bajsmackor { get => _bajsmackor; set => _bajsmackor = value; }
        public string utskrift { get => _utskrift; set => _utskrift = value; }
        
        public HighScore(List<HighScore> highScoreList)
        {
            this.highScoreList = highScoreList;
        }
        public HighScore(string name, int score, int bajsmackor)
        {
            this.name = name;
            this.score = score;
            this.bajsmackor = bajsmackor;
            //writeHighScoreToFile();
            //sortHighScoreList();

        }
        
        public int lowestHighScore()
        {
            int lowestScore = highScoreList.Last().score;
            return lowestScore;
        }
        public int getPlaceInHighScoreList(int score)
        {
            int myScore = score;
            int placement = 0;
            foreach (HighScore highscore in highScoreList)
            {
                int oldScore = highscore.score;
                if (oldScore < myScore)
                {
                    placement = highScoreList.IndexOf(highscore) + 1;
                    break;
                }
            }
            return placement;
        }
        
        
        public void showHighScoreListInMessageBox()
        {
            foreach (HighScore highScore in highScoreList)
            {
                utskrift += highScore.name + "," + highScore.score + "\n";
            }

            MessageBox.Show(utskrift);
        }
    }
}
