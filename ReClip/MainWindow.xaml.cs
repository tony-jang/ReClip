using ClipboardHelper;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiteDB;
using ReClip.Clips;
using f=System.Windows.Forms;
using System.Threading;
using System.Windows.Media.Animation;
using ReClip.Control;

namespace ReClip
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



            //MessageBox.Show(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

            ClipboardMonitor.Start();
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;

            this.Closed += delegate (object sender, EventArgs e) { ClipboardMonitor.Stop(); };


            LiteDatabase db = new LiteDatabase(System.IO.Path.GetFullPath("ReClipDB.db"));

            LiteCollection<Clip> coll = db.GetCollection<Clip>();

            //coll.Insert(new StringClipItem("a"));

            var itm = coll.FindAll();

            foreach(var collItm in itm)
            {
                MessageBox.Show(collItm.Format.ToString() + " :: " + ((StringClip)collItm).Data );
            }

            this.Deactivated += MainWindow_Deactivated;
            ClipListView.SelectionChanged += ClipListView_SelectionChanged;
            

            SetWindow();
        }
        private void ScrollToPosition(AniScrollViewer viewer, double x)
        {
            DoubleAnimation horzAnim = new DoubleAnimation();
            horzAnim.From = viewer.HorizontalOffset;
            horzAnim.To = x;
            horzAnim.DecelerationRatio = .2;
            horzAnim.EasingFunction = new CircleEase();
            horzAnim.Duration = new Duration(TimeSpan.FromMilliseconds(Math.Abs(horzAnim.From.Value - horzAnim.To.Value) + 100));
            Storyboard sb = new Storyboard();
            
            sb.Children.Add(horzAnim);
            Storyboard.SetTarget(horzAnim, viewer);
            Storyboard.SetTargetProperty(horzAnim, new PropertyPath(AniScrollViewer.CurrentHorizontalOffsetProperty));
            sb.Begin();
        }


        private void ClipListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ClipListView.Items.IndexOf(ClipListView.SelectedItem);
            if (index == -1) return;

            int itmWidth = 120 + 20;
            int AllWidth = ClipListView.Items.Count * itmWidth;
            int currOffset = index * itmWidth;
            int scrollOffset = (currOffset + itmWidth / 2) - ((int)this.Width / 2);

            AniScrollViewer viewer = (AniScrollViewer)GetDescendantByType(ClipListView, typeof(AniScrollViewer));

            if (scrollOffset > 0)
            {
                if (viewer.ScrollableWidth < scrollOffset) scrollOffset = (int)viewer.ScrollableWidth;
                ScrollToPosition(viewer, scrollOffset);
            }
            else
            {
                ScrollToPosition(viewer, 0);
            }
        }
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null) return null;
            if (element.GetType() == type) return element;
            Visual foundElement = null;
            if (element is FrameworkElement) (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null) break;
            }
            return foundElement;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //this.Hide();
        }
        
        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            
        }
        
        public void SetWindow()
        {
            this.Left = 0;
            this.Top = f.Screen.PrimaryScreen.Bounds.Height - 60 - this.Height;
            this.Width = f.Screen.PrimaryScreen.Bounds.Width;

            this.Focus();
            this.Topmost = true;
        }       
    }
}
