using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp;

public static class WebhookHelper
{
    public static Task<IReadOnlyCollection<Webhook>> GetWebhooksAsync(this TextChannel channel)
        => GetWebhooksAsync(channel.Client.Rest, channel.Id);

    public static async Task<IReadOnlyCollection<Webhook>> GetWebhooksAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(GetWebhooksAsync));

        WebhookJson[]? Webhooks = await rest.GetAsync<WebhookJson[]>($"/channels/{channelId}/webhooks");
        if (Webhooks == null)
            return Array.Empty<Webhook>();

        return Webhooks.Select(x => new Webhook(rest.Client, x)).ToImmutableArray();
    }

    public static async Task<Webhook> CreateWebhookAsync(this RevoltRestClient rest, string channelId, string webhookName, string webhookAvatarId = null)
    {
        Conditions.ChannelIdLength(channelId, nameof(CreateWebhookAsync));
        Conditions.WebhookNameLength(channelId, nameof(CreateWebhookAsync));

        CreateWebhookRequest Req = new CreateWebhookRequest
        {
            Name = webhookName
        };
        if (!string.IsNullOrEmpty(webhookAvatarId))
        {
            Conditions.WebhookAvatarIdLength(webhookAvatarId, nameof(CreateWebhookAsync));
            Req.Avatar = Optional.Some(webhookAvatarId);
        }

        WebhookJson Data = await rest.PostAsync<WebhookJson>($"channels/{channelId}/webhooks", Req);
        return new Webhook(rest.Client, Data);
    }
}