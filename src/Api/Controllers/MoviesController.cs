using Api.Mapping;
using Application.Services;
using Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token = default)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token).ConfigureAwait(false);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = response.Id }, response);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token = default)
    {
        var movie = Guid.TryParse(idOrSlug, out Guid id)
            ? await _movieService.GetByIdAsync(id, token).ConfigureAwait(false)
            : await _movieService.GetBySlugAsync(idOrSlug, token).ConfigureAwait(false);
        
        if (movie is null)
        {
            return NotFound();
        }

        var movieResponse = movie.MapToResponse();
        return Ok(movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token = default)
    {
        var movies = await _movieService.GetAllAsync(token).ConfigureAwait(false);

        var moviesResponse = movies.MapToResponse();

        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token = default)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, token).ConfigureAwait(false);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token = default)
    {
        var isDeleted = await _movieService.DeleteByIdAsync(id, token).ConfigureAwait(false);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
