namespace Application.Models;

public class MoviesFilteringOptions
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
}
