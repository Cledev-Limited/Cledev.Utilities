namespace Cledev.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FromUtcToLocalTime(this DateTime utcDate)
    {
        utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
        var result = utcDate.ToLocalTime();
        return result;
    }

    public static DateTime ToDateTime(this int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dateTime;
    }

    public static DateTime ToDateTime(this int? unixTimeStamp, int fallBack)
    {
        unixTimeStamp ??= fallBack;
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp.Value).ToUniversalTime();
        return dateTime;
    }
    
    public static string ToLocalShortDate(this DateTime timeStamp)
    {
        var localDate = timeStamp.FromUtcToLocalTime();
        var result = localDate.ToShortDateString();
        return result;
    }

    public static string ToLocalShortDateAndTime(this DateTime timeStamp)
    {
        var localDate = timeStamp.FromUtcToLocalTime();
        var result = $"{localDate.ToShortDateString()} {localDate.ToShortTimeString()}";
        return result;
    }
}