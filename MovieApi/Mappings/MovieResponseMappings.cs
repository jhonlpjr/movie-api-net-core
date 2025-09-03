using Domain.Entities;
using MovieApi.Contracts.Responses;

namespace MovieApi.Mappings;

public static class MovieResponseMappings
{
    public static MovieResponse ToResponse(this Movie m) => new()
    {
        Id = m.Id,
        Title = m.Title,
        Genre = m.Genre,
        Rating = m.Rating,
        Year = m.Year,
        Popularity = m.Popularity,
        Description = m.Description
    };

    public static IEnumerable<MovieResponse> ToResponse(this IEnumerable<Movie> items)
        => items.Select(ToResponse);
}
