using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace MovieApi.Tests.Application
{
    public class GetRecommendedMoviesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsRecommendedMovies()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetRecommendationsAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Recommended", new List<string> { "Drama" }, 2022, 8.5, 80) });

            var useCase = new GetRecommendedMoviesUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync(3);

            Assert.Single(result);
            Assert.Equal("Recommended", ((List<Movie>)result)[0].Title);
        }
    }
}
