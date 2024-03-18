using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {

            await DB.InitAsync("SearchDB", MongoClientSettings.
                FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            long count = await DB.CountAsync<Item>();

            if (count == 0)
            {
                Console.WriteLine("No Data => Will Attemp to Seed !");

                string itemData = await File.ReadAllTextAsync("Data/auctions.json");

                JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

                List<Item> items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                await DB.SaveAsync(items);
            }
        }
    }
}
