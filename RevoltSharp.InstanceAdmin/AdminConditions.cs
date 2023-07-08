using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp;
internal class AdminConditions
{
    internal static void ReportIdLength(string id, string request)
    {
        if (string.IsNullOrEmpty(id) || id.Length < 1)
            throw new RevoltArgumentException($"Report id can't be empty for the {request} request.");

        if (id.Length > Const.All_MaxIdLength)
            throw new RevoltArgumentException($"Report id length can't be more than {Const.All_MaxIdLength} characters for the {request} request.");
    }
}
