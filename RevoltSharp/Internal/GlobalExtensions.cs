using System.Numerics;

namespace RevoltSharp;

public static class GlobalExtensions
{
    public static int ToInt(this BigInteger bint)
    {
        if (int.TryParse(bint.ToString(), out int number))
            return number;

        if (bint > 0)
            throw new RevoltArgumentException($"Failed to parse big int because it's bigger than Int.MaxValue ({int.MaxValue})");

        throw new RevoltArgumentException($"Failed to parse big int becasue it's less than Int.Minvalue ({int.MinValue})");
    }
}
