using Domain.Repositories;              // o Interfaces
using Infrastructure.Options;
using Infrastructure.Persistence;
using Infrastructure.Seed;              // si usas el seeder
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Bind de opciones
        var opts = config.GetSection("Mongo").Get<MongoOptions>() ?? new MongoOptions();

        // Conexión (usa ConnectionString si la pasas completa)
        var conn = !string.IsNullOrWhiteSpace(opts.ConnectionString)
            ? opts.ConnectionString!
            : $"mongodb://{opts.User}:{opts.Password}@{opts.Host}:{opts.Port}/{opts.Database}?authSource={opts.AuthSource}";

        // REGISTROS CLAVE
        services.AddSingleton<IMongoClient>(_ => new MongoClient(conn));
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(opts.Database);
        });

        services.AddScoped<IMovieRepository, MongoMovieRepository>();

        // (opcional) seeder
        services.AddHostedService<MongoSeeder>();

        return services;
    }
}
