namespace Optionals;


/// <summary>
/// Represents an optional value.
/// </summary>
public struct Optional<T>
{
    private readonly bool hasValue;
    private readonly T value;

    /// <summary>
    /// Checks if a value is present.
    /// </summary>
    public bool HasValue => hasValue;

    public T Value => value;

    internal Optional(T value, bool hasValue)
    {
        this.value = value;
        this.hasValue = hasValue;
    }

    /// <summary>
    /// Returns the existing value if present, and otherwise an alternative value.
    /// </summary>
    /// <param name="alternative">The alternative value.</param>
    /// <returns>The existing or alternative value.</returns>
    public T ValueOr(T alternative) => hasValue ? value : alternative;

}