namespace FrendaBibliotek.Api.Models;

public enum UserType
{
    LibraryUser,
    Admin,
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.LibraryUser;

    // Navigation
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
