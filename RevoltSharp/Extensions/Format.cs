namespace RevoltSharp;

public static class Format
{
    public static string Bold(string s)
        => $"**{s}**";

    public static string Italic(string s)
        => $"*{s}*";

    public static string BoldItalic(string s)
        => $"***{s}***";

    public static string Strikethrough(string s)
        => $"~~{s}~~";

    public static string Quote(string s)
        => $"> {s}";

    public static string Subscript(string s)
        => $"~{s}~";

    public static string Superscript(string s)
        => $"^{s}^";

    public static string Spoiler(string s)
        => $"!!{s}!!";

    public static string Link(string title, string url)
        => $"[{title}]({url})";

    public static string Heading(string s, HeadingFormat headingFormat = HeadingFormat.H1)
        => $"{new string('#', (int) headingFormat)} {s}";

    public static string Code(string s)
        => $"`{s}`";

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