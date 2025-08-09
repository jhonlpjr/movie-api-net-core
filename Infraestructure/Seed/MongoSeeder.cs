using Domain.Entities;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Infrastructure.Seed;

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
                new Movie { Title="Inception",    Genre=["Sci-Fi","Action"], Rating=8.8, Year=2010, Popularity=95 },
                new Movie { Title="Interstellar",  Genre=["Sci-Fi","Drama"], Rating=8.6, Year=2014, Popularity=92 },
                new Movie { Title="The Dark Knight", Genre=["Action","Crime"], Rating=9.0, Year=2008, Popularity=98 }
            }, cancellationToken: ct);
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
