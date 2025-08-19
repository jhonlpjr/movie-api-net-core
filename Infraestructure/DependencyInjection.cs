// Infrastructure/DependencyInjection.cs
using Domain.Repositories;
using Infrastructure.Caching;
using Infrastructure.Options;
using Infrastructure.Persistence.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Bind options
        services.AddOptions<MongoOptions>().Bind(config.GetSection("Mongo"));
        services.AddOptions<RedisOptions>().Bind(config.GetSection("Redis"));
        services.AddOptions<CacheOptions>().Bind(config.GetSection("Cache"));

        // Mongo
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mo = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoOptions>>().Value;
            var conn = string.IsNullOrWhiteSpace(mo.ConnectionString) ? mo.ToConnectionString() : mo.ConnectionString!;
            return new MongoClient(conn);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var mo = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mo.Database);
        });

        // Redis (Microsoft.Extensions.Caching.StackExchangeRedis)
        var ro = config.GetSection("Redis").Get<RedisOptions>()!;
        services.AddStackExchangeRedisCache((o) =>
        {
            o.Configuration = ro.ToConfigurationString();
            o.InstanceName = ro.InstanceName; // prefijo para claves de cache
        });

        // Repo + Decorator (cache)
        services.AddScoped<IMovieRepository, MongoMovieRepository>();
        services.Decorate<IMovieRepository, CachedMovieRepository>(); // requiere Scrutor

        return services;
    }
}
