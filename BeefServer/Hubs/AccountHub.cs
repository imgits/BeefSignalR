using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeefServer.Database;

namespace BeefServer.Hubs
{
    public class AccountHub : Hub
    {
        public void Register(User user)
        {
            using (BeefDbContext db = new BeefDbContext())
            {
                user = db.add_user(user);
            }
        }
    }
}
