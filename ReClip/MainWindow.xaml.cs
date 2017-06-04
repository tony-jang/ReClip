using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using f=System.Windows.Forms;
using System.Windows.Media.Animation;
using ReClip.Control;
using ReClip.Util;
using Gma.System.MouseKeyHook;

using WinClipboard = System.Windows.Forms.Clipboard;
using WinBitmap = System.Drawing.Bitmap;
using BitmapDB = ReClip.Database.BitmapCache;
using ReClip.Setting;
using System.Collections.Specialized;
using ReClip.Database;
using ReClip.Clips;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ReClip.Windows;

namespace ReClip
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : LayeredWindow
    {

        public MainWindow()
        {
            InitializeComponent();

            Clipboard.Clear();

            ClipboardMonitor.Start();
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;

            ShowActivated = false;

            ConnectKeyHook();

            ClipListView.Items.Clear();
            ClipListView.SelectionChanged += ClipListView_SelectionChanged;
            ClipListView.PreviewMouseLeftButtonDown += ClipListView_PreviewMouseLeftButtonDown;
            ClipListView.MouseMove += ClipListView_MouseMove;

            this.Deactivated += MainWindow_Deactivated;
            this.Loaded += MainWindow_Loaded;
            this.PreviewKeyDown += MainWindow_KeyDown;
            this.Closing += MainWindow_Closing;

            SetWindow();
            this.Hide();
            InitalizeNotifyIcon();

            Itemdb = new ClipItemData();
            settingdb = new SettingDB();
            InitalizeItem();
            
            this.Closed += delegate (object sender, EventArgs e) {
                ClipboardMonitor.Stop();
                DisConnectKeyHook();
            };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //debugtb.Text += " " + msg.ToString();
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }
            else
            {
                return IntPtr.Zero;
            }

            
        }
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 0x0003;

        private void InitalizeItem()
        {
            foreach(Clip clip in Itemdb.GetAllItem())
            {
                AddClip(clip);
            }
        }

        public void AddClip(Clip clip)
        {
            bool HandleAdd = true;
            ClipItem Item = null;
            if (clip is StringClip strClip)
            {
                Item = new StringClipItem(strClip.Data) { Id = strClip.Id };
            }
            else if (clip is ImageClip ImgClip)
            {
                Item = new ImageClipItem(BitmapDB.GetBitmapFromCRC32(ImgClip.CRC32).ToThumbnail(), ImgClip.CRC32) { Id = ImgClip.Id };
            }
            else if (clip is FileClip fileClip)
            {
                Item = new FileClipItem(fileClip.Data) { Id = fileClip.Id };
            }
            else
            {
                HandleAdd = false;
            }


            if (HandleAdd)
            {
                Item.KeyDown += Itm_KeyDown;
                Item.MouseDoubleClick += Itm_MouseDoubleClick;
                TBInfo.Visibility = Visibility.Hidden;
                ClipListView.Items.Add(Item);
            }
            

        }



        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        ClipboardFormat LastFormat = ClipboardFormat.None;

        ClipItemData Itemdb;
        SettingDB settingdb;

        string LastText = null;
        ImageSource LastImage = null;
        string[] LastStrArr = null;

        bool Handled = false;
        double LastTop = 0;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClipListView.Items.Count >= 1)
                ((UIElement)ClipListView.Items[0])?.Focus();
            {
                ClipListView.SelectedIndex = 0;
            }

            this.Activate();
        }


        #region [  Initalize NotifyIcon  ]
        
        f.NotifyIcon icon;
        public void InitalizeNotifyIcon()
        {
            var menu = new f.ContextMenu();

            f.MenuItem[] itms = { new f.MenuItem() { Index = 0, Text = "버전 정보" },
                                  new f.MenuItem() { Index = 1, Text = "설정"},
                                  new f.MenuItem() { Index = 2, Text = "종료"}};
            
            menu.MenuItems.AddRange(itms);

            ((INotifyCollectionChanged)ClipListView.Items).CollectionChanged += Item_Changed;

            itms[0].Click += delegate (object o, EventArgs e) { new VersionWindow().ShowDialog(); };

            itms[1].Click += delegate (object o, EventArgs e) { new SettingWindow(EnvironmentSetting.Default).ShowDialog(); };

            itms[2].Click += delegate (object o, EventArgs e) { Environment.Exit(0); };

            icon = new f.NotifyIcon() {
                ContextMenu = menu,
                Text = "Re:Clip - Running",
                Visible = true,
                Icon = Properties.Resources.ReClipIcon,
                BalloonTipTitle = "Re:Clip",
                BalloonTipText = "Re:Clip Running!"
            };
            
            icon.ShowBalloonTip(1000);
        }

        private void Item_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ClipListView.Items.Count == 0)
            {
                ClipListView.Visibility = Visibility.Hidden;
                TBInfo.Visibility = Visibility.Visible;
            }
            else
            {
                if (ClipListView.SelectedIndex == -1)
                {
                    // TODO : 수정
                    if (LastIndex == ClipListView.Items.Count) LastIndex--;
                    ClipListView.SelectedIndex = LastIndex;
                    ((UIElement)ClipListView.SelectedItem)?.Focus();
                }
            }
        }

        int LastIndex = 0;

        #endregion


        #region [  Keyboard Hook 연결 / 해제  ]
        private IKeyboardMouseEvents GlobalHook;
        public void ConnectKeyHook()
        {
            GlobalHook = Hook.GlobalEvents();
            GlobalHook.KeyDown += GlobalHook_KeyDown;
            GlobalHook.KeyUp += GlobalHook_KeyUp;
        }


        public void DisConnectKeyHook()
        {
            GlobalHook.KeyDown -= GlobalHook_KeyDown;
            GlobalHook.KeyUp -= GlobalHook_KeyUp;
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
            else if (Keyboard.IsKeyDown(Key.Down) && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                Disappear();
            }
            else if (e.Key == Key.S && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                SettingWindow sw = new SettingWindow(settingdb.GetSetting());
                settingdb.SetSetting(sw.ShowDialog());
            }
            else if (e.Key == Key.Left || e.Key == Key.Right)
            {
                ((ListViewItem)ClipListView.SelectedItem)?.Focus();
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
            else if (e.Key == Key.Delete)
            {
                LastIndex = ClipListView.SelectedIndex;

                if (sender is ClipItem itm)
                {
                    Itemdb.Remove(itm.Id);
                }

                

                ClipListView.Items.Remove(sender);

                LastFormat = ClipboardFormat.None;
                LastImage = null;
                LastStrArr = null;
                LastText = "";
            }
        }

        //int Counter = 0;

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
                });
            }
        }
        
        private void GlobalHook_KeyUp(object sender, f.KeyEventArgs e)
        {
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
            var CurrentSetting = settingdb.GetSetting();
            if (!CurrentSetting.ClipboardSaveEnable) return;

            if ((CurrentSetting.ExceptFileItem && format == ClipboardFormat.FileDrop) ||
                (CurrentSetting.ExceptImageItem && format == ClipboardFormat.Bitmap) ||
                (CurrentSetting.ExceptTextItem && format == ClipboardFormat.Text)) return;

            if (!Handled)
            {
                bool UnknownFormat = false;
                ClipItem Item = null;

                if (format == ClipboardFormat.Text)
                {
                    if (format != LastFormat || LastText != data.ToString())
                    {
                        string Text = data.ToString();

                        long Key = KeyGenerator.GenerateKey();

                        if (string.IsNullOrEmpty(Text)) return;
                        Item = new StringClipItem(Text);
                        Item.Id = Key;

                        ClipListView.Items.Add(Item);

                        Itemdb.Add(new StringClip(Text, Key));

                        LastText = Text;
                    }
                }
                else if (format == ClipboardFormat.Bitmap)
                {
                    if (data is WinBitmap bmp)
                    {
                        uint crc32 = bmp.GetCRC32();
                        ImageSource thumbnail = bmp.ToThumbnail();

                        long Key = KeyGenerator.GenerateKey();
                        Item = new ImageClipItem(thumbnail, crc32);
                        Item.Id = Key;
                        

                        if (ClipListView.Items
                            .Cast<ClipItem>()
                            .Where(child => child is ImageClipItem)
                            .Count(child => (child as ImageClipItem).CRC32 == crc32) > 0)
                        {
                            return;
                        }
                        
                        BitmapDB.AddBitmap(bmp);



                        Itemdb.Add(new ImageClip(crc32, Key));

                        ClipListView.Items.Add(Item);
                        LastImage = thumbnail;
                    }
                }
                else if (format == ClipboardFormat.FileDrop)
                {
                    if (data is string[] files)
                    {
                        if (format != LastFormat || LastStrArr != files)
                        {
                            long Key = KeyGenerator.GenerateKey();

                            Item = new FileClipItem(files);
                            Item.Id = Key;

                            ClipListView.Items.Add(Item);
                            var itm = new FileClip(files, Key);

                            Itemdb.Add(itm);

                            LastStrArr = files;
                        }
                    }
                }
                else
                {
                    UnknownFormat = true;
                }

                if (!UnknownFormat)
                {
                    try
                    {
                        Item.KeyDown += Itm_KeyDown;
                        Item.MouseDoubleClick += Itm_MouseDoubleClick;
                    }
                    catch (Exception) { }
                }
                if (!UnknownFormat && ClipListView.Items.Count >= 1)
                {
                    TBInfo.Visibility = Visibility.Hidden;
                    ClipListView.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Handled = false;
                if (format == ClipboardFormat.Text) LastText = data.ToString();
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
                if (ClipListView.Items.Count > 0) (ClipListView.SelectedItem as ListViewItem)?.Focus();
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


        #region [  Drag & Drop  ]

        private void ClipListView_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            System.Windows.Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ClipItem listViewItem =
                    FindAnchestor<ClipItem>((UIElement)e.OriginalSource);

                if (listViewItem == null) return;

                // Find the data behind the ListViewItem
                ClipItem clipItm = (ClipItem)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                object setData = null;
                string typestring = "";

                if (clipItm.GetType() == typeof(StringClipItem))
                {
                    setData = ((StringClipItem)clipItm).Text;
                    typestring = DataFormats.Text.ToString();
                }
                else if (clipItm.GetType() == typeof(FileClipItem))
                {
                    setData = ((FileClipItem)clipItm).FilePaths;
                    typestring = DataFormats.FileDrop.ToString();
                }
                else if (clipItm.GetType() == typeof(ImageClipItem))
                {
                    setData = BitmapDB.GetBitmapFromCRC32(((ImageClipItem)clipItm).CRC32);
                    typestring = DataFormats.Bitmap.ToString();
                }
                else
                {
                    return;
                }
                // Initialize the drag & drop operation
                DataObject dragData = new DataObject(typestring, setData);
                try { DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy); }
                catch (Exception) { }
                
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        System.Windows.Point startPoint;

        private void ClipListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
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
