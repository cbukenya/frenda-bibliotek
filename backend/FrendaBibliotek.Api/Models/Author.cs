namespace FrendaBibliotek.Api.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Bio { get; set; }

    // Navigation
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
