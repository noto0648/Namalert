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
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NamaAlert
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, IOwnerWindow
    {
        private SettingData _settingData;
        private IAlertSystem _alertSystem;


        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            string filePath = System.IO.Path.GetFullPath(Utils.GetAppDirectory() + "\\Settings\\Base.xml");
            _settingData = new SettingData();
            if (System.IO.File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SettingData));
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath, Encoding.UTF8))
                {
                    _settingData = (SettingData)serializer.Deserialize(reader);
                    reader.Close();
                }
            }

            TextBoxNiconicoMailAddress.Text = _settingData.NiconicoMailaddress;
            PasswordNiconico.Password = _settingData.NiconicoPassword;
            CheckBoxRunningStart.IsChecked = _settingData.AutoRun;
            LoadCommunities();

            if(_settingData.AutoRun)
            {
                NiconicoAlertStart();
            }
        }


        private void ButtonAlertStart_Click(object sender, RoutedEventArgs e)
        {
            NiconicoAlertStart();
        }


        public void NiconicoAlertStart()
        {
            _alertSystem = new AlertNiconico();
            Task.Factory.StartNew(() =>
            {
                _alertSystem.Start(this);
            });
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_alertSystem != null && _alertSystem.IsRunning)
            {
                _alertSystem.Stop();
            }

            string filePath = System.IO.Path.GetFullPath(Utils.GetAppDirectory() + "\\Settings\\Base.xml");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            XmlSerializer serializer = new XmlSerializer(typeof(SettingData));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false))
            {
                serializer.Serialize(writer, _settingData);
                writer.Close();
            }

            SaveCommunities();
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {

        }


        private void CommunityListAdd(ListView listView, NiconicoManager.NiconicoAlertItem n)
        {
            if (listView.DataContext == null)
            {
                listView.DataContext = new CollectionViewSource();
                (listView.DataContext as CollectionViewSource).Source = new ObservableCollection<NiconicoManager.NiconicoAlertItem>();
            }
            ((ObservableCollection<NiconicoManager.NiconicoAlertItem>)((CollectionViewSource)listView.DataContext).Source).Add(n);
        }

        public InfoWindow NewInfomation(int time, string url)
        {
            InfoWindow iw = InfomationManager.Instance.NewWindow(this, time, url);
            return iw;
        }

        public System.Windows.Threading.Dispatcher OwnerDispatcher
        {
            get { return this.Dispatcher; }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public SettingData Settings
        {
            get { return _settingData; }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _settingData.NiconicoMailaddress = TextBoxNiconicoMailAddress.Text;
            _settingData.NiconicoPassword = PasswordNiconico.Password;
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            ButtonImport.IsEnabled = false;
            Task.Factory.StartNew(() =>
            {
                NiconicoManager.Instance.Login(_settingData.NiconicoMailaddress, _settingData.NiconicoPassword);

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MakeCommunityList();
                }));
            });
            ButtonImport.IsEnabled = true;
        }

        private void LoadCommunities()
        {
            NiconicoManager.Instance.LoadCommunities();
            MakeCommunityList();
        }

        private void SaveCommunities()
        {
            NiconicoManager.Instance.SaveCommunities();
        }

        private void MakeCommunityList()
        {
            for (int i = 0; i < NiconicoManager.Instance.Communities.Count; i++)
            {
                NiconicoManager.NiconicoAlertItem alertItem = NiconicoManager.Instance.Communities[i];
                if(alertItem.IsChannel)
                {
                    CommunityListAdd(NiconicoChannelList, alertItem);
                }
                else
                {
                    CommunityListAdd(NiconicoCommunityList, alertItem);
                }
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState == System.Windows.WindowState.Minimized)
            {
                GetIn();
            }
            else
            {
                GetOut();
            }
        }

        public void GetIn()
        {
            this.ShowInTaskbar = false;
        }

        public void GetOut()
        {
            this.ShowInTaskbar = true;
        }

        private void CheckBoxRunningStart_Checked(object sender, RoutedEventArgs e)
        {
            _settingData.AutoRun = (bool)CheckBoxRunningStart.IsChecked;
        }

        private void ButtonAlertStop_Click(object sender, RoutedEventArgs e)
        {
            if (_alertSystem != null && _alertSystem.IsRunning)
            {
                _alertSystem.Stop();
            }
        }
    }
}
