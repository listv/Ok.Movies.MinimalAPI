﻿using System.Text.RegularExpressions;

namespace Ok.Movies.MinimalAPI.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public string Slug => GenerateSlug();

    public float? Rating { get; set; }

    public int? UserRating { get; set; }

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; init; } = new();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex()
            .Replace(Title, string.Empty) // Remove all except alphanumeric symbols, spaces, underscores and hyphens
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
