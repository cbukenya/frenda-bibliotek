using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Books.AnyAsync()) return;

        // ─── Books ────────────────────────────────────────────────────────────
        var books = new List<Book>
        {
            new() { Title = "The Design of Everyday Things", Author = "Don Norman", Genre = "Design", Description = "A powerful primer on how design serves as the interface between objects and users.", TotalPages = 368, PublishedYear = 2013 },
            new() { Title = "Clean Code", Author = "Robert C. Martin", Genre = "Programming", Description = "A handbook of agile software craftsmanship covering principles and patterns.", TotalPages = 464, PublishedYear = 2008 },
            new() { Title = "Thinking, Fast and Slow", Author = "Daniel Kahneman", Genre = "Psychology", Description = "An exploration of the two systems that drive the way we think.", TotalPages = 512, PublishedYear = 2011 },
            new() { Title = "Sapiens", Author = "Yuval Noah Harari", Genre = "History", Description = "A brief history of humankind from the Stone Age to the twenty-first century.", TotalPages = 443, PublishedYear = 2011 },
            new() { Title = "The Pragmatic Programmer", Author = "David Thomas & Andrew Hunt", Genre = "Programming", Description = "Your journey to mastery — timeless advice on the craft of software development.", TotalPages = 352, PublishedYear = 2019 },
            new() { Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction", Description = "The story of Paul Atreides on a desert planet that is the most valuable in the universe.", TotalPages = 688, PublishedYear = 1965 },
            new() { Title = "Educated", Author = "Tara Westover", Genre = "Memoir", Description = "A memoir about a young woman who kept out of school until age 17, then earns a PhD from Cambridge.", TotalPages = 352, PublishedYear = 2018 },
            new() { Title = "Atomic Habits", Author = "James Clear", Genre = "Self-Help", Description = "An easy and proven way to build good habits and break bad ones.", TotalPages = 320, PublishedYear = 2018 },
            new() { Title = "The Hitchhiker's Guide to the Galaxy", Author = "Douglas Adams", Genre = "Science Fiction", Description = "The hitchhiker's guide to everything — start with a bathrobe.", TotalPages = 193, PublishedYear = 1979 },
            new() { Title = "Meditations", Author = "Marcus Aurelius", Genre = "Philosophy", Description = "Personal writings of the Roman Emperor — a cornerstone of Stoic philosophy.", TotalPages = 254, PublishedYear = 180 },
        };

        db.Books.AddRange(books);
        await db.SaveChangesAsync();

        // ─── Copies (2–3 per book) ────────────────────────────────────────────
        var copies = books.SelectMany(b => Enumerable.Range(0, b.Genre == "Programming" ? 3 : 2)
            .Select(_ => new BookCopy { BookId = b.Id, AcquiredAt = DateTime.UtcNow.AddYears(-2) }))
            .ToList();

        db.BookCopies.AddRange(copies);
        await db.SaveChangesAsync();

        // ─── Users ────────────────────────────────────────────────────────────
        var users = new List<User>
        {
            new() { Name = "Alice Lindgren", Email = "alice@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Bob Eriksson", Email = "bob@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Clara Svensson", Email = "clara@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "David Johansson", Email = "david@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Eva Nilsson", Email = "eva@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Fredrik Berg", Email = "fredrik@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Greta Holm", Email = "greta@bibliotek.se", UserType = UserType.LibraryUser },
            new() { Name = "Hans Björk", Email = "hans@bibliotek.se", UserType = UserType.LibraryUser },
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        // ─── Historical loans (completed) ─────────────────────────────────────
        // Build a rich history to power the top list and recommendations.
        // Pattern: each user has read ~5-6 books, with overlapping tastes.
        var historicalLoans = new List<(int UserId, int BookIndex, int DaysAgo, int DurationDays)>
        {
            // Alice: Design + Programming + Psychology
            (1, 0, 200, 6), (1, 1, 180, 9), (1, 2, 150, 12), (1, 4, 100, 8), (1, 7, 50, 5),
            // Bob: Programming + Sci-Fi + Philosophy
            (2, 1, 190, 10), (2, 4, 160, 7), (2, 5, 130, 20), (2, 8, 90, 4), (2, 9, 40, 8),
            // Clara: History + Memoir + Self-Help
            (3, 3, 210, 14), (3, 6, 170, 11), (3, 7, 140, 6), (3, 2, 100, 13), (3, 9, 60, 9),
            // David: Design + Psychology + Philosophy
            (4, 0, 220, 7), (4, 2, 185, 10), (4, 9, 155, 8), (4, 3, 110, 16), (4, 7, 55, 5),
            // Eva: Sci-Fi + Memoir + Self-Help
            (5, 5, 230, 18), (5, 6, 195, 12), (5, 7, 160, 6), (5, 8, 120, 4), (5, 4, 70, 8),
            // Fredrik: Programming + Design + Self-Help
            (6, 1, 240, 11), (6, 0, 200, 8), (6, 4, 165, 7), (6, 7, 125, 5), (6, 9, 75, 9),
            // Greta: History + Psychology + Sci-Fi
            (7, 3, 250, 15), (7, 2, 210, 11), (7, 5, 175, 22), (7, 8, 130, 5), (7, 6, 80, 10),
            // Hans: All genres — the library's power user
            (8, 0, 260, 6), (8, 1, 230, 9), (8, 2, 200, 12), (8, 3, 170, 14),
            (8, 4, 140, 8), (8, 5, 110, 19), (8, 6, 80, 11), (8, 7, 50, 5),
        };

        var copyPool = copies.GroupBy(c => c.BookId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var usedCopySlots = new Dictionary<int, DateTime>(); // copyId → next available from

        foreach (var (userId, bookIndex, daysAgo, duration) in historicalLoans)
        {
            var book = books[bookIndex];
            var available = copyPool[book.Id]
                .FirstOrDefault(c => !usedCopySlots.ContainsKey(c.Id) ||
                                     usedCopySlots[c.Id] < DateTime.UtcNow.AddDays(-daysAgo));

            if (available is null) continue;

            var borrowed = DateTime.UtcNow.AddDays(-daysAgo);
            var returned = borrowed.AddDays(duration);

            db.Loans.Add(new Loan
            {
                BookCopyId = available.Id,
                UserId = userId,
                BorrowedAt = borrowed,
                ReturnedAt = returned,
            });

            usedCopySlots[available.Id] = returned;
        }

        await db.SaveChangesAsync();

        // ─── Active loans for Alice (the demo user) ────────────────────────────
        var cleanCode = books[1]; // Clean Code
        var dune = books[5];      // Dune

        var ccCopy = copyPool[cleanCode.Id].FirstOrDefault(c =>
            !usedCopySlots.ContainsKey(c.Id) || usedCopySlots[c.Id] < DateTime.UtcNow.AddDays(-3));
        var duneCopy = copyPool[dune.Id].FirstOrDefault(c =>
            !usedCopySlots.ContainsKey(c.Id) || usedCopySlots[c.Id] < DateTime.UtcNow.AddDays(-5));

        if (ccCopy is not null)
            db.Loans.Add(new Loan { BookCopyId = ccCopy.Id, UserId = 1, BorrowedAt = DateTime.UtcNow.AddDays(-3) });
        if (duneCopy is not null)
            db.Loans.Add(new Loan { BookCopyId = duneCopy.Id, UserId = 1, BorrowedAt = DateTime.UtcNow.AddDays(-5) });

        await db.SaveChangesAsync();
    }
}
