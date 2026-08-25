using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Tests.Integration;

[Collection("Api")]
public class LoansApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    // Alice = user 1, seeded with 2 active loans
    private const int AliceId = 1;
    // Clean user for borrow/return tests to avoid state bleed
    private const int BobId = 2;

    public LoansApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private void SetUser(int userId) =>
        _client.DefaultRequestHeaders.Remove("X-User-Id").ToString();

    // ─── GET /api/loans ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetMyLoans_WithoutHeader_Returns400()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        var response = await _client.GetAsync("/api/loans");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMyLoans_ForAlice_ReturnsActiveLoans()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", AliceId.ToString());

        var loans = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");

        loans.Should().NotBeNullOrEmpty();
        // Alice has 2 seeded active loans
        loans!.Where(l => l.ReturnedAt == null).Should().HaveCountGreaterThanOrEqualTo(2);
    }

    // ─── POST /api/loans ──────────────────────────────────────────────────────

    [Fact]
    public async Task BorrowBook_ValidRequest_Returns201()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", BobId.ToString());

        // Book 3 (Thinking, Fast and Slow) — should have an available copy
        var response = await _client.PostAsJsonAsync("/api/loans", new BorrowRequest(3));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        loan.Should().NotBeNull();
        loan!.BookId.Should().Be(3);
        loan.ReturnedAt.Should().BeNull();
    }

    [Fact]
    public async Task BorrowBook_WithoutHeader_Returns400()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        var response = await _client.PostAsJsonAsync("/api/loans", new BorrowRequest(1));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ─── PATCH /api/loans/{id}/return ─────────────────────────────────────────

    [Fact]
    public async Task ReturnLoan_OtherUserLoan_Returns403()
    {
        // Get Alice's active loans
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", AliceId.ToString());
        var aliceLoans = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");
        var aliceActiveLoan = aliceLoans!.First(l => l.ReturnedAt == null);

        // Bob tries to return Alice's loan
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", BobId.ToString());
        var response = await _client.PatchAsync($"/api/loans/{aliceActiveLoan.Id}/return", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReturnLoan_NotFound_Returns404()
    {
        _client.DefaultRequestHeaders.Remove("X-User-Id");
        _client.DefaultRequestHeaders.Add("X-User-Id", AliceId.ToString());

        var response = await _client.PatchAsync("/api/loans/99999/return", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
