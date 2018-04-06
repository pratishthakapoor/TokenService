using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Support24.Runbook
{
    public class RunbookAuthentication
    {
        public void runScript()
        {
            try
            {
                string Uri = "https://s13events.azure-automation.net/webhooks?token=KABhb3NEJCq22z7v0a3%2fMR4rw0P8Qplg61B3mDMrgSk%3d";

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Uri);

                string data = string.Empty;
                request.Method = "POST";
                request.ContentType = "text/plain;charset=utf-8";
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] bytes = encoding.GetBytes(data);

                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                request.BeginGetResponse((x) =>
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            String responseString = reader.ReadToEnd();
                            Console.WriteLine("Script Triggered" + System.DateTime.Now + "\n Job details" + responseString);
                        }
                    }
                }, null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
    }
}