using System;

namespace RevoltSharp;
internal static class TimestampExtension
{
    internal static long ToTimestamp(this DateTime date)
    {
        long epoch = (date.Ticks - 621355968000000000) / 10000000;
        return epoch;
    }
}
