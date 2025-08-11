using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Mongo.Documents;
using Infrastructure.Persistence.Mongo.Mappings;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Mongo;

public class MongoMovieRepository : IMovieRepository
{
    private readonly IMongoCollection<MovieDocument> _col;
    private readonly ILogger<MongoMovieRepository> _logger;

    public MongoMovieRepository(IMongoDatabase db, ILogger<MongoMovieRepository> logger)
    {
        _col = db.GetCollection<MovieDocument>("movies");
        _logger = logger;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Retrieving all movies from MongoDB.");
        var docs = await _col.Find(FilterDefinition<MovieDocument>.Empty).ToListAsync(ct);
        _logger.LogInformation("{Count} movies retrieved from MongoDB.", docs.Count);
        return docs.Select(x => x.ToDomain());
    }

    public async Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        _logger.LogInformation("Searching for movie by ID: {Id}", id);
        var doc = await _col.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
        if (doc != null)
            _logger.LogInformation("Movie found: {Title} ({Id})", doc.Title, doc.Id);
        else
            _logger.LogWarning("No movie found with ID: {Id}", id);
        return doc?.ToDomain();
    }

    public async Task<IEnumerable<Movie>> SearchAsync(string? q, string? genre = null, int limit = 50, CancellationToken ct = default)
    {
        _logger.LogInformation("Searching movies. Query: '{Query}', Genre: '{Genre}', Limit: {Limit}", q, genre, limit);
        if (limit <= 0) limit = 50;

        var f = Builders<MovieDocument>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(q))
            f &= Builders<MovieDocument>.Filter.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(q.Trim(), "i"));
        if (!string.IsNullOrWhiteSpace(genre))
            f &= Builders<MovieDocument>.Filter.AnyEq(x => x.Genre, genre.Trim());

        var docs = await _col.Find(f).Limit(limit).ToListAsync(ct);
        _logger.LogInformation("{Count} movies found for search.", docs.Count);
        return docs.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<Movie>> GetPopularAsync(int limit = 20, CancellationToken ct = default)
    {
        _logger.LogInformation("Retrieving popular movies. Limit: {Limit}", limit);
        if (limit <= 0) limit = 20;
        var docs = await _col.Find(FilterDefinition<MovieDocument>.Empty)
                             .SortByDescending(x => x.Popularity)
                             .Limit(limit)
                             .ToListAsync(ct);
        _logger.LogInformation("{Count} popular movies retrieved.", docs.Count);
        return docs.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<Movie>> GetRecommendationsAsync(int limit = 20, CancellationToken ct = default)
    {
        _logger.LogInformation("Retrieving recommended movies. Limit: {Limit}", limit);
        if (limit <= 0) limit = 20;
        var docs = await _col.Find(FilterDefinition<MovieDocument>.Empty)
                             .SortByDescending(x => x.Popularity)
                             .Skip(5)
                             .Limit(limit)
                             .ToListAsync(ct);
        _logger.LogInformation("{Count} recommended movies retrieved.", docs.Count);
        return docs.Select(x => x.ToDomain());
    }

    public async Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new movie: {Title}", movie.Title);
        var doc = movie.ToDocument();
        await _col.InsertOneAsync(doc, cancellationToken: ct);
        movie.Id = doc.Id; // ObjectId assigned by MongoDB
        _logger.LogInformation("Movie created with ID: {Id}", movie.Id);
        return movie;
    }
}