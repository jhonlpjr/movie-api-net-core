using Domain.Entities;
using Domain.Repositories;
using Application.Common;
using Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public class CachedMovieRepository : IMovieRepository
{
    private readonly ILogger<CachedMovieRepository> _logger;
    private readonly IMovieRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly CacheOptions _opts;
    private readonly IConnectionMultiplexer _redis;

    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = false };

    public CachedMovieRepository(
        IMovieRepository inner,
        IDistributedCache cache,
        IOptions<CacheOptions> opts,
        ILogger<CachedMovieRepository> logger,
        IConnectionMultiplexer redis)
    {
        _inner = inner;
        _cache = cache;
        _opts = opts.Value;
        _logger = logger;
        _redis = redis;
    }
    // Borra todas las claves de cache de búsquedas
    public async Task RemoveAllSearchCacheAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        foreach (var key in server.Keys(pattern: "movieapi:movies:search:*"))
        {
            await _redis.GetDatabase().KeyDeleteAsync(key);
        }
        _logger.LogInformation("All search cache keys have been deleted.");
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default)
    {
        var key = "movies:all";
        _logger.LogInformation("Trying to retrieve all movies from cache with key '{CacheKey}'", key);
        var cached = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache HIT for all movies (key: '{CacheKey}')", key);
            return JsonSerializer.Deserialize<List<Movie>>(cached, JsonOpts)!;
        }

        _logger.LogInformation("Cache MISS for all movies (key: '{CacheKey}'). Querying inner repository.", key);
        var result = (await _inner.GetAllAsync(ct)).ToList();

        var entryOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opts.PopularMoviesTtlMinutes)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(result, JsonOpts), entryOpts, ct);
        _logger.LogInformation("All movies saved to cache (key: '{CacheKey}', TTL: {Ttl} minutes)", key, _opts.PopularMoviesTtlMinutes);
        return result;
    }
    public async Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var key = $"movies:id:{id}";
        _logger.LogInformation("Trying to retrieve movie by ID from cache with key '{CacheKey}'", key);
        var cached = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache HIT for movie by ID (key: '{CacheKey}')", key);
            return JsonSerializer.Deserialize<Movie>(cached, JsonOpts)!;
        }

        _logger.LogInformation("Cache MISS for movie by ID (key: '{CacheKey}'). Querying inner repository.", key);
        var result = await _inner.GetByIdAsync(id, ct);

        if (result != null)
        {
            var entryOpts = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opts.PopularMoviesTtlMinutes)
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(result, JsonOpts), entryOpts, ct);
            _logger.LogInformation("Movie by ID saved to cache (key: '{CacheKey}', TTL: {Ttl} minutes)", key, _opts.PopularMoviesTtlMinutes);
        }
        return result;
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
        int limit = Pagination.DefaultPageSize,
        CancellationToken ct = default)
    {
        var key = $"movies:search:{query}:{genre}:{yearFrom}:{yearTo}:{popularity}:{rating}:{orderBy}:{orderDirection}:{limit}";
        _logger.LogInformation("Trying to retrieve search results from cache with key '{CacheKey}'", key);
        var cached = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache HIT for search results (key: '{CacheKey}')", key);
            return JsonSerializer.Deserialize<List<Movie>>(cached, JsonOpts)!;
        }

        _logger.LogInformation("Cache MISS for search results (key: '{CacheKey}'). Querying inner repository.", key);
        var result = (await _inner.SearchAsync(query, genre, yearFrom, yearTo, popularity, rating, orderBy, orderDirection, limit, ct)).ToList();

        var entryOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opts.PopularMoviesTtlMinutes)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(result, JsonOpts), entryOpts, ct);
        _logger.LogInformation("Search results saved to cache (key: '{CacheKey}', TTL: {Ttl} minutes)", key, _opts.PopularMoviesTtlMinutes);
        return result;
    }

    public async Task<IEnumerable<Movie>> GetPopularAsync(int limit = Pagination.DefaultPageSize, CancellationToken ct = default)
    {
        var key = $"movies:popular:{limit}";
        _logger.LogInformation("Trying to retrieve popular movies from cache with key '{CacheKey}'", key);
        var cached = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache HIT for popular movies (key: '{CacheKey}')", key);
            return JsonSerializer.Deserialize<List<Movie>>(cached, JsonOpts)!;
        }

        _logger.LogInformation("Cache MISS for popular movies (key: '{CacheKey}'). Querying inner repository.", key);
        var items = (await _inner.GetPopularAsync(limit, ct)).ToList();

        var entryOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opts.PopularMoviesTtlMinutes)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(items, JsonOpts), entryOpts, ct);
        _logger.LogInformation("Popular movies saved to cache (key: '{CacheKey}', TTL: {Ttl} minutes)", key, _opts.PopularMoviesTtlMinutes);
        return items;
    }

    public async Task<IEnumerable<Movie>> GetRecommendationsAsync(int limit = Pagination.DefaultPageSize, CancellationToken ct = default)
    {
        var key = $"movies:reco:{limit}";
        _logger.LogInformation("Trying to retrieve recommendations from cache with key '{CacheKey}'", key);
        var cached = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Cache HIT for recommendations (key: '{CacheKey}')", key);
            return JsonSerializer.Deserialize<List<Movie>>(cached, JsonOpts)!;
        }

        _logger.LogInformation("Cache MISS for recommendations (key: '{CacheKey}'). Querying inner repository.", key);
        var items = (await _inner.GetRecommendationsAsync(limit, ct)).ToList();

        var entryOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_opts.RecommendationsTtlMinutes)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(items, JsonOpts), entryOpts, ct);
        _logger.LogInformation("Recommendations saved to cache (key: '{CacheKey}', TTL: {Ttl} minutes)", key, _opts.RecommendationsTtlMinutes);
        return items;
    }

    public async Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new movie: {Title}", movie.Title);
        var result = await _inner.CreateAsync(movie, ct);
        _logger.LogInformation("Movie created with ID: {Id}", result.Id);
        await RemoveAllSearchCacheAsync();
        return result;
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken ct = default)
    {
        var updated = await _inner.UpdateAsync(movie, ct);

        // Invalida la caché relevante
        await RemoveAllSearchCacheAsync();

        if (movie.Id is not null)
            await _cache.RemoveAsync($"movies:id:{movie.Id}", ct);

        return updated;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var deleted = await _inner.DeleteAsync(id, ct);

        // Invalida la caché relevante
        await RemoveAllSearchCacheAsync();

        return deleted;
    }
}