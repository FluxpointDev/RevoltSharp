using Newtonsoft.Json.Linq;

namespace RevoltSharp
{
    public class SystemMessage : Message
    {
        public string EntityId { get; }

        public string By { get; }

        public string Name { get; }

        public SystemMessage(RevoltClient client, MessageJson model)
            : base(client)
        {
            Id = model.Id;
            ChannelId = model.Channel;
            AuthorId = model.Author;

            if (model.Content is not JObject jo)
                return;

            EntityId = jo["id"]?.Value<string>();
            By = jo["by"]?.Value<string>();
            Name = jo["name"]?.Value<string>();
        }
    }
}