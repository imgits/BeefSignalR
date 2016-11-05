using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeefServer.Hubs
{
    public class ChatHub : Hub
    {
        //[Authorize]
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            return Clients.Caller.display("Authenticated and Conencted!");
            //return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return Clients.Caller.display("Reconencted!");
            //return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return Clients.Caller.display("Disconnected!");
            //return base.DiscnConnected(stopCalled);
        }

        public void Notify(string name, string message)
        {
            //string user = this.Context.User;//.Request.User.Identity.Name;
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
