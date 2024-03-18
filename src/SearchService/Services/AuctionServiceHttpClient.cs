using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHttpClient(HttpClient httpclient, IConfiguration config)
    {
        private readonly HttpClient _httpclient = httpclient;
        private readonly IConfiguration _config = config;

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            string lastUpdated = await DB.Find<Item, string>()
           .Sort(x => x.Descending(x => x.UpdatedAt))
           .Project(x => x.UpdatedAt.ToString())
           .ExecuteFirstAsync();

            return await _httpclient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"]
            + "/api/auctions?date=" + lastUpdated);
        }
    }
}
