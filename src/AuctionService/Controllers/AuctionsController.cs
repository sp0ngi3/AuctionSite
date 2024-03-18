using AuctionService.Context;
using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        private readonly AuctionDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            IQueryable<Auction> query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            Auction auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();
            return _mapper.Map<AuctionDTO>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDto)
        {
            Auction auction = _mapper.Map<Auction>(auctionDto);
            // TODO: add current user as seller
            auction.Seller = "test";

            _context.Auctions.Add(auction);

            AuctionDTO newAuction = _mapper.Map<AuctionDTO>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            bool result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById),
                new { auction.Id }, newAuction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDto)
        {
            Auction auction = await _context.Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            // TODO: check seller == username

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            bool result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem saving changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            Auction auction = await _context.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            // TODO: check seller == username

            _context.Auctions.Remove(auction);

            bool result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
