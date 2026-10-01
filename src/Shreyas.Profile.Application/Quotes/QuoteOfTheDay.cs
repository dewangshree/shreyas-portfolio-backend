namespace Shreyas.Profile.Application.Quotes;

public sealed record QuoteOfTheDay(string Text, string Author, DateOnly Date, string Source);
