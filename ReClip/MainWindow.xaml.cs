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
    

    public partial class MainWindow : LayeredWindow
    {
        IKeyboardMouseEvents hook = Hook.GlobalEvents();
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeHotKeys();
            InitializeNotifyIcon();

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
            
            itemDB = new ClipItemData();
            settingdb = new SettingDB();
            InitializeItem();

            this.Closed += (sender, e) =>
            {
                ClipboardMonitor.Stop();
            };
            
            
            hook.MouseDown += Hook_MouseDown;
            hook.MouseUp += Hook_MouseUp;
            hook.KeyDown += Hook_KeyDown;
            this.PreviewMouseDown += MainWindow_MouseDown;
        }
        bool WindowClicked = false;
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowClicked = true;
        }

        private void Hook_KeyDown(object sender, f.KeyEventArgs e)
        {
            if (e.Shift || InputWord(e.KeyCode))
            {
                if (this.IsVisible)
                    Disappear();
            }
        }

        public bool InputWord(f.Keys key)
        {
            int k = (int)key;
            
            if ((k >= 65 && k <= 90) || 
                (k >= 96 && k <= 111) || 
                (k >=112 && k <= 135) || 
                (k >= 48 && k <= 56) || 
                (k == (int)f.Keys.Tab))
            {
                return true;
            }
            return false;
        }

        private void Hook_MouseUp(object sender, f.MouseEventArgs e)
        {
            if (this.IsVisible && !WindowClicked)
                Disappear();
            WindowClicked = false;
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
                    Name = "MoveUp",

                    Key = f.Keys.Up,
                    Action = Act_Prevent
                });
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "MoveDown",

                    Key = f.Keys.Down,
                    Action = Act_Prevent
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
            HotKeyManager.AddHotKey(
                new HotKeyData()
                {
                    Name = "",
                    Control = true,
                    
                    Key = f.Keys.D,
                    Action = Act_Debug
                });
        }

        private void Act_Prevent(HotKeyData obj)
        {
            if (this.IsVisible)
                obj.Prevent = true;
        }

        private void Act_Debug(HotKeyData obj)
        {
            obj.Prevent = true;
            GetCurrentOffset();
            
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

            lastFormat = ClipboardFormat.None;
            lastImage = null;
            lastStrArr = null;
            lastText = "";

            data.Prevent = true;
        }

        private void Act_MoveRight(HotKeyData data)
        {
            if (!this.IsVisible)
                return;

            int index = lvClip.SelectedIndex + 1;

            lvClip.SelectedIndex = index;
            ChangeFormatText();

            ((ListViewItem)lvClip.SelectedItem)?.Focus();
            data.Prevent = true;
        }

        private void Act_MoveLeft(HotKeyData data)
        {
            if (!this.IsVisible)
                return;

            int index = lvClip.SelectedIndex - 1;

            if (index < 0) index = 0;
            
            int lastIndex = lvClip.SelectedIndex;

            lvClip.SelectedIndex = index;
        
            if (lastIndex == lvClip.SelectedIndex)
            {
                ClipListView_SelectionChanged(lvClip.SelectedItem, null);
            }

            ChangeFormatText();

            ((ListViewItem)lvClip.SelectedItem)?.Focus();
            data.Prevent = true;
        }

        public void ChangeFormatText()
        {
            //if (lvClip.SelectedItem is StringClipItem strItem)
            //{
            //    runFormat.Text = "텍스트";
            //}
            //else if (lvClip.SelectedItem is FileClipItem fileItem)
            //{
            //    runFormat.Text = "파일 목록";
            //}
            //else if (lvClip.SelectedItem is ImageClipItem imgItem)
            //{
            //    runFormat.Text = "이미지";
            //}

            if (lvClip.SelectedItem is ClipItem clipitem)
            {
                SetDate(clipitem.Time);
            }
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

        private void InitializeItem()
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
                Item = new StringClipItem(strClip.Data)
                {
                    Id = strClip.Id,
                    Time = strClip.Time
                };
            }
            else if (clip is ImageClip ImgClip)
            {
                try
                {
                    Item = new ImageClipItem(BitmapDB.GetBitmapFromCRC32(ImgClip.CRC32).ToThumbnail(), ImgClip.CRC32)
                    {
                        Id = ImgClip.Id,
                        Time = ImgClip.Time
                    };
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
                    Item = new FileClipItem(fileClip.Data) {
                        Id = fileClip.Id,
                        Time = fileClip.Time
                    };
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
                Item.PreviewMouseDown += Item_MouseDown;
                Item.MouseDoubleClick += Itm_MouseDoubleClick;
                TBInfo.Visibility = Visibility.Hidden;
                lvClip.Items.Add(Item);
                lvClip.SelectedItem = Item;
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Control ctrl)
            {
                if (ctrl.Parent is ListView lv)
                {
                    lv.SelectedItem = ctrl;
                }
            }            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        ClipboardFormat lastFormat = ClipboardFormat.None;

        ClipItemData itemDB;
        SettingDB settingdb;

        string lastText = null;
        ImageSource lastImage = null;
        string[] lastStrArr = null;

        bool handled = false;
        double lastTop = 0;

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
        public void InitializeNotifyIcon()
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
        
        private void ClipListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = lvClip.Items.IndexOf(lvClip.SelectedItem);
            if (index == -1) return;

            int itmWidth = 120 + 20;
            int allWidth = lvClip.Items.Count * itmWidth;
            int currOffset = index * itmWidth;
            int scrollOffset = (currOffset + itmWidth / 2) - ((int)this.Width / 2);

            var viewer = lvClip.GetDescendantByType(typeof(AniScrollViewer)) as AniScrollViewer;

            if (viewer.ScrollableWidth < scrollOffset)
                scrollOffset = (int)viewer.ScrollableWidth;
            else if (scrollOffset < 0)
                scrollOffset = 0;
            int differ = (int)viewer.HorizontalOffset - scrollOffset; 

            viewer.ScrollToPosition(scrollOffset);

            var thick = infoLower.Margin;
            thick.Left = GetCurrentOffset() + differ + (this.Width - lvClip.ActualWidth) / 2;
            infoLower.Margin = thick;

            var thick2 = infoUpper.Margin;
            thick2.Left = GetCurrentOffset() + differ - infoUpper.ActualWidth / 2 + (this.Width - lvClip.ActualWidth) / 2;
            if (thick2.Left < 0)
                thick2.Left = 0;
            else if ((thick2.Left + infoUpper.Width > this.Width))
                thick2.Left = this.Width - infoUpper.Width + (this.Width - lvClip.ActualWidth) / 2;

            infoUpper.Margin = thick2;
        }


        #region [  클립보드 처리 이벤트  ]


        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            var currentSetting = settingdb.GetSetting();
            if (!currentSetting.ClipboardSaveEnable) return;

            if ((currentSetting.ExceptFileItem && format == ClipboardFormat.FileDrop) ||
                (currentSetting.ExceptImageItem && format == ClipboardFormat.Bitmap) ||
                (currentSetting.ExceptTextItem && format == ClipboardFormat.Text)) return;

            if (!handled)
            {
                bool UnknownFormat = false;
                ClipItem Item = null;

                if (format == ClipboardFormat.Text)
                {
                    if (format != lastFormat || lastText != data.ToString())
                    {
                        string text = data.ToString();

                        long Key = KeyGenerator.GenerateKey();

                        if (string.IsNullOrEmpty(text)) return;
                        Item = new StringClipItem(text);
                        Item.Id = Key;
                        
                        itemDB.Add(new StringClip(text, Key));

                        lastText = text;
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

                        
                        lastImage = thumbnail;
                    }
                }
                else if (format == ClipboardFormat.FileDrop)
                {
                    if (data is string[] files)
                    {
                        if (format != lastFormat || !Enumerable.SequenceEqual(lastStrArr, files))
                        {
                            long Key = KeyGenerator.GenerateKey();

                            Item = new FileClipItem(files);
                            Item.Id = Key;

                            var itm = new FileClip(files, Key);

                            itemDB.Add(itm);

                            lastStrArr = files;
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
                        Item.Time = DateTime.Now;
                        lvClip.Items.Add(Item);
                        lvClip.SelectedItem = Item;
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
                handled = false;
                if (format == ClipboardFormat.Text) lastText = data.ToString();
            }

            lastFormat = format;
        }

        public void SetClipboard(object sender)
        {
            var item = sender as ClipItem;

            bool Unknown = false;

            if (item is ImageClipItem imageItem)
            {
                var bitmap = BitmapDB.GetBitmapFromCRC32(imageItem.CRC32);
                handled = true;
                WinClipboard.SetImage(bitmap);
                bitmap.Dispose();
            }
            else if (item is StringClipItem stringItem)
            {
                handled = true;
                if (string.IsNullOrEmpty(stringItem.Text))
                    return;
                WinClipboard.SetText(stringItem.Text);
            }
            else if (item is FileClipItem fileItem)
            {
                handled = true;

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

            tAnim.From = lastTop + 60;
            tAnim.To = lastTop;

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

            tAnim.From = lastTop;
            tAnim.To = lastTop + 60;

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
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListView listView = sender as ListView;
                ClipItem listViewItem = FindAnchestor<ClipItem>((UIElement)e.OriginalSource);

                if (listViewItem == null) return;
                
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
                DataObject dragData = new DataObject(typestring, setData);
                try { DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy); }
                catch (Exception) { }

            }
        }

        public double GetCurrentOffset()
        {
            double offset = ((lvClip.SelectedIndex + 1) * 140) - 70;
            var viewer = lvClip.GetDescendantByType(typeof(AniScrollViewer)) as AniScrollViewer;

            double finalOffset = offset - viewer.HorizontalOffset;

            return finalOffset;
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

        Point startPoint;
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

                    lastTop = screen.Bounds.Height - 60 - this.Height + screen.Bounds.Top;
                    this.Top = lastTop;

                    this.Width = screen.Bounds.Width;
                }
            }
        }

        public void SetDate(DateTime time)
        {
            runSaveTime.Text = time.ToString();
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
