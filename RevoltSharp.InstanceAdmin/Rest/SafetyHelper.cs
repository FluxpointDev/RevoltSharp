using Optionals;
using RevoltSharp.Requests;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;
public static class SafetyHelper
{
    public static async Task<SafetyReport?> GetReportAsync(this AdminClient admin, string reportId)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(GetReportAsync));
        AdminConditions.ReportIdLength(reportId, nameof(GetReportAsync));

        SafetyReportJson? Json = await admin.Client.Rest.GetAsync<SafetyReportJson>("safety/report/" + reportId);
        if (Json == null)
            return null;

        return new SafetyReport(admin.Client, Json);
    }

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsMadeAsync(this User user, Option<SafetyReportType> type = null)
        => GetReportsAsync(user.Client.Admin, authorId: user.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsMadeAsync(this AdminClient admin, string userId, Option<SafetyReportType> type = null)
        => GetReportsAsync(admin, authorId: userId, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this User user, Option<SafetyReportType> type = null)
        => GetReportsAsync(user.Client.Admin, user.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this Server server, Option<SafetyReportType> type = null)
        => GetReportsAsync(server.Client.Admin, server.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this UserMessage message, Option<SafetyReportType> type = null)
        => GetReportsAsync(message.Client.Admin, message.Id, type: type);

    public static async Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this AdminClient admin, string contentId = null, string authorId = null, Option<SafetyReportType> type = null)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(GetReportsAsync));

        QueryBuilder Query = new QueryBuilder()
            .AddIf(!string.IsNullOrEmpty(contentId), "content_id", contentId)
            .AddIf(!string.IsNullOrEmpty(authorId), "author_id", authorId)
            .AddIf(type != null, "status", type.Value.ToString());

        SafetyReportJson[]? Json = await admin.Client.Rest.GetAsync<SafetyReportJson[]>("safety/reports" + Query.GetQuery());
        if (Json == null || Json.Length == 0)
            return Array.Empty<SafetyReport>();

        return Json.Select(x => new SafetyReport(admin.Client, x)).ToImmutableArray();
    }


    public static Task<SafetyReport> RejectAsync(this SafetyReport report, string rejectReason, Option<string> note = null)
        => RejectReportAsync(report.Client.Admin, report.Id, rejectReason, note);

    public static Task<SafetyReport> RejectReportAsync(this AdminClient admin, string reportId, string rejectReason, Option<string> note = null)
        => InternalModifyReportAsync(admin, reportId, nameof(RejectReportAsync), new AdminReportModifyRequest
        {
            status = new AdminReportModifyStatusRequest
            {
                status = SafetyReportStatus.Rejected.ToString(),
                rejection_reason = Optional.Some(rejectReason),
                closed_at = Optional.Some(DateTime.UtcNow)
            },
            notes = note != null ? Optional.Some(note.Value) : Optional.None<string>()
        });

    public static Task<SafetyReport> ResolveAsync(this SafetyReport report, Option<string> note = null)
        => ResolveReportAsync(report.Client.Admin, report.Id, note);

    public static Task<SafetyReport> ResolveReportAsync(this AdminClient admin, string reportId, Option<string> note = null)
        => InternalModifyReportAsync(admin, reportId, nameof(ResolveReportAsync), new AdminReportModifyRequest
        {
            status = new AdminReportModifyStatusRequest
            {
                status = SafetyReportStatus.Resolved.ToString(),
                closed_at = Optional.Some(DateTime.UtcNow)
            },
            notes = note != null ? Optional.Some(note.Value) : Optional.None<string>()
        });

    public static Task<SafetyReport> ResetStatusAsync(this SafetyReport report, Option<string> note = null)
        => ResetReportStatusAsync(report.Client.Admin, report.Id, note);

    public static Task<SafetyReport> ResetReportStatusAsync(this AdminClient admin, string reportId, Option<string> note = null)
        => InternalModifyReportAsync(admin, reportId, nameof(ResetReportStatusAsync), new AdminReportModifyRequest
        {
            status = new AdminReportModifyStatusRequest
            {
                status = SafetyReportStatus.Created.ToString()
            },
            notes = note != null ? Optional.Some(note.Value) : Optional.None<string>()
        });

    public static Task ReportAsync(this User user, SafetyReportServerReason reason, string additionalContext = null)
        => InternalReportContentAsync(user.Client.Admin, new SafetyReportedContentJson
        {
            Id = user.Id,
            Type = SafetyReportType.User.ToString(),
            Reason = reason.ToString()
        }, additionalContext);

    public static Task ReportAsync(this Server server, SafetyReportServerReason reason, string additionalContext = null)
        => InternalReportContentAsync(server.Client.Admin, new SafetyReportedContentJson
        {
            Id = server.Id,
            Type = SafetyReportType.Server.ToString(),
            Reason = reason.ToString()
        }, additionalContext);

    public static Task ReportAsync(this UserMessage message, SafetyReportMessageReason reason, string additionalContext = null)
        => InternalReportContentAsync(message.Client.Admin, new SafetyReportedContentJson
        {
            Id = message.AuthorId,
            MessageId = Optional.Some(message.Id),
            Type = SafetyReportType.Message.ToString(),
            Reason = reason.ToString()
        }, additionalContext);

    internal static async Task InternalReportContentAsync(this AdminClient admin, SafetyReportedContentJson content, string additionalContext)
    {
        await admin.Client.Rest.PostAsync<dynamic>("safety/report", new ReportContentRequest
        {
            content = content,
            additional_context = !string.IsNullOrEmpty(additionalContext) ? Optional.Some(additionalContext) : Optional.None<string>()
        });
    }

    internal static async Task<SafetyReport> InternalModifyReportAsync(this AdminClient admin, string reportId, string Obj, AdminReportModifyRequest req)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, Obj);
        AdminConditions.ReportIdLength(reportId, Obj);
        if (req.status.rejection_reason.HasValue)
            AdminConditions.ReportReasonLength(req.status.rejection_reason.Value, Obj);

        SafetyReportJson Data = await admin.Client.Rest.PatchAsync<SafetyReportJson>("safety/reports/" + reportId, req);

        return new SafetyReport(admin.Client, Data);
    }

    public static Task<IReadOnlyCollection<SafetyUserStrike>> GetStrikesAsync(this User user)
        => GetStrikesAsync(user.Client.Admin, user.Id);


    public static async Task<IReadOnlyCollection<SafetyUserStrike>> GetStrikesAsync(this AdminClient admin, string userId)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(GetStrikesAsync));
        Conditions.UserIdLength(userId, nameof(GetStrikesAsync));

        SafetyUserStrikeJson[] Json = await admin.Client.Rest.GetAsync<SafetyUserStrikeJson[]>("safety/strikes/" + userId);
        if (Json == null || Json.Length == 0)
            return Array.Empty<SafetyUserStrike>();

        return Json.Select(x => new SafetyUserStrike(admin.Client, x)).ToImmutableArray();
    }

    public static Task<SafetyUserStrike> AddStrikeAsync(this User user, string reason)
        => CreateStrikeAsync(user.Client.Admin, user.Id, reason);

    public static async Task<SafetyUserStrike> CreateStrikeAsync(this AdminClient admin, string userId, string reason)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(CreateStrikeAsync));
        Conditions.UserIdLength(userId, nameof(CreateStrikeAsync));
        AdminConditions.StrikeReasonLength(reason, nameof(CreateStrikeAsync));

        SafetyUserStrikeJson Data = await admin.Client.Rest.PostAsync<SafetyUserStrikeJson>("safety/strikes", new AdminStrikeCreateRequest
        {
            user_id = userId,
            reason = reason
        });

        return new SafetyUserStrike(admin.Client, Data);
    }


    public static Task ModifyAsync(this SafetyUserStrike strike, string reason)
        => ModifyStrikeAsync(strike.Client.Admin, strike.Id, reason);

    public static async Task ModifyStrikeAsync(this AdminClient admin, string strikeId, string reason)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(ModifyStrikeAsync));
        AdminConditions.StrikeIdLength(strikeId, nameof(ModifyStrikeAsync));
        AdminConditions.StrikeReasonLength(reason, nameof(ModifyStrikeAsync));

        await admin.Client.Rest.PostAsync<dynamic>("safety/strikes/" + strikeId, new AdminStrikeModifyRequest
        {
            reason = reason
        });
    }

    public static Task DeleteAsync(this SafetyUserStrike strike)
        => DeleteStrikeAsync(strike.Client.Admin, strike.Id);

    public static async Task DeleteStrikeAsync(this AdminClient admin, string strikeId)
    {
        AdminConditions.CheckIsPrivileged(admin.Client, nameof(DeleteStrikeAsync));
        AdminConditions.StrikeIdLength(strikeId, nameof(DeleteStrikeAsync));
        await admin.Client.Rest.DeleteAsync("safety/strikes/" + strikeId);
    }
}
