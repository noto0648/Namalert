using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamaAlert
{
    public class SettingData
    {
        public string NiconicoMailaddress { get; set; }
        public string NiconicoPassword { get; set; }

        public SettingData()
        {
            NiconicoMailaddress = "";
            NiconicoPassword = "";
        }
    }
}
