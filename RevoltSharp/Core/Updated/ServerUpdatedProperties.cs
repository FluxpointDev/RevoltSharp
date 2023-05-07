using Newtonsoft.Json;
using Optionals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class ServerUpdatedProperties
    {
        internal ServerUpdatedProperties(Server server, PartialServerJson json)
        {
            Name = json.Name;
            if (json.Icon.HasValue)
                Icon = new Optional<Attachment?>(server.Icon);
            if (json.Banner.HasValue)
                Banner = new Optional<Attachment?>(server.Banner);
            Description = json.Description;
            DefaultPermissions = json.DefaultPermissions;
            Analytics = json.Analytics;
            Discoverable = json.Discoverable;
            Nsfw = json.Nsfw;
            Owner = json.Owner;
            SystemMessages = new Optional<ServerSystemMessages>(server.SystemMessages);
        }

        public Optional<string> Name { get; set; }
        public Optional<Attachment?> Icon { get; set; }
        public Optional<Attachment?> Banner { get; set; }
        public Optional<string> Description { get; set; }
        public Optional<ulong> DefaultPermissions { get; set; }
        public Optional<bool> Analytics;
        public Optional<bool> Discoverable;
        public Optional<bool> Nsfw;
        public Optional<string> Owner;
        public Optional<ServerSystemMessages> SystemMessages;
    }
}
