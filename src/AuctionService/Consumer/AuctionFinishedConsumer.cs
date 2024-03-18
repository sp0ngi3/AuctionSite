using AuctionService.Context;
using AuctionService.Models;
using Contracts;
using MassTransit;

namespace AuctionService.Consumer
{
    public class AuctionFinishedConsumer(AuctionDbContext dbContext) : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finished");

            Auction auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmunt = context.Message.Amount;
            }

            auction.Status = auction.SoldAmunt > auction.ReservePrice
                ? Status.Finished : Status.ReserveNotMet;

            await _dbContext.SaveChangesAsync();
        }
    }
}
