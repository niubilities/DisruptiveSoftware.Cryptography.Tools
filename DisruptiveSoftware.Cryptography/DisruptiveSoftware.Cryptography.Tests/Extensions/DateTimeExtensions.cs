using System;

namespace DisruptiveSoftware.Cryptography.Tests.Extensions
{
    using Shouldly;

    public static class DateTimeExtensions
    {
        public static DateTime TruncateMilliseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }

        public static string ToHexString(this byte[] obj)
        {
            return Convert.ToHexString(obj);
        }

        public static byte[] ToByteArray(this string obj)
        {
            return Convert.FromHexString(obj);
        }

        public static void ShouldBe(this byte[] obj, string expected)
        {
            obj.ToHexString().ShouldBe(expected);
        }

        public static void ShouldBe(this string obj, byte[] expected)
        {
            obj.ToByteArray().ShouldBe(expected);
        }
    }
}
