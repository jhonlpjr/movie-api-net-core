using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace MovieApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly GetPopularMoviesUseCase _popular;
    public MoviesController(GetPopularMoviesUseCase popular) => _popular = popular;

    [HttpGet("popular")]
    public async Task<IActionResult> Popular([FromQuery] int limit = 20)
        => Ok(await _popular.ExecuteAsync(limit));
}
