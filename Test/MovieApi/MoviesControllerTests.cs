using Application.UseCases;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApi.Controllers;
using Xunit;

namespace Test.MovieApi
{
    public class MoviesControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkResult()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10) });

            var useCase = new GetAllMoviesUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.GetAll(useCase, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenMovieExists()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetByIdAsync("1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10));

            var useCase = new GetMovieByIdUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.GetById("1", useCase, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetByIdAsync("2", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Movie?)null);

            var useCase = new GetMovieByIdUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.GetById("2", useCase, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOkResult()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.SearchAsync("Test", "Action", 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10) });

            var useCase = new SearchMoviesUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.Search(useCase, "Test", "Action", 10, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Popular_ReturnsOkResult()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetPopularAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Popular", new List<string> { "Action" }, 2021, 9.0, 100) });

            var useCase = new GetPopularMoviesUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.Popular(useCase, 5, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Recommendations_ReturnsOkResult()
        {
            var repoMock = new Mock<IMovieRepository>();
            repoMock.Setup(r => r.GetRecommendationsAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie> { new Movie("Recommended", new List<string> { "Drama" }, 2022, 8.5, 80) });

            var useCase = new GetRecommendedMoviesUseCase(repoMock.Object);
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<MoviesController>>();
            var controller = new MoviesController(loggerMock.Object);

            var result = await controller.Recommendations(useCase, 3, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
