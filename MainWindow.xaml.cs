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

namespace NamaAlert
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, IOwnerWindow
    {
        IAlertSystem _alertSystem;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAlertStart_Click(object sender, RoutedEventArgs e)
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
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            //NewInfomation(10000, @"http://lkappa.zashiki.com/nap.html").Show();
        }

        public InfoWindow NewInfomation(int time, string url)
        {
            InfoWindow iw = InfomationManager.Instance.NewWindow(this, time, url);
            return iw;
        }

        System.Windows.Threading.Dispatcher Dispatcher
        {
            get { return this.Dispatcher; }
        }

    }
}
