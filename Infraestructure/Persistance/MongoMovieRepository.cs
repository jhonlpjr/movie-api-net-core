using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class MongoMovieRepository : IMovieRepository
{
    private readonly IMongoCollection<Movie> _col;  // <-- Movie, no Movie?
    public MongoMovieRepository(IMongoDatabase db)
    {
        _col = db.GetCollection<Movie>("movies");
    }

    public async Task<IEnumerable<Movie>> GetPopularAsync(int limit) =>
        await _col.Find(FilterDefinition<Movie>.Empty)
                  .SortByDescending(x => x.Popularity)
                  .Limit(limit)
                  .ToListAsync();

    public Task<Movie> GetByIdAsync(string id) =>
        _col.Find(x => x.Id == id).FirstOrDefaultAsync(); // puede devolver null

    public async Task<IEnumerable<Movie>> SearchAsync(string? q, string? genre, int limit)
    {
        var filter = Builders<Movie>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(q))
            filter &= Builders<Movie>.Filter.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(q, "i"));
        if (!string.IsNullOrWhiteSpace(genre))
            filter &= Builders<Movie>.Filter.AnyEq(x => x.Genre, genre);

        return await _col.Find(filter).Limit(limit).ToListAsync();
    }
}