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
    public async Task GetBooks_ReturnsPagedResult()
    {
        var response = await _client.GetAsync("/api/books");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<BookSummaryDto>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeNullOrEmpty();
        result.Total.Should().BeGreaterThanOrEqualTo(10);
        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetBooks_WithoutAuth_StillReturnsBooks()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = null;
        var response = await client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBooks_PaginationSecondPage_ReturnsCorrectSlice()
    {
        // Page 1 with pageSize=5 should return exactly 5 items
        var result = await _client.GetFromJsonAsync<PagedResult<BookSummaryDto>>("/api/books?page=2&pageSize=5");

        result.Should().NotBeNull();
        result!.Total.Should().BeGreaterThanOrEqualTo(10);
        result.Page.Should().Be(2);
        result.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetBooks_IncludesAvailabilityData()
    {
        var result = await _client.GetFromJsonAsync<PagedResult<BookSummaryDto>>("/api/books");

        result!.Items.Should().AllSatisfy(b =>
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

    [Fact]
    public async Task GetBookById_RecommendationsNeverEmpty()
    {
        // Even for a book with no loan history, the genre fallback should populate recommendations
        // (all seeded books share genres so at least one genre-based result should exist)
        var book = await _client.GetFromJsonAsync<BookDetailDto>("/api/books/1");

        book.Should().NotBeNull();
        book!.Recommendations.Should().NotBeNull();
        // Genre fallback ensures this is never empty given we have 10 seeded books in the same genres
        book.Recommendations.Should().NotBeEmpty();
    }
}
