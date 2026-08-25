namespace FrendaBibliotek.Api.Models;

public class Book
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public string? CoverUrl { get; set; }
    public int PublishedYear { get; set; }

    // Navigation
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
}
