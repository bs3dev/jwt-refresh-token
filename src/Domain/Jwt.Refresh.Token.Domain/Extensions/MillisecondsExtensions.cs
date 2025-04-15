namespace Jwt.Refresh.Token.Domain.Extensions;

public static class MillisecondsExtensions
{
    public static TimeSpan ToTimeSpanMilliseconds(this int value) => TimeSpan.FromMilliseconds(value);
    public static TimeSpan SecondsToTimeSpanMilliseconds(this int value) => TimeSpan.FromMilliseconds(value * 1000);
    public static int SecondsToMilliseconds(this int value) => value * 1000;
    public static TimeSpan MinutesToTimeSpanMilliseconds(this int value) => TimeSpan.FromMilliseconds(value * 60 * 1000);
    public static int MinutesToMilliseconds(this int value) => value * 60 * 1000;
    public static TimeSpan HoursToTimeSpanMilliseconds(this int value) => TimeSpan.FromMilliseconds(value * 60 * 60 * 1000);
    public static int HoursToMilliseconds(this int value) => value * 60 * 60 * 1000;
    
    public static DateTimeOffset ToDateTimeOffset(this int value) => DateTimeOffset.UtcNow.AddMilliseconds(value);
}