namespace FrendaBibliotek.Api.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    // Navigation
    public Genre? Parent { get; set; }
    public ICollection<Genre> Children { get; set; } = new List<Genre>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
