using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamaAlert
{
    public interface IOwnerWindow
    {
        System.Windows.Threading.Dispatcher Dispatcher { get; }
        InfoWindow NewInfomation(int time, string url);
    }
}
