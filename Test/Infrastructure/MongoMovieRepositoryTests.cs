using Domain.Entities;
using Infrastructure.Persistence.Mongo;
using Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Test.Infrastructure
{
    public class MongoMovieRepositoryTests
    {

        [Fact]
        public async Task GetAllAsync_ReturnsMappedMovies()
        {
            // Arrange
            var docs = new List<MovieDocument> {
                new() { Id = "1", Title = "Test", Genre = new() { "Action" }, Year = 2020, Rating = 8.0, Popularity = 10 }
            };

            // Cursor simulado (prepárate para ambos: MoveNext y MoveNextAsync)
            var cursorMock = new Mock<IAsyncCursor<MovieDocument>>();
            cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>()))
                      .Returns(true).Returns(false);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true).ReturnsAsync(false);
            cursorMock.SetupGet(c => c.Current).Returns(docs);

            // Colección → FindAsync (método de instancia, no extensión)
            var colMock = new Mock<IMongoCollection<MovieDocument>>();
            colMock.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<MovieDocument>>(),
                    It.IsAny<FindOptions<MovieDocument, MovieDocument>>(),
                    It.IsAny<CancellationToken>()))
                  .ReturnsAsync(cursorMock.Object);

            var dbMock = new Mock<IMongoDatabase>();
            dbMock.Setup(db => db.GetCollection<MovieDocument>("movies", It.IsAny<MongoCollectionSettings>()))
                  .Returns(colMock.Object);

            var repo = new MongoMovieRepository(dbMock.Object, Mock.Of<ILogger<MongoMovieRepository>>());

            // Act
            var result = (await repo.GetAllAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test", result[0].Title);
        }

        [Fact]
        public async Task CreateAsync_AssignsId()
        {
            var dbMock = new Mock<IMongoDatabase>();
            var colMock = new Mock<IMongoCollection<MovieDocument>>();
            dbMock.Setup(db => db.GetCollection<MovieDocument>("movies", null)).Returns(colMock.Object);

            var loggerMock = new Mock<ILogger<MongoMovieRepository>>();
            var repo = new MongoMovieRepository(dbMock.Object, loggerMock.Object);

            var movie = new Movie("Test", new List<string> { "Action" }, 2020, 8.0, 10);

            colMock
                .Setup(c => c.InsertOneAsync(It.IsAny<MovieDocument>(), null, default))
                .Callback<MovieDocument, InsertOneOptions, CancellationToken>((doc, _, _) => doc.Id = "mocked-id")
                .Returns(Task.CompletedTask);

            var result = await repo.CreateAsync(movie);

            Assert.NotNull(result.Id);
        }
    }
}