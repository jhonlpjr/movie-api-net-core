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

        using var cursor = await _col.FindAsync(
            filter: FilterDefinition<MovieDocument>.Empty,
            options: new FindOptions<MovieDocument, MovieDocument>(),
            cancellationToken: ct);

        var docs = await cursor.ToListAsync(ct);

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

    public async Task<IEnumerable<Movie>> SearchAsync(
        string? query = null,
        string? genre = null,
        int? yearFrom = null,
        int? yearTo = null,
        int? popularity = null,
        double? rating = null,
        string? orderBy = null,
        string? orderDirection = null,
        int limit = 50,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Searching movies. Query: '{Query}', Genre: '{Genre}', YearFrom: {YearFrom}, YearTo: {YearTo}, Popularity: {Popularity}, Rating: {Rating}, OrderBy: {OrderBy}, OrderDirection: {OrderDirection}, Limit: {Limit}",
            query, genre, yearFrom, yearTo, popularity, rating, orderBy, orderDirection, limit);

        if (limit <= 0) limit = 50;

        var filters = new List<FilterDefinition<MovieDocument>>();

        // Query tipo like en Title o Description (insensible a mayúsculas)
        if (!string.IsNullOrWhiteSpace(query))
        {
            var regex = new MongoDB.Bson.BsonRegularExpression(query.Trim(), "i");
            filters.Add(Builders<MovieDocument>.Filter.Or(
                Builders<MovieDocument>.Filter.Regex(x => x.Title, regex),
                Builders<MovieDocument>.Filter.Regex(x => x.Description, regex)
            ));
        }

        // Genre tipo like en array (insensible a mayúsculas)
        if (!string.IsNullOrWhiteSpace(genre))
        {
            var regexGenre = new MongoDB.Bson.BsonRegularExpression(genre.Trim(), "i");
            filters.Add(Builders<MovieDocument>.Filter.ElemMatch(x => x.Genre, g => g.ToLower().Contains(genre.Trim().ToLower())));
            // Alternativa con regex si quieres buscar por coincidencia parcial
            // filters.Add(Builders<MovieDocument>.Filter.Regex("Genre", regexGenre));
        }

        // Year rango dinámico
        if (yearFrom.HasValue && yearTo.HasValue)
            filters.Add(Builders<MovieDocument>.Filter.And(
                Builders<MovieDocument>.Filter.Gte(x => x.Year, yearFrom.Value),
                Builders<MovieDocument>.Filter.Lte(x => x.Year, yearTo.Value)
            ));
        else if (yearFrom.HasValue)
            filters.Add(Builders<MovieDocument>.Filter.Gte(x => x.Year, yearFrom.Value));
        else if (yearTo.HasValue)
            filters.Add(Builders<MovieDocument>.Filter.Lte(x => x.Year, yearTo.Value));

        // Popularity >=
        if (popularity.HasValue)
            filters.Add(Builders<MovieDocument>.Filter.Gte(x => x.Popularity, popularity.Value));

        // Rating >=
        if (rating.HasValue)
            filters.Add(Builders<MovieDocument>.Filter.Gte(x => x.Rating, rating.Value));

        var filter = filters.Count > 0
            ? Builders<MovieDocument>.Filter.And(filters)
            : Builders<MovieDocument>.Filter.Empty;

        var find = _col.Find(filter);

        // Orden dinámico
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            var direction = orderDirection?.ToLower() == "asc" ? 1 : -1;
            switch (orderBy.ToLower())
            {
                case "title":
                    find = direction == 1 ? find.SortBy(x => x.Title) : find.SortByDescending(x => x.Title);
                    break;
                case "year":
                    find = direction == 1 ? find.SortBy(x => x.Year) : find.SortByDescending(x => x.Year);
                    break;
                case "popularity":
                    find = direction == 1 ? find.SortBy(x => x.Popularity) : find.SortByDescending(x => x.Popularity);
                    break;
                case "rating":
                    find = direction == 1 ? find.SortBy(x => x.Rating) : find.SortByDescending(x => x.Rating);
                    break;
            }
        }

        var docs = await find.Limit(limit).ToListAsync(ct);
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

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating movie: {Id}", movie.Id);
        var filter = Builders<MovieDocument>.Filter.Eq(x => x.Id, movie.Id);
        var updateDoc = movie.ToDocument();

        var result = await _col.ReplaceOneAsync(filter, updateDoc, cancellationToken: ct);

        if (result.MatchedCount == 0)
        {
            _logger.LogWarning("No movie found to update with ID: {Id}", movie.Id);
            return null;
        }

        _logger.LogInformation("Movie updated: {Id}", movie.Id);
        return movie;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _col.DeleteOneAsync(x => x.Id == id, ct);
        _logger.LogInformation("Delete result for movie {Id}: {DeletedCount}", id, result.DeletedCount);
        return result.DeletedCount > 0;
    }
}