using AuctionService.Models;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace AuctionService.Context
{
    public class AuctionDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
