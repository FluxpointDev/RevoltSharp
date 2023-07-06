using Optionals;

namespace RevoltSharp;


public class ServerUpdatedProperties
{
    internal ServerUpdatedProperties(Server server, PartialServerJson json)
    {
        Name = json.Name;
        if (json.Icon.HasValue)
            Icon = Optional.Some(server.Icon);
        if (json.Banner.HasValue)
            Banner = Optional.Some(server.Banner);
        Description = json.Description;
        DefaultPermissions = json.DefaultPermissions;
        Analytics = json.Analytics;
        Discoverable = json.Discoverable;
        Nsfw = json.Nsfw;
        Owner = json.Owner;
        SystemMessages = Optional.Some(server.SystemMessages);
    }

    public Optional<string> Name { get; set; }
    public Optional<Attachment?> Icon { get; set; }
    public Optional<Attachment?> Banner { get; set; }
    public Optional<string> Description { get; set; }
    public Optional<ulong> DefaultPermissions { get; set; }
    public Optional<bool> Analytics { get; private set; }
    public Optional<bool> Discoverable { get; private set; }
    public Optional<bool> Nsfw { get; private set; }
    public Optional<string> Owner { get; private set; }
    public Optional<ServerSystemMessages> SystemMessages { get; private set; }
}