namespace RevoltSharp
{
    internal class RoleJson
    {
        public string name;
        public int[] permissions;
        public bool hoist;
        public int rank;
        public string colour;

        public Role ToEntity(string roleId)
        {
            return new Role
            {
                Id = roleId,
                Color = colour,
                Hoist = hoist,
                Name = name,
                Permissions = permissions,
                Rank = rank
            };
        }
    }
}
