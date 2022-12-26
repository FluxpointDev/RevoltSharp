using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace RevoltSharp.Rest
{
    public class ModifySelfRequest
    {
        public static Dictionary<string, object> Create(Option<string> avatar, Option<string> statusText, Option<UserStatusType> statusType, Option<string> profileBio, Option<string> profileBackground)
        {
            Dictionary<string, object> Values = new Dictionary<string, object>();
            if (avatar != null)
                Values.Add("avatar", avatar.Value);
            JObject Status = new JObject();
            if (statusText != null)
                Status.Add("text", statusText.Value);
            if (statusType != null)
                Status.Add("presence", statusType.Value.ToString());
            JObject Profile = new JObject();
            if (profileBio != null)
                Profile.Add("content", profileBio.Value);
            if (profileBackground != null)
                Profile.Add("background", profileBackground.Value);
            Values.Add("status", Status);
            Values.Add("profile", Profile);
            return Values;
        }
    }
    [Flags]
    public enum ModifySelfFlags
    {
        Avatar,
        ProfileBanner,
        ProfileBio,
        StatusText
    }
}
