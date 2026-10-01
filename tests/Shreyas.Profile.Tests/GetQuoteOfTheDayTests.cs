using FluentAssertions;
using Shreyas.Profile.Application.Quotes;
using Shreyas.Profile.Domain.Quotes;

namespace Shreyas.Profile.Tests;

public sealed class GetQuoteOfTheDayTests
{
    private static readonly IReadOnlyList<Quote> Quotes =
    [
        new("Build quietly and consistently.", "Shreyas"),
        new("Make progress visible.", "Team"),
        new("Keep learning.", "Engineer")
    ];

    private readonly GetQuoteOfTheDay _sut = new(new TestQuoteProvider(Quotes));

    [Fact]
    public async Task Same_date_returns_same_quote()
    {
        var date = new DateOnly(2026, 9, 30);

        var first = await _sut.ExecuteAsync(date);
        var second = await _sut.ExecuteAsync(date);

        first.Should().Be(second);
    }

    [Fact]
    public async Task Selection_is_deterministic()
    {
        var date = new DateOnly(2026, 9, 30);

        var result = await _sut.ExecuteAsync(date);

        result.Text.Should().Be(Quotes[GetQuoteOfTheDay.GetIndex(date, Quotes.Count)].Text);
    }

    [Fact]
    public void Quote_collection_is_not_empty() => Quotes.Should().NotBeEmpty();

    [Fact]
    public async Task Quote_has_text()
    {
        var result = await _sut.ExecuteAsync(new DateOnly(2026, 9, 30));
        result.Text.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Quote_has_author()
    {
        var result = await _sut.ExecuteAsync(new DateOnly(2026, 9, 30));
        result.Author.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Source_equals_curated()
    {
        var result = await _sut.ExecuteAsync(new DateOnly(2026, 9, 30));
        result.Source.Should().Be("curated");
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2026, 9, 30)]
    [InlineData(9999, 12, 31)]
    public void Quote_index_is_always_within_collection_bounds(int year, int month, int day)
    {
        var index = GetQuoteOfTheDay.GetIndex(new DateOnly(year, month, day), Quotes.Count);
        index.Should().BeInRange(0, Quotes.Count - 1);
    }

    [Fact]
    public async Task Multiple_dates_can_resolve_to_different_quotes()
    {
        var first = await _sut.ExecuteAsync(new DateOnly(2026, 9, 30));
        var second = await _sut.ExecuteAsync(new DateOnly(2026, 10, 1));

        first.Text.Should().NotBe(second.Text);
    }

    private sealed class TestQuoteProvider(IReadOnlyList<Quote> quotes) : IQuoteProvider
    {
        public Task<IReadOnlyList<Quote>> GetQuotesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(quotes);
    }
}
