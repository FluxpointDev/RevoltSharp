namespace RevoltSharp
{
    /// <summary>
    /// Server invite
    /// </summary>
    public class Invite
    {
        internal Invite(InviteJson model)
        {
            Code = model.Code;
            ChannelId = model.ChannelId;
            ChannelName = model.ChannelName;
            ChannelDescription = model.ChannelDescription;
            CreatorName = model.CreatorName;
            CreatorAvatar = model.CreatorAvatar != null ? new Attachment(model.CreatorAvatar) : null;
            if (model.ChannelType == "Server")
                IsServer = true;
            else if (model.ChannelType == "Group")
                IsGroup = true;
        }
        public string Code { get; internal set; }
        public string ChannelId { get; internal set; }
        public string ChannelName { get; internal set; }
        public string ChannelDescription { get; internal set; }
        public string CreatorName { get; internal set; }
        public Attachment CreatorAvatar { get; internal set; }
        public bool IsServer { get; internal set; }
        public bool IsGroup { get; internal set; }
    }

    public class CreatedInvite
    {
        internal CreatedInvite(CreateInviteJson model)
        {
            Code = model.Code;
            CreatorId = model.CreatorId;
            ChannelId = model.ChannelId;
            if (model.ChannelType == "Server")
                IsServer = true;
            else if (model.ChannelType == "Group")
                IsGroup = true;
        }

        public string Code { get; internal set; }
        public string CreatorId { get; internal set; }

        public string ChannelId { get; internal set; }
        public bool IsServer { get; internal set; }
        public bool IsGroup { get; internal set; }
    }
}
