using System;
using System.Collections.Generic;
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
    /// Interaction logic for Intro.xaml
    /// </summary>
    public partial class Intro : Window
    {
        Menu parent;
        public Intro(Menu parent)
        {
            InitializeComponent();
            this.parent = parent;
            this.Focus();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                parent.IntroPlaying = false;
                this.Close();
                parent.Show();
            }
        }
    }
}
