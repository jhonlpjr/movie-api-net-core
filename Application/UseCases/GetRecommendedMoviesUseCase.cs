using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;

public class GetRecommendedMoviesUseCase(IMovieRepository repo)
{
    public Task<IEnumerable<Movie>> ExecuteAsync(int limit = 20, CancellationToken ct = default)
        => repo.GetRecommendationsAsync(limit, ct);
}
