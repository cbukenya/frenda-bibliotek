namespace FrendaBibliotek.Api.Models;

public class Loan
{
    public int Id { get; set; }
    public int BookCopyId { get; set; }
    public int UserId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }

    // Navigation
    public BookCopy BookCopy { get; set; } = null!;
    public User User { get; set; } = null!;
}
