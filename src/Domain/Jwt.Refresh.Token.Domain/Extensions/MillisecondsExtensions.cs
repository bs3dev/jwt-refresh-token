namespace Jwt.Refresh.Token.Domain.Extensions;

/// <summary>
/// Provides extension methods for converting time values between milliseconds, seconds, minutes, and hours
/// into <see cref="TimeSpan"/>, as well as methods for converting units of time into milliseconds or seconds,
/// and creating <see cref="DateTimeOffset"/> instances from a millisecond offset.
/// </summary>
public static class MillisecondsExtensions
{
    /// <summary>
    /// Converts the specified number of milliseconds to seconds.
    /// </summary>
    /// <param name="value">The value in milliseconds.</param>
    /// <returns>The equivalent number of seconds (integer part).</returns>
    public static int MillisecondsToSeconds(this int value) 
        => value / 1000;
    
    /// <summary>
    /// Converts the specified number of seconds to milliseconds.
    /// </summary>
    /// <param name="value">The value in seconds.</param>
    /// <returns>The equivalent number of milliseconds.</returns>
    public static int SecondsToMilliseconds(this int value) 
        => value * 1000;
    
    /// <summary>
    /// Converts the specified number of minutes to milliseconds.
    /// </summary>
    /// <param name="value">The value in minutes.</param>
    /// <returns>The equivalent number of milliseconds.</returns>
    public static int MinutesToMilliseconds(this int value) 
        => value * 60 * 1000;
    
    /// <summary>
    /// Converts the specified number of hours to milliseconds.
    /// </summary>
    /// <param name="value">The value in hours.</param>
    /// <returns>The equivalent number of milliseconds.</returns>
    public static int HoursToMilliseconds(this int value) 
        => value * 60 * 60 * 1000;
    
    /// <summary>
    /// Creates a <see cref="DateTimeOffset"/> by adding the specified number of milliseconds
    /// to the current UTC time.
    /// </summary>
    /// <param name="value">The offset in milliseconds.</param>
    /// <returns>A <see cref="DateTimeOffset"/> representing the current UTC time plus the offset.</returns>
    public static DateTimeOffset ToDateTimeOffset(this int value) 
        => DateTimeOffset.UtcNow.AddMilliseconds(value);
}
