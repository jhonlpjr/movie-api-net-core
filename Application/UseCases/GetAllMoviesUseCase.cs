using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;
public class GetAllMoviesUseCase(IMovieRepository repo)
{
    public Task<IEnumerable<Movie>> ExecuteAsync(CancellationToken ct = default)
        => repo.GetAllAsync(ct);
}
