using System;
using System.Collections.Generic;

namespace Optionals;

/// <summary>
/// Represents an optional value.
/// </summary>
/// <typeparam name="T">The type of the value to be wrapped.</typeparam>
#if !NETSTANDARD10
[Serializable]
#endif
public struct Optional<T> : IEquatable<Optional<T>>, IComparable<Optional<T>>
{
    private readonly bool hasValue;
    private readonly T value;

    /// <summary>
    /// Checks if a value is present.
    /// </summary>
    public bool HasValue => hasValue;

    public T Value => value;

    internal Optional(T value, bool hasValue = true)
    {
        this.value = value;
        this.hasValue = hasValue;
    }

    /// <summary>
    /// Determines whether two optionals are equal.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    public bool Equals(Optional<T> other)
    {
        if (!hasValue && !other.hasValue)
        {
            return true;
        }
        else if (hasValue && other.hasValue)
        {
            return EqualityComparer<T>.Default.Equals(value, other.value);
        }

        return false;
    }

    /// <summary>
    /// Determines whether two optionals are equal.
    /// </summary>
    /// <param name="obj">The optional to compare with the current one.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    public override bool Equals(object obj) => obj is Optional<T> ? Equals((Optional<T>)obj) : false;

    /// <summary>
    /// Determines whether two optionals are equal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
    public static bool operator ==(Optional<T> left, Optional<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two optionals are unequal.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
    public static bool operator !=(Optional<T> left, Optional<T> right) => !left.Equals(right);

    /// <summary>
    /// Generates a hash code for the current optional.
    /// </summary>
    /// <returns>A hash code for the current optional.</returns>
    public override int GetHashCode()
    {
        if (hasValue)
        {
            if (value == null)
            {
                return 1;
            }

            return value.GetHashCode();
        }

        return 0;
    }

    /// <summary>
    /// Compares the relative order of two optionals. An empty optional is
    /// ordered before a non-empty one.
    /// </summary>
    /// <param name="other">The optional to compare with the current one.</param>
    /// <returns>An integer indicating the relative order of the optionals being compared.</returns>
    public int CompareTo(Optional<T> other)
    {
        if (hasValue && !other.hasValue) return 1;
        if (!hasValue && other.hasValue) return -1;
        return Comparer<T>.Default.Compare(value, other.value);
    }

    /// <summary>
    /// Determines if an optional is less than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than the right optional.</returns>
    public static bool operator <(Optional<T> left, Optional<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines if an optional is less than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is less than or equal the right optional.</returns>
    public static bool operator <=(Optional<T> left, Optional<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines if an optional is greater than another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than the right optional.</returns>
    public static bool operator >(Optional<T> left, Optional<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines if an optional is greater than or equal to another optional.
    /// </summary>
    /// <param name="left">The first optional to compare.</param>
    /// <param name="right">The second optional to compare.</param>
    /// <returns>A boolean indicating whether or not the left optional is greater than or equal the right optional.</returns>
    public static bool operator >=(Optional<T> left, Optional<T> right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Returns a string that represents the current optional.
    /// </summary>
    /// <returns>A string that represents the current optional.</returns>
    public override string ToString()
    {
        if (hasValue)
        {
            if (value == null)
            {
                return "Some(null)";
            }

            return string.Format("Some({0})", value);
        }

        return "None";
    }

    /// <summary>
    /// Determines if the current optional contains a specified value.
    /// </summary>
    /// <param name="value">The value to locate.</param>
    /// <returns>A boolean indicating whether or not the value was found.</returns>
    public bool Contains(T value)
    {
        if (hasValue)
        {
            if (this.value == null)
            {
                return value == null;
            }

            return this.value.Equals(value);
        }

        return false;
    }

    /// <summary>
    /// Determines if the current optional contains a value 
    /// satisfying a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
    public bool Exists(Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return hasValue && predicate(value);
    }

    /// <summary>
    /// Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public T ValueOrDefault(T alternative) => hasValue ? value : alternative;

    /// <summary>
    /// Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public T ValueOrDefault(Func<T> alternativeFactory)
    {
        if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));
        return hasValue ? value : alternativeFactory();
    }

}
