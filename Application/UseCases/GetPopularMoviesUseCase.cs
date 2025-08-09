using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases
{
    public class GetPopularMoviesUseCase
    {
        private readonly IMovieRepository _repo;
        public GetPopularMoviesUseCase(IMovieRepository repo) => _repo = repo;
        public Task<IEnumerable<Movie>> ExecuteAsync(int limit = 20)
          => _repo.GetPopularAsync(limit);
    }
}
