using Newtonsoft.Json;

namespace RevoltSharp
{
    public abstract class Entity
    {
        [JsonIgnore]
        internal RevoltClient Client { get; }

        protected Entity(RevoltClient client)
        {
            Client = client;
        }
    }
}