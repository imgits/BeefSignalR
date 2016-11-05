using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeefServer.Database
{
    class BeefDbContext : DbContext
    {
        public static void Init()
        {
            System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BeefDbContext>());
        }

        public BeefDbContext()
            : base("name=MySqlContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure domain classes using Fluent API here
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }

        public User add_user(User user)
        {
            Users.Add(user);
            int ret = SaveChanges();
            return user;
        }

    }
}
