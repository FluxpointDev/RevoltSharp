namespace RevoltSharp;


/// <summary>
/// System messages sent by Revolt for information/changes.
/// </summary>
/// <typeparam name="Type"></typeparam>
public class SystemMessage<Type> : Message where Type : SystemData
{
    /// <summary>
    /// The content data of this system message.
    /// </summary>
    public Type Data { get; internal set; }

    /// <summary>
    /// The type of system message this is.
    /// </summary>
    public SystemType SystemType { get; internal set; }

    internal SystemMessage(RevoltClient client, MessageJson model, Type type, SystemType systemType)
        : base(client, model)
    {
        base.Type = MessageType.System;
        SystemType = systemType;
        Data = type;
        Data.BaseId = model.System.Id;
        Data.BaseBy = model.System.By;
        Data.BaseName = model.System.Name;
        Data.BaseFrom = model.System.From;
        Data.BaseTo = model.System.To;
        Data.BaseText = model.System.Content;
    }

    /// <summary> Returns a string that represents the current object.</summary>
    public override string ToString()
    {
        return SystemType.ToString();
    }
}