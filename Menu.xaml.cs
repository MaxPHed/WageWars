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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        int _highLight = 1;
        public int HighLight { get => _highLight; set => _highLight = value; }

        public Menu()
        {
            InitializeComponent();
            highLightFrame();
            
        }
        private void highLightFrame()
        {
            if (HighLight == 1)
            {
                NGLine.StrokeThickness = 10;
                HSLine.StrokeThickness = 3;
                HTPLine.StrokeThickness = 3;
            }
            if (HighLight == 2)
            {
                NGLine.StrokeThickness = 3;
                HSLine.StrokeThickness = 10;
                HTPLine.StrokeThickness = 3;
            }
            if (HighLight == 3)
            {
                NGLine.StrokeThickness = 3;
                HSLine.StrokeThickness = 3;
                HTPLine.StrokeThickness = 10;
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
                    HighLight = 1;
                    
                }
            }
            if (e.Key == Key.Up)
            {
                if (HighLight == 1)
                {
                    HighLight = 3;
                }
                else if (HighLight == 2)
                {
                    HighLight = 1;

                }
                else if (HighLight == 3)
                {
                    HighLight = 2;
                }
            }
            if (e.Key == Key.Enter)
            {
                if (HighLight == 1)
                {
                    Game game = new Game();
                    this.Hide();
                    game.Show();
                }
                else if (HighLight == 2)
                {
                    HighLight = 1;

                }
                else if (HighLight == 3)
                {
                    HighLight = 2;
                }
            }
            highLightFrame();
        }
    }
}
