using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases;

public class UpdateMovieUseCase(IMovieRepository repo)
{
    public async Task<Movie?> ExecutePatchAsync(
        string id,
        string? title = null,
        List<string>? genre = null,
        int? year = null,
        double? rating = null,
        int? popularity = null,
        string? description = null,
        CancellationToken ct = default)
    {
        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null) return null;

        if (title is not null) existing.Title = title;
        if (genre is not null) existing.Genre = genre;
        if (year is not null) existing.Year = year.Value;
        if (rating is not null) existing.Rating = rating.Value;
        if (popularity is not null) existing.Popularity = popularity.Value;
        if (description is not null) existing.Description = description;

        return await repo.UpdateAsync(existing, ct);
    }
}