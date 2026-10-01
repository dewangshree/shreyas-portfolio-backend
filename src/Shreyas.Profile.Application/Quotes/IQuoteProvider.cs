using Shreyas.Profile.Domain.Quotes;

namespace Shreyas.Profile.Application.Quotes;

public interface IQuoteProvider
{
    Task<IReadOnlyList<Quote>> GetQuotesAsync(CancellationToken cancellationToken = default);
}
