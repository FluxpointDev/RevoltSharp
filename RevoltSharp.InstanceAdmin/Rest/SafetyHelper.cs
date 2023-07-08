using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RevoltSharp;
public static class SafetyHelper
{
    public static async Task<SafetyReport?> GetReportAsync(this AdminClient admin, string reportId)
    {
        AdminConditions.ReportIdLength(reportId, nameof(GetReportAsync));

        SafetyReportJson? Json = await admin.Client.Rest.GetAsync<SafetyReportJson>("safety/report/" + reportId);
        if (Json == null)
            return null;

        return new SafetyReport(admin.Client, Json);
    }

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsMadeAsync(this User user, Option<SafetyReportType> type = null)
        => GetReportsAsync(user.Client.Admin, authorId: user.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this User user, Option<SafetyReportType> type = null)
        => GetReportsAsync(user.Client.Admin, user.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this Server server, Option<SafetyReportType> type = null)
        => GetReportsAsync(server.Client.Admin, server.Id, type: type);

    public static Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this UserMessage message, Option<SafetyReportType> type = null)
        => GetReportsAsync(message.Client.Admin, message.Id, type: type);

    public static async Task<IReadOnlyCollection<SafetyReport>> GetReportsAsync(this AdminClient admin, string contentId = null, string authorId = null, Option<SafetyReportType> type = null)
    {
        QueryBuilder Query = new QueryBuilder()
            .AddIf(!string.IsNullOrEmpty(contentId), "content_id", contentId)
            .AddIf(!string.IsNullOrEmpty(authorId), "author_id", authorId)
            .AddIf(type != null, "status", type.Value.ToString());

        SafetyReportJson[]? Json = await admin.Client.Rest.GetAsync<SafetyReportJson[]>("safety/reports" + Query.GetQuery());
        if (Json == null || Json.Length == 0)
            return Array.Empty<SafetyReport>();

        return Json.Select(x => new SafetyReport(admin.Client, x)).ToImmutableArray();
    }
}
