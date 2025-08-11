using Application.UseCases;
using Application.Common;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Contracts;
using MovieApi.Contracts.Movies;
using MovieApi.Mappings;
using MovieApi.Responses;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/v1/movies")]
public class MoviesController : ControllerBase
{
    private readonly ILogger<MoviesController> _logger;
    public MoviesController(ILogger<MoviesController> logger) => _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromServices] GetAllMoviesUseCase uc, CancellationToken ct)
    {
        _logger.LogInformation("GET /api/v1/movies requested");
        var items = await uc.ExecuteAsync(ct);
        return Ok(ApiResponse<IEnumerable<MovieResponse>>.Ok(items.ToResponse()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromServices] GetMovieByIdUseCase uc, CancellationToken ct)
    {
        _logger.LogInformation("GET /api/v1/movies/{Id}", id);
        var movie = await uc.ExecuteAsync(id, ct);
        if (movie is null) return NotFound(); // Middleware cubrirá excepciones; aquí es un simple 404
        return Ok(ApiResponse<MovieResponse>.Ok(movie.ToResponse()));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromServices] SearchMoviesUseCase uc,
        [FromQuery] string? query,
        [FromQuery] string? genre,
        [FromQuery] int limit = Pagination.DefaultPageSize,

        CancellationToken ct = default)
    {
        _logger.LogInformation("GET /api/v1/movies/search?query={Query}&genre={Genre}&limit={Limit}", query, genre, limit);
        var items = await uc.ExecuteAsync(query, genre, limit, ct);
        return Ok(ApiResponse<IEnumerable<MovieResponse>>.Ok(items.ToResponse(), new { limit, query, genre }));
    }

    [HttpGet("popular")]
    public async Task<IActionResult> Popular(
        [FromServices] GetPopularMoviesUseCase uc,
        [FromQuery] int limit = Pagination.DefaultPageSize,
        CancellationToken ct = default)
    {
        _logger.LogInformation("GET /api/v1/movies/popular?limit={Limit}", limit);
        var items = await uc.ExecuteAsync(limit, ct);
        return Ok(ApiResponse<IEnumerable<MovieResponse>>.Ok(items.ToResponse(), new { limit, cache = "redis" }));
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> Recommendations(
        [FromServices] GetRecommendedMoviesUseCase uc,
        [FromQuery] int limit = Pagination.DefaultPageSize,
        CancellationToken ct = default)
    {
        _logger.LogInformation("GET /api/v1/movies/recommendations?limit={Limit}", limit);
        var items = await uc.ExecuteAsync(limit, ct);
        return Ok(ApiResponse<IEnumerable<MovieResponse>>.Ok(items.ToResponse(), new { limit, cache = "redis" }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMovieRequest req,
        [FromServices] CreateMovieUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("POST /api/v1/movies creating {Title}", req.Title);

        var created = await uc.ExecuteAsync(req.Title, req.Genre, req.Year, req.Rating, req.Popularity, req.Description, ct);
        var resp = created.ToResponse();

        _logger.LogInformation("Movie created {Id}", resp.Id);
        return CreatedAtAction(nameof(GetById), new { id = resp.Id }, ApiResponse<MovieResponse>.Ok(resp));
    }
}
