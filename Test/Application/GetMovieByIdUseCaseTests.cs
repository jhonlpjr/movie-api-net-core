using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Moq;
using Xunit;

namespace MovieApi.Tests.Application
{
    public class GetMovieByIdUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsMovieById()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10));

            var useCase = new GetMovieByIdUseCase(repoMock.Object);

            var result = await useCase.ExecuteAsync("1");

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }
    }
}
