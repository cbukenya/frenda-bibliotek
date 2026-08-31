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

    // ─── Full borrow → return journey ─────────────────────────────────────────

    [Fact]
    public async Task BorrowThenReturn_FullFlow_UpdatesLoanCorrectly()
    {
        // Use a dedicated user (Clara = 3) so we don't collide with other tests
        const int ClaraId = 3;
        SetUser(ClaraId, "clara@bibliotek.se", "Clara Svensson");

        // Step 1: Borrow a book (Atomic Habits — ISBN 9780735211292)
        var borrowResponse = await _client.PostAsJsonAsync("/api/loans", new BorrowRequest("9780735211292"));
        borrowResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loan = await borrowResponse.Content.ReadFromJsonAsync<LoanDto>();
        loan.Should().NotBeNull();
        loan!.ISBN.Should().Be("9780735211292");
        loan.ReturnedAt.Should().BeNull("the book was just borrowed");
        loan.DueDate.Should().BeAfter(loan.BorrowedAt, "due date must be in the future relative to borrow");

        // Step 2: Verify it appears in Clara's loan list
        var myLoans = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");
        myLoans.Should().Contain(l => l.Id == loan.Id && l.ReturnedAt == null);

        // Step 3: Return the loan
        var returnResponse = await _client.PatchAsync($"/api/loans/{loan.Id}/return", null);
        returnResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var returned = await returnResponse.Content.ReadFromJsonAsync<LoanDto>();
        returned.Should().NotBeNull();
        returned!.ReturnedAt.Should().NotBeNull("the loan should now be marked as returned");
        returned.ReturnedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(10));

        // Step 4: Verify the loan history still shows it (ReturnedAt is set, not deleted)
        var loansAfterReturn = await _client.GetFromJsonAsync<List<LoanDto>>("/api/loans");
        var historicalLoan = loansAfterReturn!.FirstOrDefault(l => l.Id == loan.Id);
        historicalLoan.Should().NotBeNull("loan history must be preserved — rows are never deleted");
        historicalLoan!.ReturnedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task BorrowBook_LoanHasDueDate_14DaysFromBorrow()
    {
        SetUser(BobId, "bob@bibliotek.se", "Bob Eriksson");

        // ISBN: Sapiens
        var response = await _client.PostAsJsonAsync("/api/loans", new BorrowRequest("9780062316097"));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
        loan.Should().NotBeNull();

        var expectedDue = loan!.BorrowedAt.AddDays(14);
        loan.DueDate.Should().BeCloseTo(expectedDue, precision: TimeSpan.FromSeconds(5),
            because: "DueDate should be exactly 14 days after BorrowedAt");
    }
}
