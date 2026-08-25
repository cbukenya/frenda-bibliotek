namespace FrendaBibliotek.Api.Models;

public class BookCopy
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public DateTime AcquiredAt { get; set; }

    // Navigation
    public Book Book { get; set; } = null!;
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
