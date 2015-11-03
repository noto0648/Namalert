using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamaAlert
{
    public interface IAlertSystem
    {
        bool IsRunning { get; }
        void Start(IOwnerWindow owner);
        void Stop();
    }
}
