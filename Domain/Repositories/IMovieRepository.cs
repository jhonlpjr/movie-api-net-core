using Domain.Entities;

namespace Domain.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetPopularAsync(int limit);
        Task<Movie> GetByIdAsync(string id);
        Task<IEnumerable<Movie>> SearchAsync(string? query, string? genre, int limit);
    }
}
