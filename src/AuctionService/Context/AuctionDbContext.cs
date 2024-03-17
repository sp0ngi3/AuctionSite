using AuctionService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Context
{
    public class AuctionDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Auction> Auctions { get; set; }
    }
}
