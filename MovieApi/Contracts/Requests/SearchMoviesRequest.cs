using Application.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.Contracts.Requests
{
    public class SearchMoviesRequest
    {
        [StringLength(100)]
        public string? Query { get; set; }

        [EnumDataType(typeof(MovieGenre))]
        public MovieGenre? Genre { get; set; }

        [Range(1, 200)]
        public int Limit { get; set; } = Pagination.DefaultPageSize;

        [Range(1900, 2100)]
        public int? YearFrom { get; set; }

        [Range(1900, 2100)]
        public int? YearTo { get; set; }

        [Range(0, int.MaxValue)]
        public int? Popularity { get; set; }

        [Range(0, 10)]
        public double? Rating { get; set; }

        [EnumDataType(typeof(MovieOrderBy))]
        public MovieOrderBy? OrderBy { get; set; }

        [RegularExpression("^(asc|desc)$", ErrorMessage = "OrderDirection debe ser 'asc' o 'desc'.")]
        public string? OrderDirection { get; set; } = "desc";
    }
}
