﻿using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
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

            using IServiceScope scope = app.Services.CreateScope();

            AuctionServiceHttpClient httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            List<Item> items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine(items.Count + " returned from the auction service");

            if (items.Count > 0) await DB.SaveAsync(items);
        }
    }
}
