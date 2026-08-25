namespace FrendaBibliotek.Api.DTOs;

// ─── Books ────────────────────────────────────────────────────────────────────

public record BookSummaryDto(
    int Id,
    string Title,
    string Author,
    string Genre,
    int PublishedYear,
    string? CoverUrl,
    int TotalCopies,
    int AvailableCopies,
    double? AvgReadingDays
);

public record BookDetailDto(
    int Id,
    string Title,
    string Author,
    string Genre,
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

public record BorrowRequest(int BookId);

public record LoanDto(
    int Id,
    int BookId,
    string BookTitle,
    string BookAuthor,
    string? CoverUrl,
    DateTime BorrowedAt,
    DateTime? ReturnedAt
);

// ─── Users ────────────────────────────────────────────────────────────────────

public record UserDto(int Id, string Name, string Email);
