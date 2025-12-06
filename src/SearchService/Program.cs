using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("SearchDB", MongoClientSettings
    .FromConnectionString(builder.Configuration.GetConnectionString("MongoDBConnection")));

await DB.Index<Item>()
    .Key(i => i.Make, KeyType.Text)
    .Key(i => i.Model, KeyType.Text)
    .Key(i => i.Color, KeyType.Text)
    .CreateAsync();


app.Run();
