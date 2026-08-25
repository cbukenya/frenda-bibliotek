namespace FrendaBibliotek.Api.Models;

public class Book
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public int TotalPages { get; set; }
    public string? CoverUrl { get; set; }
    public int PublishedYear { get; set; }

    // Navigation
    public Author Author { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
}
