using FrendaBibliotek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrendaBibliotek.Api.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Book → BookCopies (one-to-many)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Copies)
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // BookCopy → Loans (one-to-many)
        modelBuilder.Entity<BookCopy>()
            .HasMany(c => c.Loans)
            .WithOne(l => l.BookCopy)
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → Loans (one-to-many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Loans)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserType stored as string for readability in the DB
        modelBuilder.Entity<User>()
            .Property(u => u.UserType)
            .HasConversion<string>();

        // Genre self-referencing tree
        modelBuilder.Entity<Genre>(g =>
        {
            g.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            g.Property(x => x.Name).IsRequired().HasMaxLength(100);
            g.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            g.HasIndex(x => x.Slug).IsUnique();
        });

        // Book → Genre
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Author
        modelBuilder.Entity<Author>(a =>
        {
            a.Property(x => x.Name).IsRequired().HasMaxLength(200);
            a.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            a.HasIndex(x => x.Slug).IsUnique();
        });

        // Book → Author
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Required string fields
        modelBuilder.Entity<Book>(b =>
        {
            b.Property(x => x.ISBN).IsRequired().HasMaxLength(20);
            b.HasIndex(x => x.ISBN).IsUnique();
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);
            b.Property(x => x.Description).IsRequired();
        });

        modelBuilder.Entity<User>(u =>
        {
            u.Property(x => x.Name).IsRequired().HasMaxLength(200);
            u.Property(x => x.Email).IsRequired().HasMaxLength(200);
            u.HasIndex(x => x.Email).IsUnique();
        });
    }
}
