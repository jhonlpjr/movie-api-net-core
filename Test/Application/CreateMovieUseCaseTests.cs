using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace Test.Application
{
    public class CreateMovieUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_CreatesMovie()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.CreateAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Movie m, CancellationToken _) => m);

            var useCase = new CreateMovieUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync("New Movie", new List<string> { "Comedy" }, 2023, 7.5, 50, "A fun movie");

            Assert.NotNull(result);
            Assert.Equal("New Movie", result.Title);
            Assert.Contains("Comedy", result.Genre);
            Assert.Equal(2023, result.Year);
            Assert.Equal(7.5, result.Rating);
            Assert.Equal(50, result.Popularity);
            Assert.Equal("A fun movie", result.Description);
        }
    }
}
