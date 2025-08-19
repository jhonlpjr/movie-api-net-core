using Domain.Entities;
using Infrastructure.Persistence.Mongo.Documents;

namespace Infrastructure.Persistence.Mongo.Mappings;

public static class MovieMappings
{
    public static Movie ToDomain(this MovieDocument d)
        => new(d.Title, d.Genre, d.Year, d.Rating, d.Popularity, d.Description) { Id = d.Id };

    public static MovieDocument ToDocument(this Movie m)
        => new()
        {
            Id = string.IsNullOrWhiteSpace(m.Id) ? null! : m.Id,
            Title = m.Title,
            Genre = m.Genre,
            Rating = m.Rating,
            Year = m.Year,
            Popularity = m.Popularity,
            Description = m.Description
        };
}
