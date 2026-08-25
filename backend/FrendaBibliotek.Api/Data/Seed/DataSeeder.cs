using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Genres.AnyAsync()) return;

        // ─── Genres (hierarchical tree) ───────────────────────────────────────
        var fiction    = new Genre { Name = "Fiction",     Slug = "fiction" };
        var nonFiction = new Genre { Name = "Non-Fiction", Slug = "non-fiction" };

        db.Genres.AddRange(fiction, nonFiction);
        await db.SaveChangesAsync();

        var sciFi           = new Genre { Name = "Science Fiction", Slug = "science-fiction",   ParentId = fiction.Id };
        var fantasy         = new Genre { Name = "Fantasy",         Slug = "fantasy",            ParentId = fiction.Id };
        var literaryFiction = new Genre { Name = "Literary Fiction", Slug = "literary-fiction", ParentId = fiction.Id };

        var technology = new Genre { Name = "Technology", Slug = "technology", ParentId = nonFiction.Id };
        var science    = new Genre { Name = "Science",    Slug = "science",    ParentId = nonFiction.Id };
        var history    = new Genre { Name = "History",    Slug = "history",    ParentId = nonFiction.Id };
        var philosophy = new Genre { Name = "Philosophy", Slug = "philosophy", ParentId = nonFiction.Id };
        var selfHelp   = new Genre { Name = "Self-Help",  Slug = "self-help",  ParentId = nonFiction.Id };
        var memoir     = new Genre { Name = "Memoir",     Slug = "memoir",     ParentId = nonFiction.Id };

        db.Genres.AddRange(sciFi, fantasy, literaryFiction, technology, science, history, philosophy, selfHelp, memoir);
        await db.SaveChangesAsync();

        var spaceOpera   = new Genre { Name = "Space Opera",   Slug = "space-opera",   ParentId = sciFi.Id };
        var cyberpunk    = new Genre { Name = "Cyberpunk",     Slug = "cyberpunk",      ParentId = sciFi.Id };
        var epicFantasy  = new Genre { Name = "Epic Fantasy",  Slug = "epic-fantasy",   ParentId = fantasy.Id };
        var urbanFantasy = new Genre { Name = "Urban Fantasy", Slug = "urban-fantasy",  ParentId = fantasy.Id };
        var programming  = new Genre { Name = "Programming",   Slug = "programming",    ParentId = technology.Id };
        var design       = new Genre { Name = "Design",        Slug = "design",         ParentId = technology.Id };
        var psychology   = new Genre { Name = "Psychology",     Slug = "psychology",     ParentId = science.Id };
        var physics      = new Genre { Name = "Physics",       Slug = "physics",        ParentId = science.Id };

        db.Genres.AddRange(spaceOpera, cyberpunk, epicFantasy, urbanFantasy, programming, design, psychology, physics);
        await db.SaveChangesAsync();

        var agile     = new Genre { Name = "Agile & Craftsmanship", Slug = "agile-craftsmanship", ParentId = programming.Id };
        var systems   = new Genre { Name = "Systems Programming",   Slug = "systems-programming", ParentId = programming.Id };
        var uxDesign  = new Genre { Name = "UX Design",             Slug = "ux-design",           ParentId = design.Id };
        var cognitive = new Genre { Name = "Cognitive Psychology",   Slug = "cognitive-psychology", ParentId = psychology.Id };
        var military  = new Genre { Name = "Military Sci-Fi",       Slug = "military-sci-fi",     ParentId = spaceOpera.Id };

        db.Genres.AddRange(agile, systems, uxDesign, cognitive, military);
        await db.SaveChangesAsync();

        // ─── Authors ──────────────────────────────────────────────────────────
        var donNorman   = new Author { Name = "Don Norman",               Slug = "don-norman" };
        var bobMartin   = new Author { Name = "Robert C. Martin",         Slug = "robert-c-martin" };
        var kahneman    = new Author { Name = "Daniel Kahneman",          Slug = "daniel-kahneman" };
        var harari      = new Author { Name = "Yuval Noah Harari",        Slug = "yuval-noah-harari" };
        var thomasHunt  = new Author { Name = "David Thomas & Andrew Hunt", Slug = "david-thomas-andrew-hunt" };
        var herbert     = new Author { Name = "Frank Herbert",            Slug = "frank-herbert" };
        var westover    = new Author { Name = "Tara Westover",            Slug = "tara-westover" };
        var clear       = new Author { Name = "James Clear",              Slug = "james-clear" };
        var adams       = new Author { Name = "Douglas Adams",            Slug = "douglas-adams" };
        var aurelius    = new Author { Name = "Marcus Aurelius",          Slug = "marcus-aurelius" };

        db.Authors.AddRange(donNorman, bobMartin, kahneman, harari, thomasHunt, herbert, westover, clear, adams, aurelius);
        await db.SaveChangesAsync();

        // ─── Books ────────────────────────────────────────────────────────────
        var books = new List<Book>
        {
            new() { ISBN = "9780465050659", Title = "The Design of Everyday Things", AuthorId = donNorman.Id, GenreId = uxDesign.Id, Description = "A powerful primer on how design serves as the interface between objects and users.", TotalPages = 368, PublishedYear = 2013, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780465050659-L.jpg" },
            new() { ISBN = "9780132350884", Title = "Clean Code", AuthorId = bobMartin.Id, GenreId = agile.Id, Description = "A handbook of agile software craftsmanship covering principles and patterns.", TotalPages = 464, PublishedYear = 2008, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg" },
            new() { ISBN = "9780374533557", Title = "Thinking, Fast and Slow", AuthorId = kahneman.Id, GenreId = cognitive.Id, Description = "An exploration of the two systems that drive the way we think.", TotalPages = 512, PublishedYear = 2011, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780374533557-L.jpg" },
            new() { ISBN = "9780062316097", Title = "Sapiens", AuthorId = harari.Id, GenreId = history.Id, Description = "A brief history of humankind from the Stone Age to the twenty-first century.", TotalPages = 443, PublishedYear = 2011, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg" },
            new() { ISBN = "9780135957059", Title = "The Pragmatic Programmer", AuthorId = thomasHunt.Id, GenreId = agile.Id, Description = "Your journey to mastery — timeless advice on the craft of software development.", TotalPages = 352, PublishedYear = 2019, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780135957059-L.jpg" },
            new() { ISBN = "9780441013593", Title = "Dune", AuthorId = herbert.Id, GenreId = spaceOpera.Id, Description = "The story of Paul Atreides on a desert planet that is the most valuable in the universe.", TotalPages = 688, PublishedYear = 1965, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780441013593-L.jpg" },
            new() { ISBN = "9780399590504", Title = "Educated", AuthorId = westover.Id, GenreId = memoir.Id, Description = "A memoir about a young woman who kept out of school until age 17, then earns a PhD from Cambridge.", TotalPages = 352, PublishedYear = 2018, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780399590504-L.jpg" },
            new() { ISBN = "9780735211292", Title = "Atomic Habits", AuthorId = clear.Id, GenreId = selfHelp.Id, Description = "An easy and proven way to build good habits and break bad ones.", TotalPages = 320, PublishedYear = 2018, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780735211292-L.jpg" },
            new() { ISBN = "9780345391803", Title = "The Hitchhiker's Guide to the Galaxy", AuthorId = adams.Id, GenreId = sciFi.Id, Description = "The hitchhiker's guide to everything — start with a bathrobe.", TotalPages = 193, PublishedYear = 1979, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780345391803-L.jpg" },
            new() { ISBN = "9780140449334", Title = "Meditations", AuthorId = aurelius.Id, GenreId = philosophy.Id, Description = "Personal writings of the Roman Emperor — a cornerstone of Stoic philosophy.", TotalPages = 254, PublishedYear = 180, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780140449334-L.jpg" },
        };

        db.Books.AddRange(books);
        await db.SaveChangesAsync();

        // ─── Copies (2–3 per book) ────────────────────────────────────────────
        var copies = books.SelectMany(b => Enumerable.Range(0, b.GenreId == agile.Id ? 3 : 2)
            .Select(_ => new BookCopy { BookId = b.Id, AcquiredAt = DateTime.UtcNow.AddYears(-2) }))
            .ToList();

        db.BookCopies.AddRange(copies);
        await db.SaveChangesAsync();

        // ─── Users ────────────────────────────────────────────────────────────
        var demoHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var users = new List<User>
        {
            new() { Name = "Alice Lindgren", Email = "alice@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Bob Eriksson", Email = "bob@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Clara Svensson", Email = "clara@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "David Johansson", Email = "david@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Eva Nilsson", Email = "eva@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Fredrik Berg", Email = "fredrik@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Greta Holm", Email = "greta@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
            new() { Name = "Hans Björk", Email = "hans@bibliotek.se", PasswordHash = demoHash, UserType = UserType.LibraryUser },
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        // ─── Historical loans (completed) ─────────────────────────────────────
        var historicalLoans = new List<(int UserId, int BookIndex, int DaysAgo, int DurationDays)>
        {
            (1, 0, 200, 6), (1, 1, 180, 9), (1, 2, 150, 12), (1, 4, 100, 8), (1, 7, 50, 5),
            (2, 1, 190, 10), (2, 4, 160, 7), (2, 5, 130, 20), (2, 8, 90, 4), (2, 9, 40, 8),
            (3, 3, 210, 14), (3, 6, 170, 11), (3, 7, 140, 6), (3, 2, 100, 13), (3, 9, 60, 9),
            (4, 0, 220, 7), (4, 2, 185, 10), (4, 9, 155, 8), (4, 3, 110, 16), (4, 7, 55, 5),
            (5, 5, 230, 18), (5, 6, 195, 12), (5, 7, 160, 6), (5, 8, 120, 4), (5, 4, 70, 8),
            (6, 1, 240, 11), (6, 0, 200, 8), (6, 4, 165, 7), (6, 7, 125, 5), (6, 9, 75, 9),
            (7, 3, 250, 15), (7, 2, 210, 11), (7, 5, 175, 22), (7, 8, 130, 5), (7, 6, 80, 10),
            (8, 0, 260, 6), (8, 1, 230, 9), (8, 2, 200, 12), (8, 3, 170, 14),
            (8, 4, 140, 8), (8, 5, 110, 19), (8, 6, 80, 11), (8, 7, 50, 5),
        };

        var copyPool = copies.GroupBy(c => c.BookId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var usedCopySlots = new Dictionary<int, DateTime>();

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
        var cleanCode = books[1];
        var dune = books[5];

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
