using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Data;
using System.Data.Entity;
using BeefServer.Database;

namespace BeefServer
{
    class Program
    {
        static void Main(string[] args)
        {
            BeefDbContext.Init();
            using (Database.BeefDbContext Db = new Database.BeefDbContext())
            {
                User user = new User() { username= "ahai",password="password",role="admin"};
                //Db.add_user(user);
            }

            using (WebApp.Start<Startup>("http://+:8088/"))
            {
                Console.WriteLine("Server running at http://localhost:8088/");
                Console.ReadLine();
            }
        }
    }
}
