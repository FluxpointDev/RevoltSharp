using Optionals;

namespace RevoltSharp;

public class MessageMasquerade
{
    public MessageMasquerade(string name, string avatar = "", RevoltColor color = null)
    {
        Name = name;
        AvatarUrl = avatar;
        Color = color == null ? new RevoltColor("") : color;
    }
    private MessageMasquerade(MessageMasqueradeJson model)
    {
        Name = model.Name;
        AvatarUrl = model.AvatarUrl;
        if (model.Color.HasValue)
            Color = new RevoltColor(model.Color.Value);
        else
            Color = new RevoltColor("");
    }

    internal static MessageMasquerade? Create(MessageMasqueradeJson model)
    {
        if (model != null)
            return new MessageMasquerade(model);
        return null;
    }

    public string? Name { get; }
    public string? AvatarUrl { get; }
    public RevoltColor? Color { get; }

    internal MessageMasqueradeJson ToJson()
    {
        MessageMasqueradeJson Json = new MessageMasqueradeJson();
        if (!string.IsNullOrEmpty(Name))
            Json.Name = Name;

        if (!string.IsNullOrEmpty(AvatarUrl))
            Json.AvatarUrl = AvatarUrl;

        if (Color != null && !Color.IsEmpty)
            Json.Color = Optional.Some(Color.Hex);

        return Json;
    }
}
