namespace RevoltSharp;
internal class AdminConditions
{
    internal static void ReportIdLength(string id, string request)
    {
        if (string.IsNullOrEmpty(id))
            throw new RevoltArgumentException($"Report id can't be empty for the {request} request.");

        if (id.Length > Const.All_MaxIdLength)
            throw new RevoltArgumentException($"Report id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
    }

    internal static void StrikeIdLength(string id, string request)
    {
        if (string.IsNullOrEmpty(id))
            throw new RevoltArgumentException($"Strike id can't be empty for the {request} request.");

        if (id.Length > Const.All_MaxIdLength)
            throw new RevoltArgumentException($"Strike id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
    }

    internal static void StrikeReasonLength(string reason, string request)
    {
        if (string.IsNullOrEmpty(reason))
            throw new RevoltArgumentException($"Strike reason can't be empty for the {request} request.");

    }
}
