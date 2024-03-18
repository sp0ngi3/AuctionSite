using SearchService.Data;
using SearchService.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>();

WebApplication app = builder.Build();

app.UseAuthorization();
app.MapControllers();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
