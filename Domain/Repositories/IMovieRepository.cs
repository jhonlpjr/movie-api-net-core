using Domain.Entities;

namespace Domain.Repositories
{
    public interface IMovieRepository
    {
        // Listar todas las películas
        Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default);

        // Obtener detalles por ID
        Task<Movie?> GetByIdAsync(string id, CancellationToken ct = default);

        // Buscar por múltiples campos y ordenación
        Task<IEnumerable<Movie>> SearchAsync(
            string? query = null,
            string? genre = null,
            int? yearFrom = null,
            int? yearTo = null,
            int? popularity = null,
            double? rating = null,
            string? orderBy = null,
            string? orderDirection = null,
            int limit = 50,
            CancellationToken ct = default);

        // Obtener populares (con límite)
        Task<IEnumerable<Movie>> GetPopularAsync(int limit = 20, CancellationToken ct = default);

        // Obtener recomendaciones (con límite)
        Task<IEnumerable<Movie>> GetRecommendationsAsync(int limit = 20, CancellationToken ct = default);

        // Crear una nueva película
        Task<Movie> CreateAsync(Movie movie, CancellationToken ct = default);

        // Actualizar una película existente
        Task<Movie?> UpdateAsync(Movie movie, CancellationToken ct = default);

        // Eliminar una película por ID
        Task<bool> DeleteAsync(string id, CancellationToken ct = default);
    }
}
