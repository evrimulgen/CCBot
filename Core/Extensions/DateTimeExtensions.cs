using System;

namespace Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string GetDateTimeStringAustrianFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(unixTimeStamp)
                .ToLocalTime();
        }

        public static double DateTimeToUnixTimeStamp(this DateTime dateTime)
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static double SubstractsecondsFromUnixTimeStamp(this double unixTimeStamp, int seconds)
        {
            return Math.Floor(unixTimeStamp - seconds);
        }

        public static double ConvertDateTimeAndSubstractSeconds(this DateTime dateTime, int seconds)
        {
            return dateTime.DateTimeToUnixTimeStamp().SubstractsecondsFromUnixTimeStamp(seconds);
        }
    }
}
