using Shreyas.Profile.Application.Quotes;

namespace Shreyas.Profile.Api.Endpoints;

public static class QuoteEndpoints
{
    public static IEndpointRouteBuilder MapQuoteEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/quotes/today", async (
            GetQuoteOfTheDay getQuoteOfTheDay,
            CancellationToken cancellationToken) =>
        {
            var result = await getQuoteOfTheDay.ExecuteAsync(DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

            return TypedResults.Ok(new QuoteResponse(result.Text, result.Author, result.Date, result.Source));
        })
        .WithName("GetQuoteOfTheDay")
        .WithSummary("Gets the curated quote for the current UTC date")
        .WithDescription("Returns a deterministic curated quote selected from the current UTC date.")
        .Produces<QuoteResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .AllowAnonymous();

        return endpoints;
    }
}

public sealed record QuoteResponse(string Text, string Author, DateOnly Date, string Source);
