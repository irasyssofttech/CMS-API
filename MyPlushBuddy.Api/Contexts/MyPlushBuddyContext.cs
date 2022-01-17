using Microsoft.EntityFrameworkCore;
using MyPlushBuddy.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Contexts
{
    public class MyPlushBuddyContext: DbContext
    {
        public DbSet<PageTag> PageTags { get; set; }

        public MyPlushBuddyContext(DbContextOptions<MyPlushBuddyContext> options)
            :base(options)
        {
            // Database.EnsureCreated();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
        }
    }
}
