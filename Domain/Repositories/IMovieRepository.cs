using Domain.Entities;

namespace Domain.Repositories
{
    public interface IMovieRepository
    {
        // Listar todas las películas
        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default);

        // Obtener detalles por ID
        Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default);

        // Buscar por título o género
        Task<IEnumerable<Movie>> SearchAsync(string? query, string? genre = null, int limit = 50, CancellationToken ct = default);

        // Obtener populares (con límite)
        Task<IEnumerable<Movie>> GetPopularAsync(int limit = 20, CancellationToken ct = default);

        // Obtener recomendaciones (con límite)
        Task<IEnumerable<Movie>> GetRecommendationsAsync(int limit = 20, CancellationToken ct = default);

        // Crear una nueva película
        Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default);
    }
}
