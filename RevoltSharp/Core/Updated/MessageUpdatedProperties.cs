using Optionals;
using RevoltSharp.WebSocket;
using RevoltSharp.WebSocket.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevoltSharp
{
    public class MessageUpdatedProperties
    {
        internal MessageUpdatedProperties(RevoltSocketClient client, MessageUpdateEventJson json)
        {
            Client = client;
            Content = json.Data.Content;
            if (json.Data.Embeds.HasValue)
            {
                if (json.Data.Embeds.Value == null || !json.Data.Embeds.Value.Any())
                    Embeds = new Option<MessageEmbed[]>(new MessageEmbed[0]);
                else
                    Embeds = new Option<MessageEmbed[]>(json.Data.Embeds.Value.Select(x => new MessageEmbed(x)).ToArray());
            }
            Edited = json.Data.Edited;
            ChannelId = json.Channel;
            if (Channel is ServerChannel SC)
                ServerId = SC.ServerId;
        }

        private RevoltSocketClient Client;

        public Optional<string> Content;

        public Option<MessageEmbed[]> Embeds;

        public DateTime Edited;

        public string ChannelId;

        public Channel? Channel => Client.ChannelCache.GetValueOrDefault(ChannelId);

        public string ServerId;

        public Server? Server => Client.ServerCache.GetValueOrDefault(ServerId);

    }
}
