namespace FrendaBibliotek.Api.DTOs;

// ─── Genres ───────────────────────────────────────────────────────────────────

public record GenreDto(
    int Id,
    string Name,
    string Slug,
    int? ParentId,
    IEnumerable<GenreDto>? Children = null
);

// ─── Authors ──────────────────────────────────────────────────────────────────

public record AuthorDto(
    int Id,
    string Name,
    string Slug
);

// ─── Books ────────────────────────────────────────────────────────────────────

public record BookSummaryDto(
    int Id,
    string ISBN,
    string Title,
    string Author,
    int AuthorId,
    string Genre,
    int GenreId,
    int PublishedYear,
    int TotalPages,
    string? CoverUrl,
    int TotalCopies,
    int AvailableCopies,
    double? AvgReadingDays
);

public record BookDetailDto(
    int Id,
    string ISBN,
    string Title,
    string Author,
    int AuthorId,
    string Genre,
    int GenreId,
    string Description,
    int PublishedYear,
    int TotalPages,
    string? CoverUrl,
    int TotalCopies,
    int AvailableCopies,
    double? AvgReadingDays,
    IEnumerable<BookSummaryDto> Recommendations
);

// ─── Loans ────────────────────────────────────────────────────────────────────

/// <summary>Borrow a book by its ISBN — unambiguous and human-readable.</summary>
public record BorrowRequest(string ISBN);

public record LoanDto(
    int Id,
    int BookId,
    string ISBN,
    string BookTitle,
    string BookAuthor,
    string? CoverUrl,
    DateTime BorrowedAt,
    DateTime? ReturnedAt
);

// ─── Users ────────────────────────────────────────────────────────────────────

public record UserDto(int Id, string Name, string Email);

// ─── Auth ─────────────────────────────────────────────────────────────────────

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, UserDto User);
