using Domain.Entities;
using MovieApi.Mappings;
using Xunit;

namespace Test.MovieApi
{
    public class MovieResponseMappingsTests
    {
        [Fact]
        public void ToResponse_MapsAllPropertiesCorrectly()
        {
            var movie = new Movie("Test Movie", new List<string> { "Action", "Drama" }, 2022, 8.5, 99, "Description");
            var response = movie.ToResponse();

            Assert.Equal(movie.Title, response.Title);
            Assert.Equal(movie.Genre, response.Genre);
            Assert.Equal(movie.Year, response.Year);
            Assert.Equal(movie.Rating, response.Rating);
            Assert.Equal(movie.Popularity, response.Popularity);
            Assert.Equal(movie.Description, response.Description);
        }
    }
}