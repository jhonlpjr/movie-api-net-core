using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace MovieApi.Tests.Application
{
    public class GetAllMoviesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsAllMovies()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10) });

            var useCase = new GetAllMoviesUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync();

            Assert.Single(result);
            Assert.Equal("Test", ((List<Movie>)result)[0].Title);
        }
    }
}
