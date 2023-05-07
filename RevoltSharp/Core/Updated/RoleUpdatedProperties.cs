using Newtonsoft.Json;
using Optionals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class RoleUpdatedProperties
    {
        internal RoleUpdatedProperties(Role role, PartialRoleJson json)
        {
            Name = json.Name;
            if (json.Permissions.HasValue)
                Permissions = new Optional<ServerPermissions>(role.Permissions);

            Hoist = json.Hoist;
            Rank = json.Rank;
            Color = json.Colour;
        }

        public Optional<string> Name { get; private set; }

        public Optional<ServerPermissions> Permissions { get; private set; }

        public Optional<bool> Hoist { get; private set; }

        public Optional<BigInteger> Rank { get; private set; }

        public Optional<string> Color { get; private set; }
    }
}
