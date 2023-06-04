namespace RevoltSharp.Rest.Requests;

internal class CreateEmojiRequest : IRevoltRequest
{
    public string name { get; internal set; }
    public CreateEmojiParent parent { get; internal set; }
    public bool nsfw { get; internal set; }
}
internal class CreateEmojiParent
{
    public string type { get; internal set; } = "Server";
    public string id { get; internal set; }
}
