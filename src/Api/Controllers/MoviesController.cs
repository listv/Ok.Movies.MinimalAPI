using Api.Mapping;
using Application.Services;
using Asp.Versioning;
using Contracts.Requests;
using Contracts.Responses;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using static Infrastructure.Cache.CacheExtensions;

namespace Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[ApiVersion(2.0)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IOutputCacheStore _cacheStore;

    public MoviesController(IMovieService movieService
        , IOutputCacheStore cacheStore
        )
    {
        _movieService = movieService;
        _cacheStore = cacheStore;
    }

    // [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<ActionResult<MovieResponse>> CreateV1([FromBody] CreateMovieRequest request, CancellationToken token = default)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);
        await _cacheStore.EvictByTagAsync(MoviesTag, token);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(GetV1), new { idOrSlug = response.Id }, response);
    }

    // [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache(PolicyName = MovieCachePolicy)]
    [MapToApiVersion(1.0)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<ActionResult<MovieResponse>> GetV1([FromRoute] string idOrSlug, CancellationToken token = default)
    {
        var user = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, user, token)
            : await _movieService.GetBySlugAsync(idOrSlug, user, token);

        if (movie is null) return NotFound();

        var movieResponse = movie.MapToResponse();
        return Ok(movieResponse);
    }

    [MapToApiVersion(2.0)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<ActionResult<MovieResponse>> GetV2([FromRoute] string idOrSlug, CancellationToken token = default)
    {
        var user = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, user, token)
            : await _movieService.GetBySlugAsync(idOrSlug, user, token);

        if (movie is null) return NotFound();

        var movieResponse = movie.MapToResponse();
        return Ok(movieResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    [OutputCache(PolicyName = MovieCachePolicy)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(HttpValidationProblemDetails))]
    [ProducesDefaultResponseType(typeof(ValidationProblemDetails))]
    public async Task<ActionResult<MoviesResponse>> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();

        var options = request.MapToOptions()
            .WithUser(userId);

        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

        var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);

        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken token = default)
    {
        var user = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, user, token);
        if (updatedMovie is null) return NotFound();

        await _cacheStore.EvictByTagAsync(MoviesTag, token);
        
        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token = default)
    {
        var isDeleted = await _movieService.DeleteByIdAsync(id, token);

        if (!isDeleted) return NotFound();
        
        await _cacheStore.EvictByTagAsync(MoviesTag, token);

        return Ok();
    }
}
