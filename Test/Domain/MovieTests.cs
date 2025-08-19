using Domain.Entities;
using Domain.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace MovieApi.Tests.Domain
{
    public class MovieTests
    {
        [Fact]
        public void Constructor_Throws_WhenTitleIsEmpty()
        {
            Assert.Throws<DomainException>(() => new Movie("", new List<string> { "Action" }, 2020, 8.0, 10));
        }

        [Fact]
        public void Constructor_Throws_WhenGenreIsEmpty()
        {
            Assert.Throws<DomainException>(() => new Movie("Test", new List<string>(), 2020, 8.0, 10));
        }
    }
}