// System

// User

// Others

// Define

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Media.Animation;

using ReClip.Control;
using ReClip.Util;
using ReClip.Setting;
using ReClip.Database;
using ReClip.Clips;
using ReClip.Windows;
using ReClip.HotKey;

using Gma.System.MouseKeyHook;

using f = System.Windows.Forms;
using WinClipboard = System.Windows.Forms.Clipboard;
using WinBitmap = System.Drawing.Bitmap;
using BitmapDB = ReClip.Database.BitmapCache;

namespace ReClip
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : LayeredWindow
    {
        IKeyboardMouseEvents hook = Hook.GlobalEvents();


        public MainWindow()
        {
            InitializeComponent();
            InitializeHotKeys();
            InitalizeNotifyIcon();

            Clipboard.Clear();

            ClipboardMonitor.Start();
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;

            ShowActivated = false;

            lvClip.Items.Clear();
            lvClip.SelectionChanged += ClipListView_SelectionChanged;
            lvClip.PreviewMouseLeftButtonDown += ClipListView_PreviewMouseLeftButtonDown;
            lvClip.MouseMove += ClipListView_MouseMove;


            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;

            SetWindow();
            this.Hide();
            //InitalizeNotifyIcon();

            itemDB = new ClipItemData();
            settingdb = new SettingDB();
            InitalizeItem();

            this.Closed += (sender, e) =>
            {
                ClipboardMonitor.Stop();
            };
            
            
            hook.MouseDown += Hook_MouseDown;
            hook.MouseUp += Hook_MouseUp;
        }
        

        private void Hook_MouseUp(object sender, f.MouseEventArgs e)
        {
            if (this.IsVisible)
                Disappear();
        }

        private void Hook_MouseDown(object sender, f.MouseEventArgs e)
        {
            
        }

        private void InitializeHotKeys()
        {
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "Visible",

                    Alt = true,
                    Key = f.Keys.Up,
                    Action = Act_Appear
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "InVisible1",

                    Key = f.Keys.Escape,
                    Action = Act_Disappear
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "InVisible2",

                    Alt = true,
                    Key = f.Keys.Down,
                    Action = Act_Disappear
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "MoveLeft",

                    Key = f.Keys.Left,
                    Action = Act_MoveLeft
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "MoveRight",

                    Key = f.Keys.Right,
                    Action = Act_MoveRight
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "DelItem",

                    Key = f.Keys.Delete,
                    Action = Act_Delete
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "CopyItem",

                    Alt = true,
                    Key = f.Keys.C,
                    Action = Act_Copy
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "PasteItem",

                    Control = true,
                    Key = f.Keys.V,
                    Action = Act_Paste
                });
        }

        private void Act_Paste(HotKeyData obj)
        {
            if (!this.IsVisible)
                return;

            SetClipboard(lvClip.SelectedItem);
            Disappear();
        }

        private void Act_Copy(HotKeyData data)
        {
            if (!this.IsVisible)
                return;

            SetClipboard(lvClip.SelectedItem);

            if (this.IsVisible)
                data.Prevent = true;
        }

        private void Act_Delete(HotKeyData data)
        {
            if (!this.IsVisible)
                return;

            LastIndex = lvClip.SelectedIndex;

            if (lvClip.SelectedItem is ClipItem itm)
            {
                itemDB.Remove(itm.Id);
            }
            lvClip.Items.Remove(lvClip.SelectedItem);

            LastFormat = ClipboardFormat.None;
            LastImage = null;
            LastStrArr = null;
            LastText = "";

            data.Prevent = true;
        }

        private void Act_MoveRight(HotKeyData data)
        {
            if (!this.IsVisible)
                return;

            int index = lvClip.SelectedIndex + 1;

            lvClip.SelectedIndex = index;

            ((ListViewItem)lvClip.SelectedItem)?.Focus();

            data.Prevent = true;
        }

        private void Act_MoveLeft(HotKeyData obj)
        {
            if (!this.IsVisible)
                return;

            int index = lvClip.SelectedIndex - 1;

            if (index < 0) index = 0;

            lvClip.SelectedIndex = index;
            
            ((ListViewItem)lvClip.SelectedItem)?.Focus();

            obj.Prevent = true;
        }

        public void Act_Appear(HotKeyData data)
        {
            if (!this.IsVisible) data.Prevent = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Show();
                SetWindow();
                Appear();
            });
        }

        public void Act_Disappear(HotKeyData data)
        {
            if (this.IsVisible) data.Prevent = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Disappear();
            });
            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
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
            foreach (Clip clip in itemDB.GetAllItem())
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
                try
                {
                    Item = new ImageClipItem(BitmapDB.GetBitmapFromCRC32(ImgClip.CRC32).ToThumbnail(), ImgClip.CRC32) { Id = ImgClip.Id };
                }
                catch (Exception)
                {
                    return;
                }

            }
            else if (clip is FileClip fileClip)
            {
                try
                {
                    Item = new FileClipItem(fileClip.Data) { Id = fileClip.Id };
                }
                catch (Exception)
                {
                    return;
                }
                
            }
            else
            {
                HandleAdd = false;
            }


            if (HandleAdd)
            {
                Item.MouseDoubleClick += Itm_MouseDoubleClick;
                TBInfo.Visibility = Visibility.Hidden;
                lvClip.Items.Add(Item);
            }


        }



        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        ClipboardFormat LastFormat = ClipboardFormat.None;

        ClipItemData itemDB;
        SettingDB settingdb;

        string LastText = null;
        ImageSource LastImage = null;
        string[] LastStrArr = null;

        bool Handled = false;
        double LastTop = 0;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (lvClip.Items.Count >= 1)
                ((UIElement)lvClip.Items[0])?.Focus();
            {
                lvClip.SelectedIndex = 0;
            }

            //this.Activate();
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

            ((INotifyCollectionChanged)lvClip.Items).CollectionChanged += Item_Changed;

            itms[0].Click += delegate (object o, EventArgs e) { new VersionWindow().ShowDialog(); };

            itms[1].Click += delegate (object o, EventArgs e) { settingdb.SetSetting(new SettingWindow(settingdb.GetSetting()).ShowDialog()); };

            itms[2].Click += delegate (object o, EventArgs e) { Environment.Exit(0); };

            icon = new f.NotifyIcon()
            {
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
            if (lvClip.Items.Count == 0)
            {
                lvClip.Visibility = Visibility.Hidden;
                TBInfo.Visibility = Visibility.Visible;
            }
            else
            {
                if (lvClip.SelectedIndex == -1)
                {
                    // TODO : 수정
                    if (LastIndex == lvClip.Items.Count) LastIndex--;
                    lvClip.SelectedIndex = LastIndex;
                    ((UIElement)lvClip.SelectedItem)?.Focus();
                }
            }
        }

        int LastIndex = 0;

        #endregion

        #region [  KeyDown Event  ]
        
        //int Counter = 0;

        


        #endregion

        private void ClipListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = lvClip.Items.IndexOf(lvClip.SelectedItem);
            if (index == -1) return;

            int itmWidth = 120 + 20;
            int AllWidth = lvClip.Items.Count * itmWidth;
            int currOffset = index * itmWidth;
            int scrollOffset = (currOffset + itmWidth / 2) - ((int)this.Width / 2);

            var viewer = lvClip.GetDescendantByType(typeof(AniScrollViewer)) as AniScrollViewer;

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
                        string text = data.ToString();

                        long Key = KeyGenerator.GenerateKey();

                        if (string.IsNullOrEmpty(text)) return;
                        Item = new StringClipItem(text);
                        Item.Id = Key;

                        lvClip.Items.Add(Item);

                        itemDB.Add(new StringClip(text, Key));

                        LastText = text;
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
                        
                        if (lvClip.Items
                            .Cast<ClipItem>()
                            .Where(child => child is ImageClipItem)
                            .Count(child => (child as ImageClipItem).CRC32 == crc32) > 0)
                        {
                            return;
                        }

                        BitmapDB.AddBitmap(bmp);

                        itemDB.Add(new ImageClip(crc32, Key));

                        lvClip.Items.Add(Item);
                        LastImage = thumbnail;
                    }
                }
                else if (format == ClipboardFormat.FileDrop)
                {
                    if (data is string[] files)
                    {
                        if (format != LastFormat || !Enumerable.SequenceEqual(LastStrArr, files))
                        {
                            long Key = KeyGenerator.GenerateKey();

                            Item = new FileClipItem(files);
                            Item.Id = Key;

                            lvClip.Items.Add(Item);
                            var itm = new FileClip(files, Key);

                            itemDB.Add(itm);

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
                        Item.MouseDoubleClick += Itm_MouseDoubleClick;
                    }
                    catch (Exception) { }
                }
                if (!UnknownFormat && lvClip.Items.Count >= 1)
                {
                    TBInfo.Visibility = Visibility.Hidden;
                    lvClip.Visibility = Visibility.Visible;
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
                if (string.IsNullOrEmpty(stringItem.Text))
                    return;
                WinClipboard.SetText(stringItem.Text);
            }
            else if (item is FileClipItem fileItem)
            {
                Handled = true;

                StringCollection coll = new StringCollection();
                coll.AddRange(fileItem.FilePaths);

                WinClipboard.SetFileDropList(coll);
            }
            else
            {
                Unknown = true;
            }
            if (!Unknown)
                (sender as ClipItem).ShowComplete();
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

        OpenState process = OpenState.None;

        private void Appear_Comp(object sender, EventArgs e)
        {
            FrmOpening = false;
            if (lvClip.Items.Count == 0)
                return;

            switch (process)
            {
                case OpenState.None:
                    return;
                case OpenState.General:
                    break;
                case OpenState.MoveLeft:
                    lvClip.SelectedIndex--;

                    break;
                case OpenState.MoveRight:
                    lvClip.SelectedIndex++;

                    break;
            }

            if (lvClip.SelectedIndex == -1 && lvClip.Items.Count > 0)
            {
                lvClip.SelectedIndex = 0;
                (lvClip.Items[0] as ListViewItem).Focus();
            }
            else
            {
                if (lvClip.Items.Count > 0) (lvClip.SelectedItem as ListViewItem)?.Focus();
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

            if (!FrmOpening)
                this.Hide();
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
