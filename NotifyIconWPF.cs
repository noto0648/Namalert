using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms;
namespace NamaAlert
{
    public class NotifyIconWPF : Component
    {
        private ContextMenuStrip _contextMenu;
        private NotifyIcon _notifyIcon;
        private ToolStripMenuItem MenuItemOpen;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MenuItemExit;
        private IContainer components;

        public string ToolTipText { get { return _notifyIcon.Text; } set { _notifyIcon.Text = value; } }
        private MainWindow _mainWindow = new MainWindow();

        public NotifyIconWPF()
        {
            InitializeComponent();
            ToolTipText = @"Namalert";
            MenuItemOpen.Click += MenuItemOpen_Click;
            MenuItemExit.Click += MenuItemExit_Click;

            _mainWindow.Show();
            _mainWindow.Initialize();
            _mainWindow.Hide();
            _mainWindow.GetIn();
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            ShowWindow(_mainWindow);
        }

        public NotifyIconWPF(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void ShowWindow(Window win)
        {
            if (win.WindowState == System.Windows.WindowState.Minimized)
                win.WindowState = System.Windows.WindowState.Normal;

            win.Show();
            win.Activate();
            win.ShowInTaskbar = true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyIconWPF));
            this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.MenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._contextMenu.SuspendLayout();
            // 
            // _contextMenu
            // 
            this._contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemOpen,
            this.toolStripSeparator1,
            this.MenuItemExit});
            this._contextMenu.Name = "ContextMenu";
            this._contextMenu.Size = new System.Drawing.Size(191, 48);
            // 
            // _notifyIcon
            // 
            this._notifyIcon.ContextMenuStrip = this._contextMenu;
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Visible = true;
            // 
            // MenuItemOpen
            // 
            this.MenuItemOpen.Name = "MenuItemOpen";
            this.MenuItemOpen.Size = new System.Drawing.Size(190, 22);
            this.MenuItemOpen.Text = "ウィンドウを表示(&S)";
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(190, 22);
            this.MenuItemExit.Text = "終了(&E)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 6);
            this._contextMenu.ResumeLayout(false);

        }
    }
}
