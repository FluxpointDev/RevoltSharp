using Newtonsoft.Json;

namespace RevoltSharp
{
    public abstract class Entity
    {
        [JsonIgnore]
        public RevoltClient Client { get; }

        protected Entity(RevoltClient client)
        {
            Client = client;
        }
    }
}