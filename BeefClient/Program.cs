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
            //Console.ReadKey();

            var hubConnection = new HubConnection(" http://localhost:8080");
            var chat = hubConnection.CreateHubProxy("ChatHub");
            chat.On<string, string>("addMessage", (name, message) => { Console.Write(name + ": "); Console.WriteLine(message); });
            chat.On<string>("send", (message) => { Console.Write("server : "); Console.WriteLine(message); });
            hubConnection.Start().Wait();
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
