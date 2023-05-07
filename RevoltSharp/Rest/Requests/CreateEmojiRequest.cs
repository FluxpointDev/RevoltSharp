namespace RevoltSharp.Rest.Requests;

internal class CreateEmojiRequest : RevoltRequest
{
    public string name;
    public CreateEmojiParent parent;
    public bool nsfw;
}
internal class CreateEmojiParent
{
    public string type = "Server";
    public string id;
}
