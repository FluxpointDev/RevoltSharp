using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace RevoltSharp;


/// <summary>
/// Represents a Universally Unique Lexicographically Sortable Identifier (ULID).
/// Spec: https://github.com/ulid/spec
/// </summary>
public struct Ulid
{
    private const int VALID_ULID_STRING_LENGTH = 26;
    private static readonly char[] CrockfordsBase32 = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z' };

    private const int TIMESTAMP_LENGTH = 6;
    private const int RANDOMNESS_LENGTH = 10;
    private const int DATA_LENGTH = TIMESTAMP_LENGTH + RANDOMNESS_LENGTH;

    private static long LastUsedTimeStamp /* = 0 */;
    private static readonly byte[] LastUsedRandomness = new byte[RANDOMNESS_LENGTH];
    private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    private static readonly object LOCK = new object();

    public byte TimeStamp_0 { get; set; }
    public byte TimeStamp_1 { get; set; }
    public byte TimeStamp_2 { get; set; }
    public byte TimeStamp_3 { get; set; }
    public byte TimeStamp_4 { get; set; }
    public byte TimeStamp_5 { get; set; }

    public byte Randomness_0 { get; set; }
    public byte Randomness_1 { get; set; }
    public byte Randomness_2 { get; set; }
    public byte Randomness_3 { get; set; }
    public byte Randomness_4 { get; set; }
    public byte Randomness_5 { get; set; }
    public byte Randomness_6 { get; set; }
    public byte Randomness_7 { get; set; }
    public byte Randomness_8 { get; set; }
    public byte Randomness_9 { get; set; }

    // <summary>
    /// Date and time of the created object.
    /// </summary>
    public DateTimeOffset Time
    {
        get
        {
            Span<byte> buffer = stackalloc byte[8];
            buffer[0] = TimeStamp_5;
            buffer[1] = TimeStamp_4;
            buffer[2] = TimeStamp_3;
            buffer[3] = TimeStamp_2;
            buffer[4] = TimeStamp_1;
            buffer[5] = TimeStamp_0; // [6], [7] = 0

            long timestampMilliseconds = Unsafe.As<byte, long>(ref MemoryMarshal.GetReference(buffer));
            return DateTimeOffset.FromUnixTimeMilliseconds(timestampMilliseconds);
        }
    }



    public static Ulid NewUlid()
    {
        /*
         * Ensure thread safety and monotonicity
         * Using lock instead of mutex, making thread keep spinning, as this process should be over quickly
         */
        lock (LOCK)
        {
            DateTime now = DateTime.UtcNow;
            long timestamp = (long)(now - EPOCH).TotalMilliseconds;

#if NETSTANDARD2_1_OR_GREATER
                Span<byte> randomness = stackalloc byte[LastUsedRandomness.Length];
#else
            byte[] randomness = new byte[LastUsedRandomness.Length];
#endif

            if (timestamp == LastUsedTimeStamp)
            {
                //Increment by one
#if NETSTANDARD2_1_OR_GREATER
                    LastUsedRandomness.AsSpan().CopyTo(randomness);
#else
                LastUsedRandomness.CopyTo(randomness, 0);
#endif
                AddOne(randomness);
            }
            else
            {
                //Use Crypto random
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randomness);
                }

                LastUsedTimeStamp = timestamp;
            }

            randomness.CopyTo(LastUsedRandomness, 0);

            return new UlidTimestampHelper(timestamp).Create(randomness);
        }
    }

    private static void AddOne(Span<byte> bytes)
    {
        for (int index = bytes.Length - 1; index >= 0; index--)
        {
            if (bytes[index] < byte.MaxValue)
            {
                ++bytes[index];
                return;
            }

            bytes[index] = 0;
        }
    }

    public override string ToString() => StringExtensions.Create(VALID_ULID_STRING_LENGTH, new UlidStringHelper(this), (buffer, value) =>
    {
        for (int index = 0; index < buffer.Length; ++index)
        {
            int i = value[index];
            buffer[index] = CrockfordsBase32[i];
        }
    });

    // https://en.wikipedia.org/wiki/Base32
    static readonly byte[] CharToBase32 = new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 255, 255, 255, 255, 255, 255, 255, 10, 11, 12, 13, 14, 15, 16, 17, 255, 18, 19, 255, 20, 21, 255, 22, 23, 24, 25, 26, 255, 27, 28, 29, 30, 31, 255, 255, 255, 255, 255, 255, 10, 11, 12, 13, 14, 15, 16, 17, 255, 18, 19, 255, 20, 21, 255, 22, 23, 24, 25, 26, 255, 27, 28, 29, 30, 31 };

    internal static Ulid Create(ReadOnlySpan<char> base32)
    {
        var UID = new Ulid();

        // unroll-code is based on NUlid.
        UID.TimeStamp_0 = (byte)((CharToBase32[base32[0]] << 5) | CharToBase32[base32[1]]);
        UID.TimeStamp_1 = (byte)((CharToBase32[base32[2]] << 3) | (CharToBase32[base32[3]] >> 2));
        UID.TimeStamp_2 = (byte)((CharToBase32[base32[3]] << 6) | (CharToBase32[base32[4]] << 1) | (CharToBase32[base32[5]] >> 4));
        UID.TimeStamp_3 = (byte)((CharToBase32[base32[5]] << 4) | (CharToBase32[base32[6]] >> 1));
        UID.TimeStamp_4 = (byte)((CharToBase32[base32[6]] << 7) | (CharToBase32[base32[7]] << 2) | (CharToBase32[base32[8]] >> 3));
        UID.TimeStamp_5 = (byte)((CharToBase32[base32[8]] << 5) | CharToBase32[base32[9]]);
        return UID;
    }

    /// <summary>
    /// Parse a Revolt id to Ulid class.
    /// </summary>
    /// <param name="base32"></param>
    /// <returns><see cref="Ulid"/></returns>
    public static Ulid Parse(string base32)
    {
        return Parse(base32.AsSpan());
    }

    internal static Ulid Parse(ReadOnlySpan<char> base32)
    {
        if (base32.Length != 26)
            throw new ArgumentException("invalid base32 length, length:" + base32.Length);
        return Create(base32);
    }

    /// <summary>
    /// Check if the Revolt id is valid.
    /// </summary>
    /// <param name="base32"></param>
    /// <returns><see langword="bool"/></returns>
    public static bool TryCheck(string base32)
    {
        return Ulid.TryParse(base32, out _);
    }

    /// <summary>
    /// Try parse a Revolt id to Ulid class.
    /// </summary>
    /// <param name="base32"></param>
    /// <param name="ulid"></param>
    /// <returns><see langword="bool"/></returns>
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
            ulid = Create(base32);
            return true;
        }
        catch
        {
            ulid = default(Ulid);
            return false;
        }
    }
}