using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;

public class SearchMoviesUseCase(IMovieRepository repo)
{
    public Task<IEnumerable<Movie>> ExecuteAsync(
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
        return repo.SearchAsync(
            query: query,
            genre: genre,
            yearFrom: yearFrom,
            yearTo: yearTo,
            popularity: popularity,
            rating: rating,
            orderBy: orderBy,
            orderDirection: orderDirection,
            limit: limit,
            ct: ct
        );
    }
}
