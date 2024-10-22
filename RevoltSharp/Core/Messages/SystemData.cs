namespace RevoltSharp;

/// <summary>
/// Raw system data for the system message.
/// </summary>
public abstract class SystemData
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
public class SystemDataUnknown : SystemData
{
    internal SystemDataUnknown()
    {
        Name = BaseName!;
        Text = BaseText!;
        Id = BaseId!;
        By = BaseBy!;
        From = BaseFrom!;
        To = BaseTo!;
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
public class SystemDataText : SystemData
{
    internal SystemDataText()
    {
        Text = BaseText!;
    }
    public string Text { get; internal set; }
}

/// <summary>
/// User has been added to a group channel
/// </summary>
public class SystemDataUserAdded : SystemData
{
    internal SystemDataUserAdded()
    {
        Id = BaseId!;
        By = BaseBy!;
    }
    public string Id { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// User has been removed from the group channel
/// </summary>
public class SystemDataUserRemoved : SystemData
{
    internal SystemDataUserRemoved()
    {
        Id = BaseId!;
        By = BaseBy!;
    }
    public string Id { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// Member has joined the server
/// </summary>
public class SystemDataUserJoined : SystemData
{
    internal SystemDataUserJoined()
    {
        Id = BaseId!;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has left the server
/// </summary>
public class SystemDataUserLeft : SystemData
{
    internal SystemDataUserLeft()
    {
        Id = BaseId!;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has been kicked from the server
/// </summary>
public class SystemDataUserKicked : SystemData
{
    internal SystemDataUserKicked()
    {
        Id = BaseId!;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Member has been banned from the server
/// </summary>
public class SystemDataUserBanned : SystemData
{
    internal SystemDataUserBanned()
    {
        Id = BaseId!;
    }
    public string Id { get; internal set; }
}

/// <summary>
/// Group channel name has been changed
/// </summary>
public class SystemDataChannelRenamed : SystemData
{
    internal SystemDataChannelRenamed()
    {
        Name = BaseName!;
        By = BaseBy!;
    }
    public string Name { get; internal set; }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel description has been changed
/// </summary>
public class SystemDataChannelDescriptionChanged : SystemData
{
    internal SystemDataChannelDescriptionChanged()
    {
        By = BaseBy!;
    }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel icon has been changed
/// </summary>
public class SystemDataChannelIconChanged : SystemData
{
    internal SystemDataChannelIconChanged()
    {
        By = BaseBy!;
    }
    public string By { get; internal set; }
}

/// <summary>
/// Group channel ownership has been changed
/// </summary>
public class SystemDataChannelOwnershipChanged : SystemData
{
    internal SystemDataChannelOwnershipChanged()
    {
        From = BaseFrom!;
        To = BaseTo!;
    }
    public string From { get; internal set; }
    public string To { get; internal set; }
}
