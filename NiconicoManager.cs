using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NamaAlert
{
    public class NiconicoManager
    {
        private static NiconicoManager _instance = new NiconicoManager();
        public static NiconicoManager Instance { get { return _instance; } }

        private const string LoginUrl = @"https://secure.nicovideo.jp/secure/login?site=nicolive_antenna";
        private const string StatusUrl = @"http://live.nicovideo.jp/api/getalertstatus";

        private const string LoginParam = @"mail={0}&password={1}";
        private const string StatusParam = @"ticket={0}";

        private string _ticket;

        private NiconicoManager() { }

        public bool Login(string mail, string pass)
        {
            string result = Utils.PostData(LoginUrl, string.Format(LoginParam, mail, pass));

            if (result == null || result.Length <= 0)
                return false;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            _ticket = doc.DocumentElement.GetElementsByTagName("ticket")[0].InnerText;

            if (_ticket == null || _ticket.Length <= 0)
                return false;

            return true;
        }

        public string GetStatus()
        {
            if (_ticket == null || _ticket.Length == 0)
                return null;

            string data = Utils.PostData(StatusUrl, string.Format(StatusParam, _ticket));
            if (data == null || data.Length == 0)
                return null;

            return data;
        }

        public class NiconicoAlertItem
        {
            private string _communityName;
            private string _communityId;
            private bool _enable;
            private bool _useBrowser;
            public string CommunityName { get { return _communityName; } set { _communityName = value; } }
            public string CommunityId { get { return _communityId; } set { _communityId = value; } }
            public bool UseBrowser { get { return _useBrowser; } set { _useBrowser = value; } }
            public bool Enable { get { return _enable; } set { _enable = value; } }

        }
    }
}
