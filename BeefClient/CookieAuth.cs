using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeefClient
{
    class UserAuth
    {
        public CookieContainer Cookies = new CookieContainer();

        public bool CookieAuth1(string url, string username, string password)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.CookieContainer = Cookies;
                using (var httpClient = new HttpClient(handler))
                {
                    var content = string.Format("Username={0}&Password={1}", username, password);
                    var response = httpClient.PostAsync(url + "/Account/Login", new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                    if (response.StatusCode == HttpStatusCode.OK) return true;
                }
            }
            return false;
        }

        public bool CookieAuth2(string url, string username, string password)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url + "/Account/Login");
            request.CookieContainer = Cookies;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            var content = string.Format("Username={0}&Password={1}", username, password);
            byte[] bytedata = Encoding.UTF8.GetBytes(content);
            request.ContentLength = bytedata.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK) return true;
            else return false;
        }

        private void RunWindowsAuth(string url, string username, string password)
        {
        //    var hubConnection = new HubConnection(url);
        //    hubConnection.TraceWriter = _traceWriter;

        //    hubConnection.Credentials = CredentialCache.DefaultCredentials;

        //    var hubProxy = hubConnection.CreateHubProxy("AuthHub");
        //    hubProxy.On<string, string>("invoked", (connectionId, date) => hubConnection.TraceWriter.WriteLine("connectionId={0}, date={1}", connectionId, date));

        //    hubConnection.Start().Wait();
        //    hubConnection.TraceWriter.WriteLine("transport.Name={0}", hubConnection.Transport.Name);

        //    hubProxy.Invoke("InvokedFromClient").Wait();
        }

    }
}
