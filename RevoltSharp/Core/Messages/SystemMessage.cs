namespace RevoltSharp;

/// <summary>
/// System messages sent by Revolt for information/changes
/// </summary>
/// <typeparam name="Type"></typeparam>
public class SystemMessage<Type> : Message where Type : SystemType
{
    public Type Content { get; internal set; }
    internal SystemMessage(RevoltClient client, MessageJson model, Type type)
        : base(client, model)
    {
        ChannelId = model.Channel;
        Channel = client.GetChannel(ChannelId);
        if (Channel != null && Channel is ServerChannel SC)
            ServerId = SC.ServerId;

        AuthorId = model.Author;
        Content = type;
        if (Content != null)
        {
            Content.BaseId = model.System.Id;
            Content.BaseBy = model.System.By;
            Content.BaseName = model.System.Name;
            Content.BaseFrom = model.System.From;
            Content.BaseTo = model.System.To;
            Content.BaseText = model.System.Content;
        }
    }
}
public abstract class SystemType
{
    internal string? BaseText { get; set; }
    internal string? BaseId { get; set; }
    internal string? BaseBy { get; set; }
    internal string? BaseName { get; set; }
    internal string? BaseFrom { get; set; }
    internal string? BaseTo { get; set; }
}

/// <summary>
/// Unknown system messages
/// </summary>
public class SystemUnknown : SystemType
{
    public SystemUnknown()
    {
        Name = BaseName;
        Text = BaseText;
        Id = BaseId;
        By = BaseBy;
        From = BaseFrom;
        To = BaseTo;
    }

    public string Name { get; internal set; }
    public string From { get; internal set; }
    public string To { get; internal set; }
    public string Text { get; internal set; }
    public string Id { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// System message with text content
/// </summary>
public class SystemText : SystemType
{
    public SystemText()
    {
        Text = BaseText;
    }
    public string Text { get; internal set; }
}

/// <summary>
/// User has been added to a group channel
/// </summary>
public class SystemUserAdded : SystemType
{
    public SystemUserAdded()
    {
        Id = BaseId;
        By = BaseBy;
    }
    public string Id { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// User has been removed from the group channel
/// </summary>
public class SystemUserRemoved : SystemType
{
    public SystemUserRemoved()
    {
        Id = BaseId;
        By = BaseBy;
    }
    public string Id { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// Member has joined the server
/// </summary>
public class SystemUserJoined : SystemType
{
    public SystemUserJoined()
    {
        Id = BaseId;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has left the server
/// </summary>
public class SystemUserLeft : SystemType
{
    public SystemUserLeft()
    {
        Id = BaseId;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has been kicked from the server
/// </summary>
public class SystemUserKicked : SystemType
{
    public SystemUserKicked()
    {
        Id = BaseId;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has been banned from the server
/// </summary>
public class SystemUserBanned : SystemType
{
    public SystemUserBanned()
    {
        Id = BaseId;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Group channel name has been changed
/// </summary>
public class SystemChannelRenamed : SystemType
{
    public SystemChannelRenamed()
    {
        Name = BaseName;
        By = BaseBy;
    }
    public string Name { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel description has been changed
/// </summary>
public class SystemChannelDescriptionChanged : SystemType
{
    public SystemChannelDescriptionChanged()
    {
        By = BaseBy;
    }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel icon has been changed
/// </summary>
public class SystemChannelIconChanged : SystemType
{
    public SystemChannelIconChanged()
    {
        By = BaseBy;
    }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel ownership has been changed
/// </summary>
public class SystemChannelOwnershipChanged : SystemType
{
    public SystemChannelOwnershipChanged()
    {
        From = BaseFrom;
        To = BaseTo;
    }
    public string From { get; internal set; }
    public string To { get; internal set; }
}