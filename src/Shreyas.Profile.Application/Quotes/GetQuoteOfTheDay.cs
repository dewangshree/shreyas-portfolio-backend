using Shreyas.Profile.Domain.Quotes;

namespace Shreyas.Profile.Application.Quotes;

public sealed class GetQuoteOfTheDay(IQuoteProvider quoteProvider)
{
    public async Task<QuoteOfTheDay> ExecuteAsync(
        DateOnly utcDate,
        CancellationToken cancellationToken = default)
    {
        var quotes = await quoteProvider.GetQuotesAsync(cancellationToken);

        if (quotes.Count == 0)
        {
            throw new InvalidOperationException("No curated quotes are available.");
        }

        var quote = quotes[GetIndex(utcDate, quotes.Count)];
        return new QuoteOfTheDay(quote.Text, quote.Author, utcDate, "curated");
    }

    public static int GetIndex(DateOnly date, int quoteCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quoteCount);
        return (int)(date.DayNumber % quoteCount);
    }
}
