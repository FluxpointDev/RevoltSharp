using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RevoltSharp;


/// <summary>
/// Represents a Universally Unique Lexicographically Sortable Identifier (ULID).
/// Spec: https://github.com/ulid/spec
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
public partial struct Ulid
{
    // https://en.wikipedia.org/wiki/Base32
    static readonly byte[] CharToBase32 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 255, 255, 255, 255, 255, 255, 255, 10, 11, 12, 13, 14, 15, 16, 17, 255, 18, 19, 255, 20, 21, 255, 22, 23, 24, 25, 26, 255, 27, 28, 29, 30, 31, 255, 255, 255, 255, 255, 255, 10, 11, 12, 13, 14, 15, 16, 17, 255, 18, 19, 255, 20, 21, 255, 22, 23, 24, 25, 26, 255, 27, 28, 29, 30, 31 };

    // Core

    // Timestamp(48bits)
    [FieldOffset(0)] readonly byte timestamp0;
    [FieldOffset(1)] readonly byte timestamp1;
    [FieldOffset(2)] readonly byte timestamp2;
    [FieldOffset(3)] readonly byte timestamp3;
    [FieldOffset(4)] readonly byte timestamp4;
    [FieldOffset(5)] readonly byte timestamp5;

    public DateTimeOffset Time
    {
        get
        {
            Span<byte> buffer = stackalloc byte[8];
            buffer[0] = timestamp5;
            buffer[1] = timestamp4;
            buffer[2] = timestamp3;
            buffer[3] = timestamp2;
            buffer[4] = timestamp1;
            buffer[5] = timestamp0; // [6], [7] = 0

            long timestampMilliseconds = Unsafe.As<byte, long>(ref MemoryMarshal.GetReference(buffer));
            return DateTimeOffset.FromUnixTimeMilliseconds(timestampMilliseconds);
        }
    }


    internal Ulid(ReadOnlySpan<char> base32)
    {
        // unroll-code is based on NUlid.
        timestamp0 = (byte)((CharToBase32[base32[0]] << 5) | CharToBase32[base32[1]]);
        timestamp1 = (byte)((CharToBase32[base32[2]] << 3) | (CharToBase32[base32[3]] >> 2));
        timestamp2 = (byte)((CharToBase32[base32[3]] << 6) | (CharToBase32[base32[4]] << 1) | (CharToBase32[base32[5]] >> 4));
        timestamp3 = (byte)((CharToBase32[base32[5]] << 4) | (CharToBase32[base32[6]] >> 1));
        timestamp4 = (byte)((CharToBase32[base32[6]] << 7) | (CharToBase32[base32[7]] << 2) | (CharToBase32[base32[8]] >> 3));
        timestamp5 = (byte)((CharToBase32[base32[8]] << 5) | CharToBase32[base32[9]]);

    }


    // Factory
    public static Ulid Parse(string base32)
    {
        return Parse(base32.AsSpan());
    }

    public static Ulid Parse(ReadOnlySpan<char> base32)
    {
        if (base32.Length != 26)
            throw new ArgumentException("invalid base32 length, length:" + base32.Length);
        return new Ulid(base32);
    }

    public static bool TryParse(string base32, out Ulid ulid)
    {
        return TryParse(base32.AsSpan(), out ulid);
    }

    internal static bool TryParse(ReadOnlySpan<char> base32, out Ulid ulid)
    {
        if (base32.Length != 26)
        {
            ulid = default(Ulid);
            return false;
        }

        try
        {
            ulid = new Ulid(base32);
            return true;
        }
        catch
        {
            ulid = default(Ulid);
            return false;
        }
    }
}