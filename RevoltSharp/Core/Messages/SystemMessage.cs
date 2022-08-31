using Newtonsoft.Json.Linq;

namespace RevoltSharp
{
    public class SystemMessage<Type> : Message where Type : SystemType
    {
        public Type Content { get; internal set; }
        internal SystemMessage(RevoltClient client, MessageJson model)
            : base(client)
        {
            Id = model.Id;
            ChannelId = model.Channel;
            Channel = client.GetChannel(ChannelId);
            AuthorId = model.Author;
            Content.BaseId = model.System.Id;
            Content.BaseBy = model.System.By;
            Content.BaseName = model.System.Name;
            Content.BaseFrom = model.System.From;
            Content.BaseTo = model.System.To;
            Content.BaseText = model.System.Content;
        }
    }
    public abstract class SystemType
    {
        internal string BaseText { get; set; }
        internal string BaseId { get; set; }
        internal string BaseBy { get; set; }
        internal string BaseName { get; set; }
        internal string BaseFrom { get; set; }
        internal string BaseTo { get; set; }
    }

    public class SystemUnknown : SystemType
    { }

    public class SystemText : SystemType
    {
        public SystemText()
        {
            Text = base.BaseText;
        }
        public string Text { get; internal set; }
    }
    public class SystemUserAdded : SystemType
    {
        public SystemUserAdded()
        {
            Id = base.BaseId;
            By = base.BaseBy;
        }
        public string Id { get; internal set; }
        public string By { get; internal set; }
    }

    public class SystemUserRemoved : SystemType
    {
        public SystemUserRemoved()
        {
            Id = base.BaseId;
            By = base.BaseBy;
        }
        public string Id { get; internal set; }
        public string By { get; internal set; }
    }

    public class SystemUserJoined : SystemType
    {
        public SystemUserJoined()
        {
            Id = base.BaseId;
        }
        public string Id { get; internal set; }
    }

    public class SystemUserLeft : SystemType
    {
        public SystemUserLeft()
        {
            Id = base.BaseId;
        }
        public string Id { get; internal set; }
    }

    public class SystemUserKicked : SystemType
    {
        public SystemUserKicked()
        {
            Id = base.BaseId;
        }
        public string Id { get; internal set; }
    }

    public class SystemUserBanned : SystemType
    {
        public SystemUserBanned()
        {
            Id = base.BaseId;
        }
        public string Id { get; internal set; }
    }

    public class SystemChannelRenamed : SystemType
    {
        public SystemChannelRenamed()
        {
            Name = base.BaseName;
            By = base.BaseBy;
        }
        public string Name { get; internal set; }
        public string By { get; internal set; }
    }

    public class SystemChannelDescriptionChanged : SystemType
    {
        public SystemChannelDescriptionChanged()
        {
            By = base.BaseBy;
        }
        public string By { get; internal set; }
    }

    public class SystemChannelIconChanged : SystemType
    {
        public SystemChannelIconChanged()
        {
            By = base.BaseBy;
        }
        public string By { get; internal set; }
    }

    public class SystemChannelOwnershipChanged : SystemType
    {
        public SystemChannelOwnershipChanged()
        {
            From = base.BaseFrom;
            To = base.BaseTo;
        }
        public string From { get; internal set; }
        public string To { get; internal set; }
    }
}