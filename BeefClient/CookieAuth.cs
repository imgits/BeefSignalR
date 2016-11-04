using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeefClient
{
    class CookieAuth
    {
        public CookieContainer Cookies = new CookieContainer();
        bool Auth(string url, string username, string password)
        {// url+ "/Account/Login"
            var handler = new HttpClientHandler();
            handler.CookieContainer = Cookies;
            using (var httpClient = new HttpClient(handler))
            {
                var content = string.Format("UserName={0}&Password={1}", "user", "password");
                var response = httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                if (response.StatusCode == HttpStatusCode.OK) return true;
            }
            return false;
        }
    }
}
