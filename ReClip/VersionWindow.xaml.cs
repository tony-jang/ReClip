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

namespace ReClip
{
    /// <summary>
    /// VersionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VersionWindow : Window
    {
        public VersionWindow()
        {
            InitializeComponent();

            this.MouseDown += Window_MouseDown;
            this.MouseUp += Window_MouseUp;
            this.KeyDown += Window_KeyDown;
            this.Deactivated += Window_Deactivated;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try { this.Close(); } catch { }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) this.Close();
        }

        bool mouseDown = false;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDown)
            {
                this.Close();
            }
            mouseDown = false;
        }
    }
}
