﻿using Api.Mapping;
using Application.Services;
using Asp.Versioning;
using Contracts.Requests;
using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var isMovieUpdated = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

        return isMovieUpdated ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var isRatingDeleted = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);

        return isRatingDeleted ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);

        var ratingsResponse = ratings.MapToResponse();
        
        return Ok(ratingsResponse);
    }
}
