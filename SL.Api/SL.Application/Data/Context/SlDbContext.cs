
using Microsoft.EntityFrameworkCore;
using SL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SL.Application.Data.Context
{
    public class SlDbContext : DbContext
    {
        public SlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Связь User -> Images
            builder.Entity<Image>()
                .HasOne(i => i.Owner)
                .WithMany(u => u.Images)
                .HasForeignKey(i => i.UserId);

            // Связь User -> Nfts
            builder.Entity<Nft>()
                .HasOne(n => n.Owner)
                .WithMany(u => u.Nfts)
                .HasForeignKey(n => n.UserId);
            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Nft> Nfts { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
