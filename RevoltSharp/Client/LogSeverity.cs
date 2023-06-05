namespace RevoltSharp;

/// <summary>
/// The severity of a log message raised by <see cref="ClientEvents.OnLog"/>.
/// </summary>
public enum RevoltLogSeverity
{
    /// <summary>
    /// Debug info message.
    /// </summary>
    Verbose,

    /// <summary>
    /// Error message info.
    /// </summary>
    Error,

    /// <summary>
    /// Standard error message.
    /// </summary>
    Standard
}