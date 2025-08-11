// Infrastructure/Options/CacheOptions.cs
namespace Infrastructure.Options;

public class CacheOptions
{
    public int PopularMoviesTtlMinutes { get; set; } = 60;
    public int RecommendationsTtlMinutes { get; set; } = 120;
}