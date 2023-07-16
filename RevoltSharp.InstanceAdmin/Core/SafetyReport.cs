namespace RevoltSharp;
public class SafetyReport : CreatedEntity
{
    internal SafetyReport(RevoltClient client, SafetyReportJson model) : base(client, model.Id)
    {
        Status = model.Status.ToEnum<SafetyReportStatus>();
        AuthorId = model.AuthorId;
        AdditionalContext = model.AdditionalContext;
        Note = model.Note;
        Type = model.Content.Type.ToEnum<SafetyReportType>();
        ReportedContent = ISafetyReportContent.Create(client, Type, model.Content);
    }

    public SafetyReportStatus Status { get; internal set; }

    public string AuthorId { get; internal set; }

    public string AdditionalContext { get; internal set; }

    public string Note { get; internal set; }

    public SafetyReportType Type { get; internal set; }

    public ISafetyReportContent ReportedContent { get; internal set; }
}

public enum SafetyReportStatus
{
    Created, Rejected, Resolved
}
public enum SafetyReportType
{
    Message, User, Server
}