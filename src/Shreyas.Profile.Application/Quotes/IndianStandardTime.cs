namespace Shreyas.Profile.Application.Quotes;

public static class IndianStandardTime
{
    private static readonly TimeZoneInfo TimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

    public static DateOnly Today() => FromUtc(DateTimeOffset.UtcNow);

    public static DateOnly FromUtc(DateTimeOffset utcTime) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(utcTime, TimeZone).DateTime);
}
