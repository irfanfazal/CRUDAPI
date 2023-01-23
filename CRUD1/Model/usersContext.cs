using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD1.Model
{
    public class usersContext : DbContext
    {//options is name of object
        public usersContext(DbContextOptions<usersContext> options)
            : base(options) 
        { }
        public DbSet<users> users { get; set; }

        public DbSet<loginmodel> loginmodel { get; set; }
    }
}
