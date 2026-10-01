using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shreyas.Profile.Application.Quotes;
using Shreyas.Profile.Domain.Quotes;

namespace Shreyas.Profile.Infrastructure.Quotes;

public sealed class CuratedQuoteProvider(
    IHostEnvironment environment,
    ILogger<CuratedQuoteProvider> logger) : IQuoteProvider
{
    private static readonly IReadOnlyList<Quote> FallbackQuotes =
    [new("Success is the sum of small efforts repeated day in and day out.", "Robert Collier")];

    private readonly Lazy<Task<IReadOnlyList<Quote>>> _quotes = new(() => LoadQuotesAsync(environment, logger));

    public Task<IReadOnlyList<Quote>> GetQuotesAsync(CancellationToken cancellationToken = default) =>
        _quotes.Value.WaitAsync(cancellationToken);

    private static async Task<IReadOnlyList<Quote>> LoadQuotesAsync(
        IHostEnvironment environment,
        ILogger logger)
    {
        var filePath = Path.Combine(environment.ContentRootPath, "Data", "quotes.json");

        try
        {
            await using var stream = File.OpenRead(filePath);
            var data = await JsonSerializer.DeserializeAsync<List<Quote>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var validQuotes = data?
                .Where(quote => !string.IsNullOrWhiteSpace(quote.Text) && !string.IsNullOrWhiteSpace(quote.Author))
                .ToArray() ?? [];

            if (validQuotes.Length > 0)
            {
                return validQuotes;
            }

            logger.LogWarning("Curated quote data at {QuoteFilePath} is empty or invalid; using fallback quote.", filePath);
        }
        catch (Exception exception) when (exception is IOException or JsonException)
        {
            logger.LogWarning(exception, "Unable to load curated quotes from {QuoteFilePath}; using fallback quote.", filePath);
        }

        return FallbackQuotes;
    }
}
