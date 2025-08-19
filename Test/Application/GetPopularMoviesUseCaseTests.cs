using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace MovieApi.Tests.Application
{
    public class GetPopularMoviesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsPopularMovies()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetPopularAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Popular", new List<string> { "Action" }, 2021, 9.0, 100) });

            var useCase = new GetPopularMoviesUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync(5);

            Assert.Single(result);
            Assert.Equal("Popular", ((List<Movie>)result)[0].Title);
        }
    }
}
