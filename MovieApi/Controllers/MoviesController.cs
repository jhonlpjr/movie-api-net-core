using Application.UseCases;
using Application.Common;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Mappings;
using MovieApi.Contracts.Responses;
using MovieApi.Contracts.Requests;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/v1/movies")]
[Produces("application/json")]
public class MoviesController : ControllerBase
{
    private readonly ILogger<MoviesController> _logger;
    public MoviesController(ILogger<MoviesController> logger) => _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MovieResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> GetAll(
        [FromServices] GetAllMoviesUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("GET /api/v1/movies requested");
        var items = await uc.ExecuteAsync(ct);
        var meta = new MetaResponse { limit = items.Count(), };
        var response = new ApiResponse<IEnumerable<MovieResponse>>(items.ToResponse(), meta);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MovieResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> GetById(
        string id,
        [FromServices] GetMovieByIdUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("GET /api/v1/movies/{Id}", id);
        var movie = await uc.ExecuteAsync(id, ct);
        if (movie is null) return NotFound();
        var response = new ApiResponse<MovieResponse>(movie.ToResponse());
        return Ok(response);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MovieResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> Search(
        [FromServices] SearchMoviesUseCase uc,
        [FromQuery] SearchMoviesRequest req,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "GET /api/v1/movies/search params: query={Query}, genre={Genre}, yearFrom={YearFrom}, yearTo={YearTo}, popularity={Popularity}, rating={Rating}, orderBy={OrderBy}, orderDirection={OrderDirection}, limit={Limit}",
            req.Query, req.Genre, req.YearFrom, req.YearTo, req.Popularity, req.Rating, req.OrderBy, req.OrderDirection, req.Limit);

        var items = await uc.ExecuteAsync(
            req.Query,
            req.Genre?.ToString(),
            req.YearFrom,
            req.YearTo,
            req.Popularity,
            req.Rating,
            req.OrderBy?.ToString(),
            req.OrderDirection,
            req.Limit,
            ct);

        var meta = new MetaResponse { limit = req.Limit };
        var response = new ApiResponse<IEnumerable<MovieResponse>>(items.ToResponse(), meta);
        return Ok(response);
    }

    [HttpGet("popular")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MovieResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> Popular(
        [FromServices] GetPopularMoviesUseCase uc,
        [FromQuery] int limit = Pagination.DefaultPageSize,
        CancellationToken ct = default)
    {
        _logger.LogInformation("GET /api/v1/movies/popular?limit={Limit}", limit);
        var items = await uc.ExecuteAsync(limit, ct);
        var meta = new MetaResponse { limit = limit };
        var response = new ApiResponse<IEnumerable<MovieResponse>>(items.ToResponse(), meta);
        return Ok(response);
    }

    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MovieResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MovieResponse>>>> Recommendations(
        [FromServices] GetRecommendedMoviesUseCase uc,
        [FromQuery] int limit = Pagination.DefaultPageSize,
        CancellationToken ct = default)
    {
        _logger.LogInformation("GET /api/v1/movies/recommendations?limit={Limit}", limit);
        var items = await uc.ExecuteAsync(limit, ct);
        var meta = new MetaResponse { limit = limit };
        var response = new ApiResponse<IEnumerable<MovieResponse>>(items.ToResponse(), meta);
        return Ok(response);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<MovieResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> Create(
        [FromBody] CreateMovieRequest req,
        [FromServices] CreateMovieUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("POST /api/v1/movies creating {Title}", req.Title);

        var created = await uc.ExecuteAsync(req.Title, req.Genre, req.Year, req.Rating, req.Popularity, req.Description, ct);
        var resp = created.ToResponse();
        var response = new ApiResponse<MovieResponse>(resp);

        _logger.LogInformation("Movie created {Id}", resp.Id);
        return CreatedAtAction(nameof(GetById), new { id = resp.Id }, response);
    }

    [HttpPatch("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<MovieResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MovieResponse>>> Patch(
        string id,
        [FromBody] UpdateMovieRequest req,
        [FromServices] UpdateMovieUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("PATCH /api/v1/movies/{Id} actualizando campos", id);

        var updated = await uc.ExecutePatchAsync(
            id,
            req.Title,
            req.Genre,
            req.Year,
            req.Rating,
            req.Popularity,
            req.Description,
            ct);

        if (updated is null) return NotFound();
        var resp = updated.ToResponse();
        var response = new ApiResponse<MovieResponse>(resp);

        _logger.LogInformation("Movie patched {Id}", resp.Id);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] DeleteMovieUseCase uc,
        CancellationToken ct)
    {
        _logger.LogInformation("DELETE /api/v1/movies/{Id}", id);
        var deleted = await uc.ExecuteAsync(id, ct);
        if (!deleted) return NotFound();
        _logger.LogInformation("Movie deleted {Id}", id);
        return NoContent();
    }
}
