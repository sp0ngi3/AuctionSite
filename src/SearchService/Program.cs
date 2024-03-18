WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
