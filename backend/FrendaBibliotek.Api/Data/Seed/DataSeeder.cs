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
        var thriller        = new Genre { Name = "Thriller",        Slug = "thriller",           ParentId = fiction.Id };
        var horror          = new Genre { Name = "Horror",          Slug = "horror",             ParentId = fiction.Id };
        var romance         = new Genre { Name = "Romance",         Slug = "romance",            ParentId = fiction.Id };

        var technology = new Genre { Name = "Technology", Slug = "technology", ParentId = nonFiction.Id };
        var science    = new Genre { Name = "Science",    Slug = "science",    ParentId = nonFiction.Id };
        var history    = new Genre { Name = "History",    Slug = "history",    ParentId = nonFiction.Id };
        var philosophy = new Genre { Name = "Philosophy", Slug = "philosophy", ParentId = nonFiction.Id };
        var selfHelp   = new Genre { Name = "Self-Help",  Slug = "self-help",  ParentId = nonFiction.Id };
        var memoir     = new Genre { Name = "Memoir",     Slug = "memoir",     ParentId = nonFiction.Id };
        var business   = new Genre { Name = "Business",   Slug = "business",   ParentId = nonFiction.Id };

        db.Genres.AddRange(sciFi, fantasy, literaryFiction, thriller, horror, romance, technology, science, history, philosophy, selfHelp, memoir, business);
        await db.SaveChangesAsync();

        var spaceOpera   = new Genre { Name = "Space Opera",   Slug = "space-opera",   ParentId = sciFi.Id };
        var cyberpunk    = new Genre { Name = "Cyberpunk",     Slug = "cyberpunk",      ParentId = sciFi.Id };
        var dystopian    = new Genre { Name = "Dystopian",     Slug = "dystopian",      ParentId = sciFi.Id };
        var epicFantasy  = new Genre { Name = "Epic Fantasy",  Slug = "epic-fantasy",   ParentId = fantasy.Id };
        var urbanFantasy = new Genre { Name = "Urban Fantasy", Slug = "urban-fantasy",  ParentId = fantasy.Id };
        var programming  = new Genre { Name = "Programming",   Slug = "programming",    ParentId = technology.Id };
        var design       = new Genre { Name = "Design",        Slug = "design",         ParentId = technology.Id };
        var psychology   = new Genre { Name = "Psychology",     Slug = "psychology",     ParentId = science.Id };
        var physics      = new Genre { Name = "Physics",       Slug = "physics",        ParentId = science.Id };
        var biology      = new Genre { Name = "Biology",       Slug = "biology",        ParentId = science.Id };

        db.Genres.AddRange(spaceOpera, cyberpunk, dystopian, epicFantasy, urbanFantasy, programming, design, psychology, physics, biology);
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
        // New authors for expanded catalog
        var orwell      = new Author { Name = "George Orwell",            Slug = "george-orwell" };
        var tolkien     = new Author { Name = "J.R.R. Tolkien",           Slug = "jrr-tolkien" };
        var asimov      = new Author { Name = "Isaac Asimov",             Slug = "isaac-asimov" };
        var martinGRR   = new Author { Name = "George R.R. Martin",       Slug = "george-rr-martin" };
        var rowling     = new Author { Name = "J.K. Rowling",             Slug = "jk-rowling" };
        var king        = new Author { Name = "Stephen King",             Slug = "stephen-king" };
        var hawking     = new Author { Name = "Stephen Hawking",          Slug = "stephen-hawking" };
        var sagan       = new Author { Name = "Carl Sagan",               Slug = "carl-sagan" };
        var gibson      = new Author { Name = "William Gibson",           Slug = "william-gibson" };
        var atwood      = new Author { Name = "Margaret Atwood",          Slug = "margaret-atwood" };
        var vonnegut    = new Author { Name = "Kurt Vonnegut",            Slug = "kurt-vonnegut" };
        var camus       = new Author { Name = "Albert Camus",             Slug = "albert-camus" };
        var huxley      = new Author { Name = "Aldous Huxley",            Slug = "aldous-huxley" };
        var bradbury    = new Author { Name = "Ray Bradbury",             Slug = "ray-bradbury" };
        var sanderson   = new Author { Name = "Brandon Sanderson",        Slug = "brandon-sanderson" };
        var leGuin      = new Author { Name = "Ursula K. Le Guin",        Slug = "ursula-k-le-guin" };
        var manson      = new Author { Name = "Mark Manson",              Slug = "mark-manson" };
        var duckworth   = new Author { Name = "Angela Duckworth",         Slug = "angela-duckworth" };
        var dweck       = new Author { Name = "Carol S. Dweck",           Slug = "carol-s-dweck" };
        var taleb       = new Author { Name = "Nassim Nicholas Taleb",    Slug = "nassim-nicholas-taleb" };
        var gladwell    = new Author { Name = "Malcolm Gladwell",         Slug = "malcolm-gladwell" };
        var fowler      = new Author { Name = "Martin Fowler",            Slug = "martin-fowler" };
        var knuth       = new Author { Name = "Donald E. Knuth",          Slug = "donald-e-knuth" };
        var gamma       = new Author { Name = "Erich Gamma et al.",       Slug = "erich-gamma-et-al" };
        var mcconnell   = new Author { Name = "Steve McConnell",          Slug = "steve-mcconnell" };
        var krug        = new Author { Name = "Steve Krug",               Slug = "steve-krug" };
        var newport     = new Author { Name = "Cal Newport",              Slug = "cal-newport" };
        var brown       = new Author { Name = "Brené Brown",              Slug = "brene-brown" };
        var obama       = new Author { Name = "Michelle Obama",           Slug = "michelle-obama" };
        var isaacson    = new Author { Name = "Walter Isaacson",          Slug = "walter-isaacson" };
        var pinker      = new Author { Name = "Steven Pinker",            Slug = "steven-pinker" };
        var dawkins     = new Author { Name = "Richard Dawkins",          Slug = "richard-dawkins" };
        var thiel       = new Author { Name = "Peter Thiel",              Slug = "peter-thiel" };
        var christie    = new Author { Name = "Agatha Christie",          Slug = "agatha-christie" };

        db.Authors.AddRange(
            donNorman, bobMartin, kahneman, harari, thomasHunt, herbert, westover, clear, adams, aurelius,
            orwell, tolkien, asimov, martinGRR, rowling, king, hawking, sagan, gibson, atwood,
            vonnegut, camus, huxley, bradbury, sanderson, leGuin, manson, duckworth, dweck, taleb,
            gladwell, fowler, knuth, gamma, mcconnell, krug, newport, brown, obama, isaacson,
            pinker, dawkins, thiel, christie
        );
        await db.SaveChangesAsync();

        // ─── Books (60+) ──────────────────────────────────────────────────────
        var books = new List<Book>
        {
            // ── Original 10 ──
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

            // ── Science Fiction ──
            new() { ISBN = "9780451524935", Title = "1984", AuthorId = orwell.Id, GenreId = dystopian.Id, Description = "A dystopian novel set in Airstrip One, a province of the superstate Oceania.", TotalPages = 328, PublishedYear = 1949, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg" },
            new() { ISBN = "9780060850524", Title = "Brave New World", AuthorId = huxley.Id, GenreId = dystopian.Id, Description = "A dystopian society where humans are genetically engineered and socially conditioned.", TotalPages = 288, PublishedYear = 1932, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780060850524-L.jpg" },
            new() { ISBN = "9781451673319", Title = "Fahrenheit 451", AuthorId = bradbury.Id, GenreId = dystopian.Id, Description = "A future American society where books are outlawed and firemen burn any that are found.", TotalPages = 194, PublishedYear = 1953, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781451673319-L.jpg" },
            new() { ISBN = "9780553293357", Title = "Foundation", AuthorId = asimov.Id, GenreId = spaceOpera.Id, Description = "The first novel in Asimov's Foundation trilogy — mathematical sociology predicts the fall of the Galactic Empire.", TotalPages = 244, PublishedYear = 1951, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780553293357-L.jpg" },
            new() { ISBN = "9780553382563", Title = "I, Robot", AuthorId = asimov.Id, GenreId = sciFi.Id, Description = "A collection of nine stories about the interaction of humans and robots.", TotalPages = 224, PublishedYear = 1950, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780553382563-L.jpg" },
            new() { ISBN = "9780441569595", Title = "Neuromancer", AuthorId = gibson.Id, GenreId = cyberpunk.Id, Description = "The classic cyberpunk novel that coined the term 'cyberspace'.", TotalPages = 271, PublishedYear = 1984, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780441569595-L.jpg" },
            new() { ISBN = "9780385333481", Title = "The Handmaid's Tale", AuthorId = atwood.Id, GenreId = dystopian.Id, Description = "Set in the near-future Republic of Gilead where women have lost all rights.", TotalPages = 311, PublishedYear = 1985, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780385333481-L.jpg" },
            new() { ISBN = "9780812511819", Title = "Slaughterhouse-Five", AuthorId = vonnegut.Id, GenreId = sciFi.Id, Description = "Billy Pilgrim's journey through time and his experiences in World War II.", TotalPages = 275, PublishedYear = 1969, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780812511819-L.jpg" },
            new() { ISBN = "9780441478125", Title = "The Left Hand of Darkness", AuthorId = leGuin.Id, GenreId = sciFi.Id, Description = "An envoy from Earth arrives on a planet where people have no fixed gender.", TotalPages = 304, PublishedYear = 1969, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780441478125-L.jpg" },
            new() { ISBN = "9780553803716", Title = "The End of Eternity", AuthorId = asimov.Id, GenreId = sciFi.Id, Description = "An organization called Eternity exists outside of time to prevent human suffering.", TotalPages = 191, PublishedYear = 1955, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780553803716-L.jpg" },

            // ── Fantasy ──
            new() { ISBN = "9780618640157", Title = "The Lord of the Rings", AuthorId = tolkien.Id, GenreId = epicFantasy.Id, Description = "The epic tale of the One Ring and the quest to destroy it in the fires of Mount Doom.", TotalPages = 1178, PublishedYear = 1954, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780618640157-L.jpg" },
            new() { ISBN = "9780547928227", Title = "The Hobbit", AuthorId = tolkien.Id, GenreId = epicFantasy.Id, Description = "Bilbo Baggins's unexpected journey with a group of dwarves to reclaim their mountain home.", TotalPages = 310, PublishedYear = 1937, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780547928227-L.jpg" },
            new() { ISBN = "9780553103540", Title = "A Game of Thrones", AuthorId = martinGRR.Id, GenreId = epicFantasy.Id, Description = "Noble families fight for control of the mythical land of Westeros in the first book of A Song of Ice and Fire.", TotalPages = 694, PublishedYear = 1996, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780553103540-L.jpg" },
            new() { ISBN = "9780439708180", Title = "Harry Potter and the Sorcerer's Stone", AuthorId = rowling.Id, GenreId = fantasy.Id, Description = "A boy discovers he is a wizard on his eleventh birthday and attends Hogwarts School.", TotalPages = 309, PublishedYear = 1997, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780439708180-L.jpg" },
            new() { ISBN = "9780765311788", Title = "Mistborn: The Final Empire", AuthorId = sanderson.Id, GenreId = epicFantasy.Id, Description = "In a world of ash and mist, a street thief discovers she has special allomantic powers.", TotalPages = 541, PublishedYear = 2006, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780765311788-L.jpg" },
            new() { ISBN = "9780765326355", Title = "The Way of Kings", AuthorId = sanderson.Id, GenreId = epicFantasy.Id, Description = "The first book of The Stormlight Archive — a world of massive storms and ancient warriors.", TotalPages = 1007, PublishedYear = 2010, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780765326355-L.jpg" },

            // ── Thriller / Horror / Literary ──
            new() { ISBN = "9781501142970", Title = "It", AuthorId = king.Id, GenreId = horror.Id, Description = "Seven adults return to their hometown to confront a nightmare they had first faced as teenagers.", TotalPages = 1138, PublishedYear = 1986, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781501142970-L.jpg" },
            new() { ISBN = "9780307743657", Title = "The Shining", AuthorId = king.Id, GenreId = horror.Id, Description = "A family heads to an isolated hotel where a sinister presence influences the father into violence.", TotalPages = 447, PublishedYear = 1977, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780307743657-L.jpg" },
            new() { ISBN = "9780062073488", Title = "Murder on the Orient Express", AuthorId = christie.Id, GenreId = thriller.Id, Description = "Detective Hercule Poirot investigates a murder that occurred on the famous Orient Express train.", TotalPages = 274, PublishedYear = 1934, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062073488-L.jpg" },
            new() { ISBN = "9780062073501", Title = "And Then There Were None", AuthorId = christie.Id, GenreId = thriller.Id, Description = "Ten strangers are lured to an isolated island where they are murdered one by one.", TotalPages = 272, PublishedYear = 1939, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062073501-L.jpg" },
            new() { ISBN = "9780679720201", Title = "The Stranger", AuthorId = camus.Id, GenreId = literaryFiction.Id, Description = "The story of an ordinary man who unwittingly gets drawn into a senseless murder on an Algerian beach.", TotalPages = 123, PublishedYear = 1942, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780679720201-L.jpg" },
            new() { ISBN = "9780452284234", Title = "Animal Farm", AuthorId = orwell.Id, GenreId = literaryFiction.Id, Description = "An allegorical novella reflecting events leading up to the Russian Revolution.", TotalPages = 112, PublishedYear = 1945, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780452284234-L.jpg" },

            // ── Science / Physics / Biology ──
            new() { ISBN = "9780553380163", Title = "A Brief History of Time", AuthorId = hawking.Id, GenreId = physics.Id, Description = "A landmark volume in science writing exploring time, space, black holes, and the Big Bang.", TotalPages = 256, PublishedYear = 1988, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780553380163-L.jpg" },
            new() { ISBN = "9780345539434", Title = "Cosmos", AuthorId = sagan.Id, GenreId = physics.Id, Description = "An exploration of the universe, its origins, and humanity's place within it.", TotalPages = 396, PublishedYear = 1980, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780345539434-L.jpg" },
            new() { ISBN = "9780393355628", Title = "Astrophysics for People in a Hurry", AuthorId = sagan.Id, GenreId = physics.Id, Description = "Essential knowledge about the universe delivered in short, digestible chapters.", TotalPages = 222, PublishedYear = 2017, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780393355628-L.jpg" },
            new() { ISBN = "9780141033570", Title = "The Selfish Gene", AuthorId = dawkins.Id, GenreId = biology.Id, Description = "Dawkins's brilliant reformulation of the theory of natural selection from the gene's point of view.", TotalPages = 360, PublishedYear = 1976, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780141033570-L.jpg" },
            new() { ISBN = "9780143126560", Title = "The Better Angels of Our Nature", AuthorId = pinker.Id, GenreId = psychology.Id, Description = "A detailed study of the decline of violence in human history.", TotalPages = 832, PublishedYear = 2011, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780143126560-L.jpg" },

            // ── Programming / Tech ──
            new() { ISBN = "9780201633610", Title = "Design Patterns", AuthorId = gamma.Id, GenreId = programming.Id, Description = "The Gang of Four classic — essential patterns for reusable object-oriented software.", TotalPages = 395, PublishedYear = 1994, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201633610-L.jpg" },
            new() { ISBN = "9780201485677", Title = "Refactoring", AuthorId = fowler.Id, GenreId = agile.Id, Description = "Improving the design of existing code through systematic, behaviour-preserving transformations.", TotalPages = 431, PublishedYear = 1999, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780201485677-L.jpg" },
            new() { ISBN = "9780735619678", Title = "Code Complete", AuthorId = mcconnell.Id, GenreId = programming.Id, Description = "A practical handbook of software construction — widely regarded as one of the best guides.", TotalPages = 960, PublishedYear = 2004, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780735619678-L.jpg" },
            new() { ISBN = "9780321125217", Title = "Domain-Driven Design", AuthorId = fowler.Id, GenreId = programming.Id, Description = "Tackling complexity in the heart of software through domain modeling.", TotalPages = 560, PublishedYear = 2003, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780321125217-L.jpg" },
            new() { ISBN = "9780321127426", Title = "Patterns of Enterprise Application Architecture", AuthorId = fowler.Id, GenreId = programming.Id, Description = "Patterns and strategies for building enterprise applications.", TotalPages = 533, PublishedYear = 2002, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780321127426-L.jpg" },
            new() { ISBN = "9780596007126", Title = "Head First Design Patterns", AuthorId = gamma.Id, GenreId = programming.Id, Description = "A brain-friendly guide to design patterns using a visually rich format.", TotalPages = 694, PublishedYear = 2004, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780596007126-L.jpg" },
            new() { ISBN = "9780321278654", Title = "Extreme Programming Explained", AuthorId = bobMartin.Id, GenreId = agile.Id, Description = "Embrace change — the foundational text for Extreme Programming methodology.", TotalPages = 224, PublishedYear = 2004, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780321278654-L.jpg" },

            // ── UX / Design ──
            new() { ISBN = "9780321965516", Title = "Don't Make Me Think", AuthorId = krug.Id, GenreId = uxDesign.Id, Description = "A common-sense approach to web usability — the classic guide to intuitive navigation.", TotalPages = 216, PublishedYear = 2014, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780321965516-L.jpg" },

            // ── Self-Help / Psychology ──
            new() { ISBN = "9780062457714", Title = "The Subtle Art of Not Giving a F*ck", AuthorId = manson.Id, GenreId = selfHelp.Id, Description = "A counterintuitive approach to living a good life by caring less about more.", TotalPages = 224, PublishedYear = 2016, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062457714-L.jpg" },
            new() { ISBN = "9781501111105", Title = "Grit", AuthorId = duckworth.Id, GenreId = psychology.Id, Description = "Why passion and persistence matter more than talent in achieving success.", TotalPages = 352, PublishedYear = 2016, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781501111105-L.jpg" },
            new() { ISBN = "9780345472328", Title = "Mindset", AuthorId = dweck.Id, GenreId = psychology.Id, Description = "How a simple idea about the brain can change the way you learn and live.", TotalPages = 320, PublishedYear = 2006, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780345472328-L.jpg" },
            new() { ISBN = "9781455586691", Title = "Deep Work", AuthorId = newport.Id, GenreId = selfHelp.Id, Description = "Rules for focused success in a distracted world.", TotalPages = 296, PublishedYear = 2016, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781455586691-L.jpg" },
            new() { ISBN = "9780812981605", Title = "Antifragile", AuthorId = taleb.Id, GenreId = philosophy.Id, Description = "Things that gain from disorder — a framework for thriving in an uncertain world.", TotalPages = 519, PublishedYear = 2012, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780812981605-L.jpg" },
            new() { ISBN = "9780812979688", Title = "The Black Swan", AuthorId = taleb.Id, GenreId = philosophy.Id, Description = "The impact of the highly improbable — rare events shape our world.", TotalPages = 444, PublishedYear = 2007, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780812979688-L.jpg" },
            new() { ISBN = "9780316017930", Title = "Outliers", AuthorId = gladwell.Id, GenreId = psychology.Id, Description = "The story of success — why some people achieve more than others.", TotalPages = 309, PublishedYear = 2008, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780316017930-L.jpg" },
            new() { ISBN = "9780316346627", Title = "The Tipping Point", AuthorId = gladwell.Id, GenreId = psychology.Id, Description = "How little things can make a big difference — the science of epidemics.", TotalPages = 301, PublishedYear = 2000, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780316346627-L.jpg" },

            // ── Business ──
            new() { ISBN = "9780804139298", Title = "Zero to One", AuthorId = thiel.Id, GenreId = business.Id, Description = "Notes on startups, or how to build the future — every moment in business happens only once.", TotalPages = 224, PublishedYear = 2014, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780804139298-L.jpg" },

            // ── Memoir / Biography ──
            new() { ISBN = "9781524763138", Title = "Becoming", AuthorId = obama.Id, GenreId = memoir.Id, Description = "An intimate, powerful, and inspiring memoir by the former First Lady of the United States.", TotalPages = 448, PublishedYear = 2018, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781524763138-L.jpg" },
            new() { ISBN = "9781451648539", Title = "Steve Jobs", AuthorId = isaacson.Id, GenreId = memoir.Id, Description = "The exclusive biography of Steve Jobs based on more than forty interviews over two years.", TotalPages = 656, PublishedYear = 2011, CoverUrl = "https://covers.openlibrary.org/b/isbn/9781451648539-L.jpg" },
            new() { ISBN = "9780399592522", Title = "Leonardo da Vinci", AuthorId = isaacson.Id, GenreId = memoir.Id, Description = "Drawing on thousands of pages from Leonardo's notebooks — the biography of the ultimate genius.", TotalPages = 624, PublishedYear = 2017, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780399592522-L.jpg" },
            new() { ISBN = "9780812995343", Title = "Dare to Lead", AuthorId = brown.Id, GenreId = business.Id, Description = "Brave work. Tough conversations. Whole hearts. Leadership for a new era.", TotalPages = 320, PublishedYear = 2018, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780812995343-L.jpg" },

            // ── History ──
            new() { ISBN = "9780062316110", Title = "Homo Deus", AuthorId = harari.Id, GenreId = history.Id, Description = "A brief history of tomorrow — Harari explores what might happen to the world when old myths combine with new technology.", TotalPages = 450, PublishedYear = 2017, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062316110-L.jpg" },
            new() { ISBN = "9780062464347", Title = "21 Lessons for the 21st Century", AuthorId = harari.Id, GenreId = history.Id, Description = "Harari addresses the biggest questions facing us today — from technology to terrorism.", TotalPages = 372, PublishedYear = 2018, CoverUrl = "https://covers.openlibrary.org/b/isbn/9780062464347-L.jpg" },
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
        // Format: (UserId, BookIndex, DaysAgo borrowed, Duration in days)
        var historicalLoans = new List<(int UserId, int BookIndex, int DaysAgo, int DurationDays)>
        {
            // ── Alice (user 1) — heavy reader, 20+ completed loans ──
            (1, 0, 360, 6),  (1, 1, 340, 9),  (1, 2, 320, 12), (1, 3, 300, 8),  (1, 4, 280, 5),
            (1, 7, 260, 10), (1, 8, 240, 4),  (1, 9, 220, 13), (1, 10, 200, 7), (1, 13, 180, 14),
            (1, 15, 160, 11),(1, 20, 140, 10),(1, 22, 120, 8), (1, 25, 100, 6), (1, 30, 85, 11),
            (1, 33, 70, 5),  (1, 37, 55, 9),  (1, 40, 45, 6),  (1, 45, 38, 8),  (1, 50, 30, 13),
            (1, 53, 380, 7), (1, 55, 370, 14),(1, 57, 350, 10),(1, 58, 330, 12),

            // ── Bob (user 2) — sci-fi and programming fan, 18 loans ──
            (2, 1, 350, 10), (2, 4, 320, 7),  (2, 5, 290, 20), (2, 8, 260, 4),  (2, 9, 230, 8),
            (2, 10, 200, 6), (2, 11, 175, 12),(2, 13, 150, 9), (2, 14, 125, 15),(2, 16, 100, 7),
            (2, 17, 80, 10), (2, 19, 60, 8),  (2, 22, 45, 15), (2, 38, 340, 11),(2, 39, 310, 8),
            (2, 40, 280, 13),(2, 44, 250, 6), (2, 48, 220, 9),

            // ── Clara (user 3) — history and memoir lover, 16 loans ──
            (3, 3, 370, 14), (3, 6, 340, 11), (3, 7, 310, 6),  (3, 2, 280, 13), (3, 9, 250, 9),
            (3, 15, 220, 10),(3, 25, 190, 7), (3, 31, 160, 12),(3, 35, 130, 8), (3, 42, 100, 11),
            (3, 46, 70, 9),  (3, 50, 50, 7),  (3, 55, 350, 10),(3, 56, 300, 8), (3, 57, 270, 14),
            (3, 58, 240, 6),

            // ── David (user 4) — philosophy and psychology, 16 loans ──
            (4, 0, 380, 7),  (4, 2, 350, 10), (4, 9, 320, 8),  (4, 3, 290, 16), (4, 7, 260, 5),
            (4, 18, 230, 11),(4, 28, 200, 9), (4, 36, 170, 14),(4, 42, 140, 7), (4, 47, 110, 10),
            (4, 49, 80, 8),  (4, 51, 55, 12), (4, 52, 360, 9), (4, 53, 330, 11),(4, 10, 300, 7),
            (4, 30, 270, 13),

            // ── Eva (user 5) — fantasy and self-help, 16 loans ──
            (5, 5, 390, 18), (5, 6, 360, 12), (5, 7, 330, 6),  (5, 8, 300, 4),  (5, 4, 270, 8),
            (5, 12, 240, 14),(5, 20, 210, 10),(5, 21, 180, 13),(5, 23, 150, 7), (5, 24, 120, 11),
            (5, 33, 90, 7),  (5, 43, 65, 9),  (5, 45, 45, 6),  (5, 46, 370, 10),(5, 48, 340, 8),
            (5, 50, 310, 12),

            // ── Fredrik (user 6) — programming and tech focus, 16 loans ──
            (6, 1, 400, 11), (6, 0, 370, 8),  (6, 4, 340, 7),  (6, 7, 310, 5),  (6, 9, 280, 9),
            (6, 16, 250, 13),(6, 26, 220, 6), (6, 36, 190, 10),(6, 37, 160, 8), (6, 38, 130, 12),
            (6, 39, 100, 7), (6, 40, 75, 9),  (6, 41, 50, 6),  (6, 44, 380, 14),(6, 2, 350, 11),
            (6, 3, 320, 8),

            // ── Greta (user 7) — eclectic reader, 16 loans ──
            (7, 3, 410, 15), (7, 2, 380, 11), (7, 5, 350, 22), (7, 8, 310, 5),  (7, 6, 280, 10),
            (7, 19, 250, 8), (7, 29, 220, 12),(7, 41, 190, 7), (7, 11, 160, 9), (7, 22, 130, 14),
            (7, 32, 100, 6), (7, 43, 70, 10), (7, 47, 50, 8),  (7, 53, 390, 7), (7, 55, 360, 12),
            (7, 57, 330, 9),

            // ── Hans (user 8) — steady reader, 18 loans ──
            (8, 0, 420, 6),  (8, 1, 390, 9),  (8, 2, 360, 12), (8, 3, 330, 14), (8, 4, 300, 8),
            (8, 5, 270, 19), (8, 6, 240, 11), (8, 7, 210, 5),  (8, 17, 180, 10),(8, 27, 150, 13),
            (8, 37, 120, 8), (8, 10, 100, 7), (8, 14, 80, 9),  (8, 21, 60, 11), (8, 31, 45, 6),
            (8, 34, 380, 10),(8, 44, 350, 8), (8, 54, 320, 12),

            // ── Second round — users re-borrowing popular books ──
            // Alice re-borrows favourites
            (1, 1, 150, 8),  (1, 7, 130, 5),  (1, 9, 110, 10), (1, 20, 95, 7),
            // Bob circles back to sci-fi
            (2, 5, 150, 12), (2, 10, 130, 6), (2, 13, 110, 14),(2, 17, 90, 9),
            // Clara revisits history
            (3, 3, 140, 10), (3, 55, 120, 8), (3, 57, 100, 11),
            // Eva re-reads fantasy
            (5, 20, 130, 9), (5, 23, 100, 7), (5, 24, 75, 12),
            // Hans re-reads classics
            (8, 9, 140, 8),  (8, 2, 110, 10), (8, 0, 80, 6),
        };

        var copyPool = copies.GroupBy(c => c.BookId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var usedCopySlots = new Dictionary<int, DateTime>();

        foreach (var (userId, bookIndex, daysAgo, duration) in historicalLoans)
        {
            if (bookIndex >= books.Count) continue;
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
                DueDate = borrowed.AddDays(14),
                ReturnedAt = returned,
            });

            usedCopySlots[available.Id] = returned;
        }

        await db.SaveChangesAsync();

        // ─── Active loans for Alice ───────────────────────────────────────────
        // Current (not overdue)
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 1, daysAgo: 3);   // Clean Code — 11 days left
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 5, daysAgo: 5);   // Dune — 9 days left
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 23, daysAgo: 2);  // Harry Potter — 12 days left

        // OVERDUE loans for Alice (borrowed > 14 days ago, not yet returned)
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 32, daysAgo: 21); // A Brief History of Time — 7 days overdue
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 46, daysAgo: 30); // The Subtle Art... — 16 days overdue
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 1, bookIndex: 10, daysAgo: 18); // 1984 — 4 days overdue

        // Active loans for other users (to make the library feel lived-in)
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 2, bookIndex: 3, daysAgo: 7);   // Bob — Sapiens
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 2, bookIndex: 20, daysAgo: 4);  // Bob — Lord of the Rings
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 3, bookIndex: 7, daysAgo: 10);  // Clara — Atomic Habits
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 4, bookIndex: 15, daysAgo: 6);  // David — Neuromancer
        AddActiveLoan(db, books, copyPool, usedCopySlots, userId: 5, bookIndex: 24, daysAgo: 8);  // Eva — Mistborn

        await db.SaveChangesAsync();
    }

    /// <summary>Helper: add an active (not returned) loan for a user.</summary>
    private static void AddActiveLoan(
        AppDbContext db, List<Book> books, Dictionary<int, List<BookCopy>> copyPool,
        Dictionary<int, DateTime> usedCopySlots, int userId, int bookIndex, int daysAgo)
    {
        if (bookIndex >= books.Count) return;
        var book = books[bookIndex];
        if (!copyPool.ContainsKey(book.Id)) return;

        var copy = copyPool[book.Id].FirstOrDefault(c =>
            !usedCopySlots.ContainsKey(c.Id) || usedCopySlots[c.Id] < DateTime.UtcNow.AddDays(-daysAgo));

        if (copy is null) return;

        var borrowed = DateTime.UtcNow.AddDays(-daysAgo);
        db.Loans.Add(new Loan
        {
            BookCopyId = copy.Id,
            UserId = userId,
            BorrowedAt = borrowed,
            DueDate = borrowed.AddDays(14),
        });

        usedCopySlots[copy.Id] = DateTime.MaxValue; // Mark as still active
    }
}
