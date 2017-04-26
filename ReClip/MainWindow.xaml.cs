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
using ReClip.Util;
using System.Diagnostics;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.HotKeys;
using System.Windows.Interop;

using WinClipboard = System.Windows.Forms.Clipboard;
using WinBitmap = System.Drawing.Bitmap;
using BitmapDB = ReClip.Database.BitmapCache;

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

            ClipboardMonitor.Start();
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;

            ConnectKeyHook();

            LiteDatabase db = new LiteDatabase(System.IO.Path.GetFullPath("ReClipDB.db"));
            LiteCollection<Clip> coll = db.GetCollection<Clip>();

            ClipListView.Items.Clear();
            ClipListView.SelectionChanged += ClipListView_SelectionChanged;

            this.Deactivated += MainWindow_Deactivated;
            this.Loaded += MainWindow_Loaded;
            this.PreviewKeyDown += MainWindow_KeyDown;

            SetWindow();
            this.Hide();


            this.Closed += delegate (object sender, EventArgs e) {
                ClipboardMonitor.Stop();
                DisConnectKeyHook();
            };
        }


        ClipboardFormat LastFormat = ClipboardFormat.None;

        string LastText = null;
        ImageSource LastImage = null;
        string[] LastStrArr = null;

        bool Handled = false;
        double LastTop = 0;


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClipListView.Items.Count >= 1)
            {
                ((UIElement)ClipListView.Items[0]).Focus();
                ClipListView.SelectedIndex = 0;
            }

            this.Activate();
        }


        #region [  Keyboard Hook 연결 / 해제  ]
        private IKeyboardMouseEvents GlobalHook;
        public void ConnectKeyHook()
        {
            GlobalHook = Hook.GlobalEvents();
            GlobalHook.KeyDown += GlobalHook_KeyDown;
        }

        public void DisConnectKeyHook()
        {
            GlobalHook.KeyDown -= GlobalHook_KeyDown;
            GlobalHook.Dispose();
        }
        #endregion


        #region [  KeyDown Event  ]

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Dispatcher.Invoke(() => { Disappear(); });
            }
        }
        OpenState process = OpenState.None;
        private void GlobalHook_KeyDown(object sender, f.KeyEventArgs e)
        {
            if (e.Alt)
            {
                process = OpenState.None;
                switch (e.KeyCode)
                {
                    case f.Keys.Left:
                        process = OpenState.MoveLeft;
                        break;
                    case f.Keys.Right:
                        process = OpenState.MoveRight;
                        break;
                    case f.Keys.Up:
                        process = OpenState.General;
                        break;
                }
                if (process == OpenState.None) return;

                e.SuppressKeyPress = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (this.Visibility == Visibility.Hidden)
                    {
                        this.Show();
                        SetWindow();
                        this.Activate();
                        Appear();
                    }
                    TBName.Text = ClipListView.SelectedIndex.ToString();
                });
            }
        }

        #endregion

        private void ClipListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ClipListView.Items.IndexOf(ClipListView.SelectedItem);
            if (index == -1) return;

            int itmWidth = 120 + 20;
            int AllWidth = ClipListView.Items.Count * itmWidth;
            int currOffset = index * itmWidth;
            int scrollOffset = (currOffset + itmWidth / 2) - ((int)this.Width / 2);

            var viewer = ClipListView.GetDescendantByType(typeof(AniScrollViewer)) as AniScrollViewer;

            if (scrollOffset > 0)
            {
                if (viewer.ScrollableWidth < scrollOffset) scrollOffset = (int)viewer.ScrollableWidth;
                viewer.ScrollToPosition(scrollOffset);
            }
            else
            {
                viewer.ScrollToPosition(0);
            }
        }


        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Disappear();
        }


        #region [  클립보드 처리 이벤트  ]

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (!Handled)
            {
                if (format == ClipboardFormat.Text)
                {
                    if (format != LastFormat || LastText != data.ToString())
                    {
                        string str = data.ToString();

                        if (string.IsNullOrEmpty(str)) return;
                        var itm = new StringClipItem(str);
                        ClipListView.Items.Add(itm);
                        itm.KeyDown += Itm_KeyDown;
                        itm.MouseDoubleClick += Itm_MouseDoubleClick;
                        LastText = str;
                    }
                }
                else if (format == ClipboardFormat.Bitmap)
                {
                    if (data is WinBitmap bmp)
                    {
                        uint crc32 = bmp.GetCRC32();

                        var thumbnail = bmp.ToThumbnail();
                        var item = new ImageClipItem(thumbnail, crc32);

                        if (ClipListView.Items
                            .Cast<ClipItem>()
                            .Where(child => child is ImageClipItem)
                            .Count(child => (child as ImageClipItem).CRC32 == crc32) > 0)
                        {
                            return;
                        }

                        BitmapDB.AddBitmap(bmp);

                        ClipListView.Items.Add(item);
                        item.KeyDown += Itm_KeyDown;
                        item.MouseDoubleClick += Itm_MouseDoubleClick;

                        LastImage = thumbnail;
                    }
                }
                else if (format == ClipboardFormat.FileDrop)
                {
                    string[] Data = (string[])data;
                    if (format != LastFormat || LastStrArr != Data)
                    {
                        var itm = new FileClipItem(Data);
                        ClipListView.Items.Add(itm);

                        itm.KeyDown += Itm_KeyDown;
                        itm.MouseDoubleClick += Itm_MouseDoubleClick;

                        LastStrArr = Data;
                    }
                }
            }
            else
            {
                Handled = false;
                if (format == ClipboardFormat.Text) LastText = data.ToString();
                TBName.Text = "Handled 처리됨";
            }


            LastFormat = format;
        }

        public void SetClipboard(object sender)
        {
            var item = sender as ClipItem;

            bool Unknown = false;

            if (item is ImageClipItem imageItem)
            {
                var bitmap = BitmapDB.GetBitmapFromCRC32(imageItem.CRC32);
                Handled = true;
                WinClipboard.SetImage(bitmap);
                bitmap.Dispose();
            }
            else if (item is StringClipItem stringItem)
            {
                Handled = true;
                if (string.IsNullOrEmpty(stringItem.Text)) return;
                WinClipboard.SetText(stringItem.Text);
            }
            else
            {
                Unknown = true;
            }
            if (!Unknown) (sender as ClipItem).ShowComplete();
        }

        #endregion

        #region [  아이템 처리 이벤트  ]

        private void Itm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetClipboard(sender);
                
            }
        }

        private void Itm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl)) ||
                e.Key == Key.Space ||
                e.Key == Key.Return)
            {
                SetClipboard(sender);
            }
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                Disappear();
            }
            
        }

        #endregion


        #region [  Appear / Disappear  ]

        bool FrmClosing = false;
        bool FrmOpening = false;
        public void Appear()
        {
            FrmOpening = true;
            DoubleAnimation oAnim = new DoubleAnimation();
            DoubleAnimation tAnim = new DoubleAnimation();

            oAnim.Completed += Appear_Comp;
            oAnim.From = 0; oAnim.To = 1.0;

            tAnim.From = LastTop + 60;
            tAnim.To = LastTop;

            oAnim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            tAnim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            oAnim.AccelerationRatio = 1.0;
            tAnim.AccelerationRatio = 1.0;


            oAnim.EasingFunction = new CircleEase();
            tAnim.EasingFunction = new CircleEase();

            
            this.BeginAnimation(OpacityProperty, oAnim);
            this.BeginAnimation(TopProperty, tAnim);
        }

        private void Appear_Comp(object sender, EventArgs e)
        {
            FrmOpening = false;
            if (ClipListView.Items.Count == 0) return;

            switch (process)
            {
                case OpenState.None:
                    return;
                case OpenState.General:
                    break;
                case OpenState.MoveLeft:
                    ClipListView.SelectedIndex--;

                    break;
                case OpenState.MoveRight:
                    ClipListView.SelectedIndex++;

                    break;
            }

            if (ClipListView.SelectedIndex == -1 && ClipListView.Items.Count > 0)
            {
                ClipListView.SelectedIndex = 0;
                (ClipListView.Items[0] as ListViewItem).Focus();
            }
            else
            {
                if (ClipListView.Items.Count > 0) (ClipListView.SelectedItem as ListViewItem).Focus();
            }
        }
        
        public void Disappear()
        {
            if (FrmClosing) return;

            FrmClosing = true;
            DoubleAnimation oAnim = new DoubleAnimation();
            DoubleAnimation tAnim = new DoubleAnimation();

            oAnim.Completed += Disappear_Comp;
            oAnim.From = 1.0; oAnim.To = 0.0;

            tAnim.From = LastTop;
            tAnim.To = LastTop + 60;

            oAnim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            tAnim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            oAnim.AccelerationRatio = 1.0;
            tAnim.AccelerationRatio = 1.0;


            oAnim.EasingFunction = new CircleEase();
            tAnim.EasingFunction = new CircleEase();
            
            this.BeginAnimation(OpacityProperty, oAnim);
            this.BeginAnimation(TopProperty, tAnim);
        }

        private void Disappear_Comp(object sender, EventArgs e)
        {
            FrmClosing = false;

            if (!FrmOpening) this.Hide();
        }

        #endregion

        public void SetWindow()
        {
            var point = f.Control.MousePosition;
            
            foreach (var screen in f.Screen.AllScreens)
            {
                if (screen.Bounds.Contains(point))
                {
                    this.Left = screen.Bounds.Left;

                    LastTop = screen.Bounds.Height - 60 - this.Height + screen.Bounds.Top;
                    this.Top = LastTop; 

                    this.Width = screen.Bounds.Width;

                    this.Focus();
                    this.Topmost = true;
                }
            }
        }
    }

    public enum OpenState
    {
        None,
        General,
        MoveLeft,
        MoveRight,
    }
}
