using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;

public class CreateMovieUseCase(IMovieRepository repo)
{
    public Task<Movie> ExecuteAsync(string title, List<string> genre, int year, double rating, int popularity, string? description, CancellationToken ct = default)
    {
        var entity = new Movie(title, genre, year, rating, popularity, description);
        return repo.CreateAsync(entity, ct);
    }
}
