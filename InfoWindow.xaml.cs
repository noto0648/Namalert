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

namespace NamaAlert
{
    /// <summary>
    /// InfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InfoWindow : MahApps.Metro.Controls.MetroWindow
    {
        private string _description;
        private string _infoTitle;
        private ImageSource _infoImage;
        private string _url;

        public string InfomationDescription { get { return _description; } set
            {
                _description = value;
                LabelDescription.Text = _description;
            }
        }

        public string InfomationTitle
        {
            get { return _infoTitle; }
            set
            {
                _infoTitle = value;
                LabelTitle.Content = _infoTitle;
            }
        }

        public ImageSource InfomationImage
        {
            get { return _infoImage; }
            set
            {
                _infoImage = value;
                ImageViewer.Source = _infoImage;
            }

        }

        public InfoWindow(int time = 10000, string url = null)
        {
            InitializeComponent();

            IntPtr hWind = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
            Win32.SetWindowLong(hWind, -16, Win32.WS_SYSMENU);
            _url = url;
            Task.Factory.StartNew(() => {
                System.Threading.Thread.Sleep(time);
                this.Dispatcher.BeginInvoke(new Action(() => { this.Close(); }));
            });
        }

        private void InfoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InfomationManager.Instance.RemoveWindow(this);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_url != null)
            {
                System.Diagnostics.Process.Start(@_url);

                this.Close();
            }
        }

    }
}
