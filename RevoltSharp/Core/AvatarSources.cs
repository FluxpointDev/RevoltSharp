using System;

namespace RevoltSharp;


[Flags]
public enum AvatarSources
{
    Default = 1,
    User = 2,
    Server = 4,

    UserOrDefault = Default | User,
    ServerOrUser = User | Server,
    Any = Server | User | Default,
}