using Microsoft.Win32;
using ReClip.Control;
using ReClip.Setting;
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
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        EnvironmentSetting Temp, NotSave;
        public SettingWindow(EnvironmentSetting setting)
        {
            InitializeComponent();
            Temp = setting.Clone();
            NotSave = setting;
            Sync();
            SyncItem();

            this.KeyDown += SettingWindow_KeyDown;
        }

        private void SettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) btnCancel_Click(this, null);
        }

        public void Sync()
        {
            cbClipboard.IsChecked = Temp.ClipboardSaveEnable;
            cbThumbnailStretch.IsChecked = Temp.StrectchThumbnail;

            cbSvRecents.SelectedIndex = SaveCountsToIndex();
            
            cbSaveExceptImg.IsChecked = Temp.ExceptImageItem;
            cbSaveExceptText.IsChecked = Temp.ExceptTextItem;
            cbSaveExceptFile.IsChecked = Temp.ExceptFileItem;

            cbStartup.IsChecked = Temp.SetStartupProgram;
        }

        public void SyncItem()
        {
            ItemCount.Text = ((MainWindow)Application.Current.MainWindow).lvClip.Items.Count.ToString();
        }

        /// <summary>
        /// 취소와 같은 윈도우를 닫을 경우 기존 Setting을 반환 아닐 경우 변경된 걸 반환
        /// </summary>
        /// <returns></returns>
        public new EnvironmentSetting ShowDialog()
        {
            base.ShowDialog();

            if (SuccessfulHandled)
            {
                if (Temp.SetStartupProgram)
                    reg.SetValue("ReClip", $@"""{AppDomain.CurrentDomain.BaseDirectory}{AppDomain.CurrentDomain.FriendlyName}"" -bystartup");
                else
                    reg.DeleteValue("ReClip");

                return Temp;
            }
            else
            {
                return NotSave;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            Sync();
        }

        bool SuccessfulHandled = false;


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NotSave.Equals(Temp))
            {
                this.Close();
            }
            else if (MessageBox.Show("저장되지 않은 내용이 있습니다. 저장하지 않고 닫으시겠습니까?", "저장되지 않은 내용", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            SuccessfulHandled = true;
        }
        

        MainWindow wdw = ((MainWindow)Application.Current.MainWindow);


        private void btnDelAll_Click(object sender, RoutedEventArgs e)
        {
            if (wdw.lvClip.Items.Count == 0)
            {
                MessageBox.Show("삭제할 아이템이 없습니다", "Re:Clip 삭제 확인");
                return;
            }
            if (MessageBox.Show($"정말 삭제하시겠습니까? 모든 아이템이 Re:Clip에서 영구적으로 삭제됩니다.",
                    "Re:Clip 삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;


            wdw.lvClip.Items.Clear();
        }

        private void btnDelTypes_Click(object sender, RoutedEventArgs e)
        {
            Type FindType;
            string FindTypeString;

            string name = ((System.Windows.Controls.Control)sender).Name;

            if (name.Contains("Text"))
            {
                FindType = typeof(StringClipItem);
                FindTypeString = "텍스트";
            }
            else if (name.Contains("Image"))
            {
                FindType = typeof(ImageClipItem);
                FindTypeString = "이미지";
            }
            else if (name.Contains("File"))
            {
                FindType = typeof(FileClipItem);
                FindTypeString = "파일";
            }
            else
            {
                FindType = typeof(ClipItem);
                FindTypeString = "모든";
            }


            var itm = wdw.lvClip.Items.Cast<ClipItem>()
                                      .Where(i => FindType == i.GetType())
                                      .ToList();

            if (itm.Count == 0)
            {
                MessageBox.Show("삭제할 아이템이 없습니다", "Re:Clip 삭제 확인");
                return;
            }

            if (MessageBox.Show($"정말 삭제하시겠습니까? {itm.Count}개의 {FindTypeString} 아이템이 Re:Clip에서 영구적으로 삭제됩니다.",
                "Re:Clip 삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

            itm.ForEach(i=> wdw.lvClip.Items.Remove(i));

            SyncItem();
        }

        private void btnDelRecentItem_Click(object sender, RoutedEventArgs e)
        {
            int DeleteCount = int.Parse(((System.Windows.Controls.Control)sender).Tag.ToString());
            int Count = wdw.lvClip.Items.Count;

            if (Count > DeleteCount)
            {
                if (MessageBox.Show($"정말 삭제하시겠습니까? 오래된 {DeleteCount}개의 아이템이 Re:Clip에서 영구적으로 삭제됩니다.",
                    "Re:Clip 삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            else
            {
                if (Count == 0)
                {
                    MessageBox.Show("삭제할 아이템이 없습니다", "Re:Clip 삭제 확인");
                    return;
                }
                if (MessageBox.Show($"정말 삭제하시겠습니까? 모든 아이템이 Re:Clip에서 영구적으로 삭제됩니다.",
                    "Re:Clip 삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }

            for (int i = 0; i < DeleteCount; i++)
            {
                if (wdw.lvClip.Items.Count == 0) break;
                wdw.lvClip.Items.RemoveAt(0);
            }
            SyncItem();
        }

        private void cbClipboard_Checked(object sender, RoutedEventArgs e)
        {
            if (Temp != null) Temp.ClipboardSaveEnable = cbClipboard.IsChecked.Value;
        }

        private void cbThumbnailStretch_Checked(object sender, RoutedEventArgs e)
        {
            if (Temp != null)
                Temp.StrectchThumbnail = cbThumbnailStretch.IsChecked.Value;
        }

        private void cbSvRecents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Temp != null) Temp.SaveCount = IndexToSaveCounts();
        }

        public int SaveCountsToIndex()
        {
            if (Temp != null)
            {
                switch (Temp.SaveCount)
                {
                    case ItemSaveTypes.None: return -1;
                    case ItemSaveTypes.Over10Items: return 0;
                    case ItemSaveTypes.Over20Items: return 1;
                    case ItemSaveTypes.Over30Items: return 2;
                    case ItemSaveTypes.Over50Items: return 3;
                    case ItemSaveTypes.Over75Items: return 4;
                    case ItemSaveTypes.Over100Items: return 5;
                    case ItemSaveTypes.InitifiteItems: return 6;
                    default: return -1;
                }
            }
            return -1;
        }

        private void cbSaveExceptImg_Checked(object sender, RoutedEventArgs e)
        {
            if (Temp != null)
                Temp.ExceptImageItem = cbSaveExceptImg.IsChecked.Value;
        }

        private void cbSaveExceptText_Checked(object sender, RoutedEventArgs e)
        {
            if (Temp != null)
                Temp.ExceptTextItem = cbSaveExceptText.IsChecked.Value;
        }

        private void cbSaveExceptFile_Checked(object sender, RoutedEventArgs e)
        {
            if (Temp != null)
                Temp.ExceptFileItem = cbSaveExceptFile.IsChecked.Value;
        }


        static RegistryKey regKey = Registry.CurrentUser;
        static RegistryKey reg = regKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
        private void cbStartup_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Temp != null)
                Temp.SetStartupProgram = cbStartup.IsChecked.Value;
            
        }

        public ItemSaveTypes IndexToSaveCounts()
        {
            switch (cbSvRecents.SelectedIndex)
            {
                case 0: return ItemSaveTypes.Over10Items;
                case 1: return ItemSaveTypes.Over20Items;
                case 2: return ItemSaveTypes.Over30Items;
                case 3: return ItemSaveTypes.Over50Items;
                case 4: return ItemSaveTypes.Over75Items;
                case 5: return ItemSaveTypes.Over100Items;
                case 6: return ItemSaveTypes.InitifiteItems;
                default: return ItemSaveTypes.None;
            }
        }
    }

    public enum SyncItems
    {
        None = 0,
        Basis = 1,
        Key = 2,
        Clip = 4,
        All = 7,
    }
}
