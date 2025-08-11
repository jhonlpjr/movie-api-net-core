using Domain.Exceptions;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Movie
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public List<string> Genre { get; set; } = new();
    public double Rating { get; set; }
    public int Year { get; set; }
    public int Popularity { get; set; }
    public string? Description { get; set; }

    [JsonConstructor]
    public Movie(string title, List<string> genre, int year, double rating, int popularity, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title requerido");

        if (genre == null || !genre.Any(s => !string.IsNullOrWhiteSpace(s)))
            throw new DomainException("Genre requerido");

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new DomainException("Year inválido");

        Title = title.Trim();
        Genre = genre;
        Year = year;
        Rating = rating;
        Popularity = popularity;
        Description = description;
    }

    // Constructor de copia opcional
    public Movie(Movie movie)
        : this(movie.Title, movie.Genre, movie.Year, movie.Rating, movie.Popularity, movie.Description)
    { }
}