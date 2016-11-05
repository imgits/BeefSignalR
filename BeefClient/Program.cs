using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;

namespace BeefClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            string url = "http://localhost:8088";
            UserAuth ca = new UserAuth();
            if (!ca.CookieAuth1(url, "user", "password"))
            {
                Console.WriteLine("Login failed\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            else Console.WriteLine("Login OK");
            var hubConnection = new HubConnection(url);
            hubConnection.CookieContainer = ca.Cookies;

            hubConnection.Headers.Add("username", "user");
            hubConnection.Headers.Add("password", "password");

            var chat = hubConnection.CreateHubProxy("ChatHub");
            //var chat = hubConnection.CreateHubProxy("AuthorizeEchoHub");
            
            chat.On<string, string>("addMessage", (name, message) => { Console.Write(name + ": "); Console.WriteLine(message); });
            chat.On<string>("send", (message) => { Console.Write("server : "); Console.WriteLine(message); });
            hubConnection.Start().Wait();
            //hubConnection.Headers.Remove("password");
            chat.Invoke("Notify", "Console app", hubConnection.ConnectionId);
            string msg = null;

            while ((msg = Console.ReadLine()) != null)
            {
                //chat.Invoke("Send", "Console app", msg).Wait();
                chat.Invoke("send", msg).Wait();
            }

        }
    }
}
