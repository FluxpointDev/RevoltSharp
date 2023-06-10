using Newtonsoft.Json.Linq;
using Optionals;
using System;
using System.Collections.Generic;

namespace RevoltSharp.Rest;

internal class ModifySelfRequest
{
    internal static Dictionary<string, object> Create(Option<string> avatar, Option<string> statusText, Option<UserStatusType> statusType, Option<string> profileBio, Option<string> profileBackground)
    {
        Dictionary<string, object> Values = new Dictionary<string, object>();
        Optionals.Optional<List<string>> Remove = Optional.None<List<string>>();
        if (avatar != null)
        {
            if (string.IsNullOrEmpty(avatar.Value))
            {
                if (!Remove.HasValue)
                    Remove = Optional.Some(new List<string>());

                Remove.Value.Add("Avatar");
            }
            else
                Values.Add("avatar", avatar.Value);
        }

        JObject Status = new JObject();
        if (statusText != null)
        {
            if (string.IsNullOrEmpty(statusText.Value))
            {
                if (!Remove.HasValue)
                    Remove = Optional.Some(new List<string>());

                Remove.Value.Add("Status.Text");
            }
            else
                Status.Add("text", statusText.Value);
        }

        if (statusType != null)
            Status.Add("presence", statusType.Value.ToString());


        JObject Profile = new JObject();
        if (profileBio != null)
        {
            if (string.IsNullOrEmpty(profileBio.Value))
            {
                if (!Remove.HasValue)
                    Remove = Optional.Some(new List<string>());

                Remove.Value.Add("Profile.Content");
            }
            else
                Profile.Add("content", profileBio.Value);
        }

        if (profileBackground != null)
        {
            if (string.IsNullOrEmpty(profileBackground.Value))
            {
                if (!Remove.HasValue)
                    Remove = Optional.Some(new List<string>());

                Remove.Value.Add("Profile.Background");
            }
            else
                Profile.Add("background", profileBackground.Value);
        }

        Values.Add("status", Status);
        Values.Add("profile", Profile);
        if (Remove.HasValue)
            Values.Add("remove", Remove.Value);
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
