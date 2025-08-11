using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;

public class SearchMoviesUseCase(IMovieRepository repo)
{
    public Task<IEnumerable<Movie>> ExecuteAsync(string? query, string? genre = null, int limit = 50, CancellationToken ct = default)
        => repo.SearchAsync(query, genre, limit, ct);
}
