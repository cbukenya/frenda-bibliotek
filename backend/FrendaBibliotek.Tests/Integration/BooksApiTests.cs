using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Tests.Integration;

[Collection("Api")]
public class BooksApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    public BooksApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        // Authenticate as Alice (user 1) for all requests
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ApiFactory.CreateTestToken(1, "alice@bibliotek.se", "Alice Lindgren"));
    }

    [Fact]
    public async Task GetBooks_ReturnsAllBooks()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var books = await response.Content.ReadFromJsonAsync<List<BookSummaryDto>>();
        books.Should().NotBeNullOrEmpty();
        books!.Count.Should().Be(10); // seeded 10 books
    }

    [Fact]
    public async Task GetBooks_WithoutAuth_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = null;
        var response = await client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBooks_IncludesAvailabilityData()
    {
        var books = await _client.GetFromJsonAsync<List<BookSummaryDto>>("/api/books");

        books.Should().AllSatisfy(b =>
        {
            b.TotalCopies.Should().BeGreaterThan(0);
            b.AvailableCopies.Should().BeGreaterThanOrEqualTo(0);
            b.AvailableCopies.Should().BeLessThanOrEqualTo(b.TotalCopies);
        });
    }

    [Fact]
    public async Task GetBookById_ReturnsDetail()
    {
        var response = await _client.GetAsync("/api/books/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var book = await response.Content.ReadFromJsonAsync<BookDetailDto>();
        book.Should().NotBeNull();
        book!.Id.Should().Be(1);
        book.Title.Should().NotBeNullOrWhiteSpace();
        book.Recommendations.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBookById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/books/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopBooks_ReturnsTenBooks()
    {
        var books = await _client.GetFromJsonAsync<List<BookSummaryDto>>("/api/books/top");

        books.Should().NotBeNullOrEmpty();
        books!.Count.Should().BeLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetBookById_HasReadingTimeAfterSeed()
    {
        // Books with historical loans should have avg reading days populated
        var book = await _client.GetFromJsonAsync<BookDetailDto>("/api/books/1");

        book.Should().NotBeNull();
        // Seeded historical loans exist so avgReadingDays should be set
        book!.AvgReadingDays.Should().NotBeNull();
        book.AvgReadingDays.Should().BeGreaterThan(0);
    }
}
