using Domain.Entities;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Infraestructure.Persistance.Mongo.Seed;

public class MongoSeeder : IHostedService
{
    private readonly IMongoDatabase _db;
    private readonly IHostEnvironment _env;

    public MongoSeeder(IMongoDatabase db, IHostEnvironment env)
    {
        _db = db; _env = env;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        if (!_env.IsDevelopment()) return;

        var col = _db.GetCollection<Movie>("movies");
        if (await col.EstimatedDocumentCountAsync(cancellationToken: ct) == 0)
        {
            await col.InsertManyAsync(new[]
            {
                new Movie("Inception",    new List<string> { "Sci-Fi", "Action" }, 2010, 8.8, 95),
                new Movie("Interstellar", new List<string> { "Sci-Fi", "Drama" }, 2014, 8.6, 92),
                new Movie("The Dark Knight", new List<string> { "Action", "Crime" }, 2008, 9.0, 98)
            }, cancellationToken: ct);
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}