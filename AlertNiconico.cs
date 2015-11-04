using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Windows.Media.Imaging;

namespace NamaAlert
{
    public class AlertNiconico : IAlertSystem
    {
        const string GetAlertInfoUrl = @"http://live.nicovideo.jp/api/getalertinfo";
        const string GetLiveInfoUrl = @"http://live.nicovideo.jp/api/getstreaminfo/lv{0}";

        private AlertInfo _infomation;
        private bool _isRunning;
        private bool _stopFlag = false;
        private IOwnerWindow _ownerWindow;

        Random rand = new Random();

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public void Start(IOwnerWindow owner)
        {
            _ownerWindow = owner;
            _infomation = null;
            if(!InitAlert())
            {
                return;
            }
            AlertLoop();
        }


        private bool InitAlert()
        {
            _isRunning = true;
            string xml = Utils.GetResponse(GetAlertInfoUrl);
            if (xml == null)
            {
                return false;
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            _infomation = new AlertInfo();
            _infomation.UserId = doc.DocumentElement.GetElementsByTagName("user_id")[0].InnerText;
            _infomation.UserId = doc.DocumentElement.GetElementsByTagName("user_hash")[0].InnerText;
            XmlNodeList msList = doc.DocumentElement.GetElementsByTagName("ms");
            _infomation.Address = msList[0].ChildNodes[0].InnerText;
            _infomation.Port = int.Parse(msList[0].ChildNodes[1].InnerText);
            _infomation.Thread = long.Parse(msList[0].ChildNodes[2].InnerText);
            return true;
        }

        private void AlertLoop()
        {
            try
            {
                IPAddress address = Dns.GetHostAddresses(_infomation.Address)[0];
                IPEndPoint endPoint = new IPEndPoint(address, _infomation.Port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                string param = string.Format("<thread thread=\"{0}\" version=\"20061206\" res_from=\"-1\"/>\0", _infomation.Thread);
                byte[] data = Encoding.UTF8.GetBytes(param);
                socket.Send(data, data.Length, SocketFlags.None);
                Console.WriteLine("Connecting...");
                string prev = "";
                _stopFlag = false;
                while(!_stopFlag)
                {
                    byte[] resBytes = new byte[1024 * 100];
                    int resSize = socket.Receive(resBytes, resBytes.Length, SocketFlags.None);
                    if (resSize != 0)
                    {
                        string xml = prev + Encoding.UTF8.GetString(resBytes, 0, resSize);

                        xml = xml.Replace('\0', '\n');
                        string[] lines = xml.Split('\n');
                        for(int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[i];
                            if(line.Length > 0 && !line.EndsWith(">"))
                            {
                                prev = line;
                                break;
                            }
                            if(line.StartsWith("<chat"))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(line);
                                string[] infos = doc.InnerText.Split(',');
                                if(infos.Length != 3)
                                {
                                    continue;
                                }
                                if (rand.Next(4) == 0)
                                {
                                    ShowInfomation(infos[0], infos[1], infos[2]);

                                }
                            }
                        }
                    }
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                _isRunning = false;
                _stopFlag = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private void ShowInfomation(string live, string comu, string user)
        {
            string result = null;

            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                InfoWindow window = _ownerWindow.NewInfomation(5000, "http://live.nicovideo.jp/watch/lv" + live);
                window.Show();
                Task.Factory.StartNew(() =>
                {
                    result = Utils.GetResponse(string.Format(GetLiveInfoUrl, live));
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);
                    XmlNodeList infos = doc.DocumentElement.GetElementsByTagName("streaminfo");

                    window.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        window.InfomationTitle = infos[0].ChildNodes[0].InnerText;
                        window.InfomationDescription = infos[0].ChildNodes[1].InnerText;

                        window.Title = infos[0].ChildNodes[2].InnerText + " - " + doc.DocumentElement.GetElementsByTagName("communityinfo")[0].ChildNodes[0].InnerText;

                        BitmapImage imageSource = new BitmapImage(new Uri(doc.DocumentElement.GetElementsByTagName("communityinfo")[0].ChildNodes[1].InnerText));
                        window.InfomationImage = imageSource;
                    }));
                });
            }));


        }

        public void Stop()
        {
            if (!_stopFlag)
            {
                _stopFlag = true;
            }
        }



        class AlertInfo
        {
            private string _userId;
            private string _userHash;
            private string _address;
            private int _port;
            private long _thread;

            public string UserId { get { return _userId; } set { _userId = value; } }
            public string UserHash { get { return _userHash; } set { _userHash = value; } }
            public string Address { get { return _address; } set { _address = value; } }
            public int Port { get { return _port; } set { _port = value; } }
            public long Thread { get { return _thread; } set { _thread = value; } }

        }



    }
}
