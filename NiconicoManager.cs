using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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

        public List<NiconicoAlertItem> Communities { get { return _nicoCommunities; } }
        private List<NiconicoManager.NiconicoAlertItem> _nicoCommunities = new List<NiconicoManager.NiconicoAlertItem>();

        private object _lock = new object();

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

        public bool GetCommunities()
        {
            List<NiconicoAlertItem> result = new List<NiconicoAlertItem>();
            string text = GetStatus();

            if (text != null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(text);

                    XmlNodeList nodeList = doc.DocumentElement.GetElementsByTagName("communities");
                    for (int i = 0; i < nodeList[0].ChildNodes.Count; i++)
                    {
                        string id = nodeList[0].ChildNodes[i].InnerText;
                        result.Add(new NiconicoAlertItem() { CommunityId = id, IsChannel = !id.StartsWith("co") });
                    }

                }
                catch(Exception)
                {
                    return false;
                }
            }
            _nicoCommunities = result;
            return true;
        }


        public void LoadCommunities()
        {
            string filePath = System.IO.Path.GetFullPath(Utils.GetAppDirectory() + "\\Settings\\Niconico.xml");
            if (System.IO.File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<NiconicoManager.NiconicoAlertItem>));
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath, Encoding.UTF8))
                {
                    _nicoCommunities = (List<NiconicoManager.NiconicoAlertItem>)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
        }

        public void SaveCommunities()
        {
            string filePath = System.IO.Path.GetFullPath(Utils.GetAppDirectory() + "\\Settings\\Niconico.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<NiconicoManager.NiconicoAlertItem>), new Type[] { typeof(NiconicoManager.NiconicoAlertItem) });
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false))
            {
                serializer.Serialize(writer, _nicoCommunities);
                writer.Close();
            }
        }

        public InfomationType GetInfomationType(string communityid)
        {
            //Console.WriteLine(communityid);
            lock (_lock)
            {
                NiconicoAlertItem index = _nicoCommunities.Find(w => w.CommunityId == communityid);
                if (index != null)
                {
                    return (index.Enable && index.UseBrowser) ? InfomationType.OpenBrowser : (index.Enable) ? InfomationType.Infomation : InfomationType.None;
                }
            }
            return InfomationType.None;
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

            private bool _isChannel;
            public bool IsChannel { get { return _isChannel; } set { _isChannel = value; } }

            public NiconicoAlertItem()
            {
                _enable = true;
            }
        }
    }
}
