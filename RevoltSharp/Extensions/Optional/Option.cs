namespace Optionals;


/// <summary>
/// Provides a set of functions for creating optional values.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Wraps an existing value in an Option&lt;T&gt; instance.
    /// </summary>
    /// <param name="value">The value to be wrapped.</param>
    /// <returns>An optional containing the specified value.</returns>
    public static Optional<T> Some<T>(T value) => new Optional<T>(value, true);


    /// <summary>
    /// Creates an empty Option&lt;T&gt; instance.
    /// </summary>
    /// <returns>An empty optional.</returns>
    public static Optional<T> None<T>() => new Optional<T>(default(T), false);

}