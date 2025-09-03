using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace MovieApi.Tests.Application
{
    public class SearchMoviesUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsSearchResults()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.SearchAsync(
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<double?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10) });

            var useCase = new SearchMoviesUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync(
                query: "Test",
                genre: "Action",
                yearFrom: null,
                yearTo: null,
                popularity: null,
                rating: null,
                orderBy: null,
                orderDirection: null,
                limit: 10,
                ct: default);

            Assert.Single(result);
            Assert.Equal("Test", ((List<Movie>)result)[0].Title);
        }
    }
}
