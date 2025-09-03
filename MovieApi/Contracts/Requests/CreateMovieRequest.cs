using System.ComponentModel.DataAnnotations;

namespace MovieApi.Contracts.Requests;

public class CreateMovieRequest
{
    [Required, StringLength(200)]
    public string Title { get; set; } = default!;

    [MinLength(1)]
    public List<string> Genre { get; set; } = new();

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0, 10)]
    public double Rating { get; set; }

    public int Popularity { get; set; }
    public string? Description { get; set; }
}
