using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NamaAlert
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private NotifyIconWPF _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _notifyIcon = new NotifyIconWPF();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _notifyIcon.Dispose();
        }
    }
}
