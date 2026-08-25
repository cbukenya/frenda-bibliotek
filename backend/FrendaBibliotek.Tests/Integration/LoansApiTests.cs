using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using FrendaBibliotek.Api.DTOs;

namespace FrendaBibliotek.Tests.Integration;

[Collection("Api")]
public class LoansApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private readonly ApiFactory _factory;

    // Alice = user 1, seeded with 2 active loans
    private const int AliceId = 1;
    // Clean user for borrow/return tests to avoid state bleed
    private const int BobId = 2;

    public LoansApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private void SetUser(int userId, string email = "test@test.se", string name = "Test User")
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ApiFactory.CreateTestToken(userId, email, name));
    }

    // ─── GET /api/loans ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetMyLoans_WithoutAuth_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = null;
        var response = await client.GetAsync("/api/loans");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyLoans_ForAlice_ReturnsActiveLoans()
    {
        SetUser(AliceId, "alice@bibliotek.se", "Alice Lindgren");

        var loans = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");

        loans.Should().NotBeNullOrEmpty();
        // Alice has 2 seeded active loans
        loans!.Where(l => l.ReturnedAt == null).Should().HaveCountGreaterThanOrEqualTo(2);
    }

    // ─── POST /api/loans ──────────────────────────────────────────────────────

    [Fact]
    public async Task BorrowBook_ValidRequest_Returns201()
    {
        SetUser(BobId, "bob@bibliotek.se", "Bob Eriksson");

        // Book 3 (Thinking, Fast and Slow) — ISBN 9780374533557
        var response = await _client.PostAsJsonAsync("/api/loans", new BorrowRequest("9780374533557"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        loan.Should().NotBeNull();
        loan!.ISBN.Should().Be("9780374533557");
        loan.ReturnedAt.Should().BeNull();
    }

    [Fact]
    public async Task BorrowBook_WithoutAuth_Returns401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = null;
        var response = await client.PostAsJsonAsync("/api/loans", new BorrowRequest("9780465050659"));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ─── PATCH /api/loans/{id}/return ─────────────────────────────────────────

    [Fact]
    public async Task ReturnLoan_OtherUserLoan_Returns403()
    {
        // Get Alice's active loans
        SetUser(AliceId, "alice@bibliotek.se", "Alice Lindgren");
        var aliceLoans = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");
        var aliceActiveLoan = aliceLoans!.First(l => l.ReturnedAt == null);

        // Bob tries to return Alice's loan
        SetUser(BobId, "bob@bibliotek.se", "Bob Eriksson");
        var response = await _client.PatchAsync($"/api/loans/{aliceActiveLoan.Id}/return", null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReturnLoan_NotFound_Returns404()
    {
        SetUser(AliceId, "alice@bibliotek.se", "Alice Lindgren");

        var response = await _client.PatchAsync("/api/loans/99999/return", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
