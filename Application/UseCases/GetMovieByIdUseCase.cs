using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;
public class GetMovieByIdUseCase(IMovieRepository repo)
{
    public Task<Movie> ExecuteAsync(string id, CancellationToken ct = default)
        => repo.GetByIdAsync(id, ct);
}
