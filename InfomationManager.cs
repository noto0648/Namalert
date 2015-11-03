using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamaAlert
{
    public class InfomationManager
    {
        private static InfomationManager _instance = new InfomationManager();
        public static InfomationManager Instance { get { return _instance; } }

        private List<InfoWindow> _infoWindows = new List<InfoWindow>();

        public InfoWindow NewWindow(System.Windows.Window owner, int time = 1000, string url = null)
        {
            InfoWindow iw = new InfoWindow(time, url);
            int nextIndex = _infoWindows.Count;
            for(int i = 0; i < _infoWindows.Count; i++)
            {
                if(_infoWindows[i] == null)
                {
                    nextIndex=i;
                    break;
                }
            }
            var screenSize = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            iw.Top = screenSize.Height - (iw.Height + (nextIndex * iw.Height));
            iw.Left = screenSize.Width - (iw.Width + 0);


            iw.Owner = owner;
            if (nextIndex == _infoWindows.Count)
            {
                _infoWindows.Add(iw);
            }
            else
            {
                _infoWindows[nextIndex] = iw;
            }
            return iw;
        }

        public void RemoveWindow(InfoWindow iw)
        {
            _infoWindows[_infoWindows.IndexOf(iw)] = null;
        }

    }
}
