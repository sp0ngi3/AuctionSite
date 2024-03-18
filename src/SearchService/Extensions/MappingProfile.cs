using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuctionCreated, Item>();
            CreateMap<AuctionUpdated, Item>();
        }
    }
}
