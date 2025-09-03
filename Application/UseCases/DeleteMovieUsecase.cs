using Domain.Repositories;

namespace Application.UseCases;

public class DeleteMovieUseCase(IMovieRepository repo)
{
    public async Task<bool> ExecuteAsync(string id, CancellationToken ct = default)
    {
        // Se asume que el repositorio tiene un método DeleteAsync
        return await repo.DeleteAsync(id, ct);
    }
}