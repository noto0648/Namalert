using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace NamaAlert
{
    public class Utils
    {
        public static string GetAppDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string GetResponse(string url)
        {
            string result = null;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    result = wc.DownloadString(url);
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static string PostData(string url, string data)
        {
            if (data == null) data = "";

            byte[] postDataBytes = System.Text.Encoding.UTF8.GetBytes(data);

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = data.Length;
            webReq.UserAgent = "Namaralert1.0.0.0";

            using (Stream reqStream = webReq.GetRequestStream())
            {
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            }

            string r = null;

            using (HttpWebResponse webRes = (HttpWebResponse)webReq.GetResponse())
            {
                using (StreamReader sr = new StreamReader(webRes.GetResponseStream()))
                {
                    r = sr.ReadToEnd();
                }
            }
            return r;
        }
    }
}
