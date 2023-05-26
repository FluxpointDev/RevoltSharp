namespace Optionals;

/// <summary>
/// Optional wrapper for specifying properties that can be missing or included.
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Wraps an existing value in an Option&lt;T&gt; instance.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    /// <returns>An optional containing the specified value.</returns>
    internal static Optional<T> Some<T>(this T value) => Optional.Some(value);

    /// <summary>
    /// Creates an empty Option&lt;T&gt; instance from a specified value.
    /// </summary>
    /// <param name="value">A value determining the type of the optional.</param>
    /// <returns>An empty optional.</returns>
    internal static Optional<T> None<T>(this T value) => Optional.None<T>();

}
