using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeefServer
{
    public class ChatHub : Hub
    {
        public void Notify(string name, string message)
        {
            string user = this.Context.Request.User.Identity.Name;
            Console.WriteLine("Connect by " + name + " with id:" + message);
        }

        public void Send(string name, string message)
        {
            string user = this.Context.Request.User.Identity.Name;
            Clients.All.addMessage(name, message);
        }

        public void send(string message)
        {
            string user = this.Context.Request.User.Identity.Name;
            Clients.All.addMessage("Server", message);
        }
    }
}
