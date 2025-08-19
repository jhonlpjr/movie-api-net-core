using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Caching;
using Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MovieApi.Tests.Infrastructure
{
    public class CachedMovieRepositoryTests
    {
        private CachedMovieRepository CreateRepo(
            Mock<IMovieRepository>? innerMock = null,
            Mock<IDistributedCache>? cacheMock = null)
        {
            var cacheOptions = Options.Create(new CacheOptions { PopularMoviesTtlMinutes = 10, RecommendationsTtlMinutes = 10 });
            var loggerMock = new Mock<ILogger<CachedMovieRepository>>();
            return new CachedMovieRepository(
                innerMock?.Object ?? new Mock<IMovieRepository>().Object,
                cacheMock?.Object ?? new Mock<IDistributedCache>().Object,
                cacheOptions,
                loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromCache_IfExists()
        {
            var cacheMock = new Mock<IDistributedCache>();
            var movies = new List<Movie> { new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10) };
            // Configura el método base GetAsync en vez del método de extensión GetStringAsync
            cacheMock.Setup(c => c.GetAsync("movies:all", It.IsAny<CancellationToken>()))
                .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(movies)));
            var repo = CreateRepo(cacheMock: cacheMock);

            var result = await repo.GetAllAsync();

            Assert.Single(result);
            Assert.Equal("Test", ((List<Movie>)result)[0].Title);
        }
    }
}