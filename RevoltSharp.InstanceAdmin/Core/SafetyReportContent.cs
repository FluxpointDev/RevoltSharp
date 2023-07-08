namespace RevoltSharp;
public interface ISafetyReportContent
{
    internal static ISafetyReportContent? Create(RevoltClient client, SafetyReportType type, SafetyReportedContentJson model)
    {
        switch (type)
        {
            case SafetyReportType.Message:
                return new SafetyReportedMessage(client, model);
            case SafetyReportType.Server:
                return new SafetyReportedServer(client, model);
            case SafetyReportType.User:
                return new SafetyReportedUser(client, model);
        }
        return null;
    }
}
public class SafetyReportedServer : CreatedEntity, ISafetyReportContent
{
    internal SafetyReportedServer(RevoltClient client, SafetyReportedContentJson model) : base(client, model.Id)
    {
        ServerId = model.Id;
        Reason = model.Reason.ToEnum<SafetyReportServerReason>();
    }

    public string ServerId { get; internal set; }
    public SafetyReportServerReason Reason { get; internal set; }
}
public class SafetyReportedUser : CreatedEntity, ISafetyReportContent
{
    internal SafetyReportedUser(RevoltClient client, SafetyReportedContentJson model) : base(client, model.Id)
    {
        UserId = model.Id;
        MessageId = model.MessageId;
        Reason = model.Reason.ToEnum<SafetyReportUserReason>();
    }

    public string UserId { get; internal set; }

    public string? MessageId { get; internal set; }
    public SafetyReportUserReason Reason { get; internal set; }
}
public class SafetyReportedMessage : CreatedEntity, ISafetyReportContent
{
    internal SafetyReportedMessage(RevoltClient client, SafetyReportedContentJson model) : base(client, model.MessageId)
    {
        MessageId = model.MessageId;
        Reason = model.Reason.ToEnum<SafetyReportMessageReason>();
    }

    public string MessageId { get; internal set; }

    public SafetyReportMessageReason Reason { get; internal set; }
}
public enum SafetyReportUserReason
{
    NoneSpecified,
    UnsolicitedSpam,
    SpamAbuse,
    InappropriateProfile,
    Impersonation,
    BanEvasion,
    Underage
}
public enum SafetyReportMessageReason
{
    NoneSpecified,
    Illegal,
    IllegalGoods,
    IllegalExtortion,
    IllegalPornography,
    IllegalHacking,
    ExtremeViolence,
    PromotesHarm,
    UnsolicitedSpam,
    Raid,
    SpamAbuse,
    ScamsFraud,
    Malware,
    Harassment
}
public enum SafetyReportServerReason
{
    NoneSpecified,
    Illegal,
    IllegalGoods,
    IllegalExtortion,
    IllegalPornography,
    IllegalHacking,
    ExtremeViolence,
    PromotesHarm,
    UnsolicitedSpam,
    Raid,
    SpamAbuse,
    ScamsFraud,
    Malware,
    Harassment
}