using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using GameStore.Models;
using GameStoreV1._0.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStoreV1._0.DB
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Profile> profile { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<MoneyHistory> MoneyHistory { get; set; }
        public DbSet<Game> game { get; set; }
        public DbSet<Order> order { get; set; }
        public DbSet<OrderDetail> orderDetail { get; set; }
        public DbSet<CategoryConnect> CategoryConnect { get; set; }
        public DbSet<ShoppingCart> shoppingCart { get; set; }
        public DbSet<Wishlist> wishlist { get; set; }
        public DbSet<Feedback> feedback { get; set; }
        public DbSet<MoneyManagement> moneyManagement { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryConnect>().HasKey(c => new {c.GID, c.CID });
            modelBuilder.Entity<MoneyManagement>().HasKey(c => new {c.ODID, c.username});
            modelBuilder.Entity<ShoppingCart>().HasKey(c => new {c.username, c.GID});
            modelBuilder.Entity<Wishlist>().HasKey(c => new {c.username, c.GID});
        }

    }
}