using System;

namespace RevoltSharp.Commands;

/// <summary>
///     Marks the execution information for a command.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandAttribute : Attribute
{
    /// <summary>
    ///     Gets the text that has been set to be recognized as a command.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Ignore extra arguments passed instead of erroring.
    /// </summary>
    public bool? IgnoreExtraArgs { get; }

    /// <summary>
    /// Initializes a new <see cref="CommandAttribute" /> attribute with the specified name.
    /// </summary>
    public CommandAttribute()
    {
        Text = string.Empty;
    }

    /// <summary>
    ///     Initializes a new <see cref="CommandAttribute" /> attribute with the specified name.
    /// </summary>
    /// <param name="text">The name of the command.</param>
    public CommandAttribute(string text)
    {
        Text = text;
    }

    /// <summary>
    ///     Initializes a new <see cref="CommandAttribute" /> attribute with the specified name.
    /// </summary>
    /// <param name="text">The name of the command.</param>
    /// <param name="ignoreExtraArgs">Ignore extra arguments passed.</param>
    public CommandAttribute(string text, bool ignoreExtraArgs)
    {
        Text = text;
        IgnoreExtraArgs = ignoreExtraArgs;
    }
}
