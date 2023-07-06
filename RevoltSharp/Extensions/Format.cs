namespace RevoltSharp;


public static class Format
{
    /// <summary>
    /// Format the text in bold.
    /// </summary>
    public static string Bold(string s)
        => $"**{s}**";

    /// <summary>
    /// Format the text in italics.
    /// </summary>
    public static string Italic(string s)
        => $"*{s}*";


    /// <summary>
    /// Format the text in bold and italics.
    /// </summary>
    public static string BoldItalic(string s)
        => $"***{s}***";

    /// <summary>
    /// Format the text with a strikethrough.
    /// </summary>
    public static string Strikethrough(string s)
        => $"~{s}~";

    /// <summary>
    /// Format the text in a quote block.
    /// </summary>
    public static string Quote(string s)
        => $"> {s}";

    /// <summary>
    /// Format the text in a spoiler block
    /// </summary>
    public static string Spoiler(string s)
        => $"!!{s}!!";

    /// <summary>
    /// Format the text as a link.
    /// </summary>
    public static string Link(string title, string url)
        => $"[{title}]({url})";

    /// <summary>
    /// Format the text as a heading title.
    /// </summary>
    public static string Heading(string s, HeadingFormat headingFormat = HeadingFormat.H1)
        => $"{new string('#', (int)headingFormat)} {s}";

    /// <summary>
    /// Format the text in a code line.
    /// </summary>
    public static string Code(string s)
        => $"`{s}`";

    /// <summary>
    /// Format the text in code block.
    /// </summary>
    public static string CodeBlock(string code, string language = null)
        => language is null ? $"```\n{code}\n```" : $"```{language}\n{code}\n```";
}
public enum HeadingFormat
{
    H1 = 1,

    H2 = 2,

    H3 = 3,

    H4 = 4,

    H5 = 5,

    H6 = 6
}