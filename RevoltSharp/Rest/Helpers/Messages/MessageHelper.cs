using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RevoltSharp;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Revolt http/rest methods for messages.
/// </summary>
public static class MessageHelper
{
    /// <inheritdoc cref="SendMessageAsync(RevoltRestClient, string, string, Embed[], string[], MessageMasquerade, MessageInteractions, MessageReply[], MessageFlag)" />
    public static Task<UserMessage> SendMessageAsync(this Channel channel, string? text, Embed[]? embeds = null, string[]? attachments = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
        => SendMessageAsync(channel.Client.Rest, channel.Id, text, embeds, attachments, masquerade, interactions, replies, flags);

    /// <summary>
    /// Send a message to the channel.
    /// </summary>
    /// <returns>
    /// <see cref="UserMessage"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<UserMessage> SendMessageAsync(this RevoltRestClient rest, string channelId, string? text, Embed[]? embeds = null, string[]? attachments = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
    {
        Conditions.ChannelIdLength(channelId, nameof(SendMessageAsync));
        Conditions.MessagePropertiesEmpty(text, attachments, embeds, nameof(SendMessageAsync));

        if (embeds != null)
        {
            Conditions.EmbedsNotAllowedForUsers(rest, embeds, nameof(SendMessageAsync));

            IEnumerable<Task> uploadTasks = embeds.Where(x => !string.IsNullOrEmpty(x.Image)).Select(async x =>
            {
                if (x.Image.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || x.Image.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] Bytes = await rest.FileHttpClient.GetByteArrayAsync(x.Image);
                    try
                    {
                        FileAttachment Upload = await rest.UploadFileAsync(Bytes, "image.png", UploadFileType.Attachment);
                        x.Image = Upload.Id;
                    }
                    catch { }
                }
                else if (x.Image.Contains('/') || x.Image.Contains('\\'))
                {
                    if (!System.IO.File.Exists(x.Image))
                        throw new RevoltArgumentException("Embed image url path does not exist.");
                    try
                    {
                        FileAttachment Upload = await rest.UploadFileAsync(x.Image, UploadFileType.Attachment);
                        x.Image = Upload.Id;
                    }
                    catch { }
                }

            });
            if (uploadTasks.Any())
                await Task.WhenAll(uploadTasks);
        }

        SendMessageRequest Req = new SendMessageRequest
        {
            //nonce = Guid.NewGuid().ToString()
        };
        if (!string.IsNullOrEmpty(text))
        {
            Conditions.MessageContentLength(text, nameof(SendMessageAsync));
            Req.content = Optional.Some(text);
        }
        if (flags.HasFlag(MessageFlag.SupressNotifications))
            Req.flags = Optional.Some(flags);

        if (attachments != null && attachments.Any())
        {
            Req.attachments = Optional.Some(attachments);
        }

        if (embeds != null && embeds.Any())
            Req.embeds = Optional.Some(embeds.Select(x => x.ToJson()).ToArray());

        if (masquerade != null)
        {
            Conditions.MasqueradeNameLength(masquerade.Name, nameof(SendMessageAsync));
            Conditions.MasqueradeAvatarUrlLength(masquerade.AvatarUrl, nameof(SendMessageAsync));

            Req.masquerade = Optional.Some(masquerade.ToJson());
        }

        if (replies != null && replies.Any())
        {
            Conditions.ReplyListCount(replies, nameof(SendMessageAsync));
            Req.replies = Optional.Some(replies.Select(x => x.ToJson()).ToArray());
        }

        if (interactions != null)
            Req.interactions = Optional.Some(interactions.ToJson());


        MessageJson Data = await rest.PostAsync<MessageJson>($"channels/{channelId}/messages", Req);
        return (UserMessage)Message.Create(rest.Client, Data);
    }

    /// <inheritdoc cref="SendFileAsync(RevoltRestClient, string, byte[], string, string, Embed[], MessageMasquerade, MessageInteractions, MessageReply[], MessageFlag)" />
    public static Task<UserMessage> SendFileAsync(this Channel channel, string filePath, string? text = null, Embed[]? embeds = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
    => SendFileAsync(channel.Client.Rest, channel.Id, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies, flags);

    /// <inheritdoc cref="SendFileAsync(RevoltRestClient, string, byte[], string, string, Embed[], MessageMasquerade, MessageInteractions, MessageReply[], MessageFlag)" />
    public static Task<UserMessage> SendFileAsync(this Channel channel, byte[] bytes, string fileName, string? text = null, Embed[]? embeds = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
    => SendFileAsync(channel.Client.Rest, channel.Id, bytes, fileName, text, embeds, masquerade, interactions, replies, flags);

    /// <inheritdoc cref="SendFileAsync(RevoltRestClient, string, byte[], string, string, Embed[], MessageMasquerade, MessageInteractions, MessageReply[], MessageFlag)" />
    public static Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, string filePath, string? text = null, Embed[]? embeds = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
    => SendFileAsync(rest, channelId, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies, flags);

    /// <summary>
    /// Upload a file and send a message to the channel.
    /// </summary>
    /// <returns>
    /// <see cref="UserMessage"/> 
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, byte[] bytes, string fileName, string? text = null, Embed[]? embeds = null, MessageMasquerade? masquerade = null, MessageInteractions? interactions = null, MessageReply[]? replies = null, MessageFlag flags = MessageFlag.None)
    {
        Conditions.FileBytesEmpty(bytes, nameof(SendFileAsync));
        Conditions.FileNameEmpty(fileName, nameof(SendFileAsync));
        Conditions.MessageContentLength(text, nameof(SendFileAsync));
        Conditions.EmbedsNotAllowedForUsers(rest, embeds, nameof(SendFileAsync));

        FileAttachment File = await rest.UploadFileAsync(bytes, fileName, UploadFileType.Attachment);
        return await rest.SendMessageAsync(channelId, text, embeds, new string[] { File.Id }, masquerade, interactions, replies, flags).ConfigureAwait(false);
    }

    /// <inheritdoc cref="GetMessagesAsync(RevoltRestClient, string, int, bool, string, string, string)" />
    public static Task<IReadOnlyCollection<Message>> GetMessagesAsync(this Channel channel, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
        => GetMessagesAsync(channel.Client.Rest, channel.Id, messageCount, includeUserDetails, beforeMessageId, afterMessageId);

    /// <summary>
    /// Get a list of messages from the channel up to 100.
    /// </summary>
    /// <returns>
    /// List of <see cref="Message"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<IReadOnlyCollection<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, int messageCount = 100, bool includeUserDetails = false, string nearbyMessageId = "", string beforeMessageId = "", string afterMessageId = "")
    {
        Conditions.ChannelIdLength(channelId, nameof(GetMessagesAsync));
        Conditions.MessageSearchCount(messageCount, nameof(GetMessagesAsync));

        QueryBuilder QueryBuilder = new QueryBuilder()
            .Add("limit", messageCount)
            .Add("include_users", includeUserDetails)
            .Add("sort", "Latest");

        if (!string.IsNullOrEmpty(nearbyMessageId))
        {
            QueryBuilder.Add("nearby", nearbyMessageId);
            Conditions.MessageNearbyIdLength(nearbyMessageId, nameof(GetMessagesAsync));
        }
        if (!string.IsNullOrEmpty(afterMessageId))
        {
            QueryBuilder.Add("after", afterMessageId);
            Conditions.MessageNearbyIdLength(afterMessageId, nameof(GetMessagesAsync));
        }
        if (!string.IsNullOrEmpty(beforeMessageId))
        {
            QueryBuilder.Add("before", beforeMessageId);
            Conditions.MessageNearbyIdLength(beforeMessageId, nameof(GetMessagesAsync));
        }

        MessageJson[]? Data = await rest.GetAsync<MessageJson[]>($"channels/{channelId}/messages" + QueryBuilder.GetQuery());
        if (Data == null)
            return Array.Empty<Message>();

        return Data.Select(x => Message.Create(rest.Client, x)).ToImmutableArray();
    }

    /// <inheritdoc cref="GetMessageAsync(RevoltRestClient, string, string)" />
    public static Task<Message?> GetMessageAsync(this Channel channel, string messageId)
        => GetMessageAsync(channel.Client.Rest, channel.Id, messageId);

    /// <summary>
    /// Get a message from the current channel.
    /// </summary>
    /// <returns>
    /// <see cref="Message"/> or <see langword="null" />
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<Message?> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdLength(channelId, nameof(GetMessageAsync));
        Conditions.MessageIdLength(messageId, nameof(GetMessageAsync));

        MessageJson? Data = await rest.GetAsync<MessageJson>($"channels/{channelId}/messages/{messageId}");
        if (Data == null)
            return null;

        Message msg = Message.Create(rest.Client, Data);

        if (msg.Type == MessageType.User && rest.Client.Mode == ClientMode.Http)
        {
            rest.Client.InvokeLog("Http mode fetching ", RevoltLogSeverity.Debug);
            msg.Author = await rest.GetUserAsync(msg.AuthorId);
        }
        else
        {
            if (msg.Type == MessageType.User && msg.Author == null)
                msg.Author = await rest.GetUserAsync(msg.AuthorId);
        }

        return msg;
    }

    /// <inheritdoc cref="EditMessageAsync(RevoltRestClient, string, string, Option{string}, Option{Embed[]})" />
    public static Task<UserMessage> EditMessageAsync(this UserMessage msg, Option<string> content = null, Option<Embed[]> embeds = null)
        => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

    /// <summary>
    /// Edit a message sent by the current user/bot account with properties.
    /// </summary>
    /// <returns>
    /// <see cref="UserMessage"/>
    /// </returns>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task<UserMessage> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Option<string> content = null, Option<Embed[]> embeds = null)
    {
        Conditions.ChannelIdLength(channelId, nameof(EditMessageAsync));
        Conditions.MessageIdLength(messageId, nameof(EditMessageAsync));

        SendMessageRequest Req = new SendMessageRequest();
        if (content != null)
            Req.content = Optional.Some(content.Value);

        if (embeds != null)
            Req.embeds = embeds.Value != null ? Optional.Some(embeds.Value.Select(x => x.ToJson()).ToArray()) : Optional.Some(Array.Empty<EmbedJson>());

        MessageJson Data = await rest.PatchAsync<MessageJson>($"channels/{channelId}/messages/{messageId}", Req);
        return (UserMessage)Message.Create(rest.Client, Data);
    }

    /// <inheritdoc cref="DeleteMessageAsync(RevoltRestClient, string, string)" />
    public static Task DeleteAsync(this Message mes)
      => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

    /// <inheritdoc cref="DeleteMessageAsync(RevoltRestClient, string, string)" />
    public static Task DeleteMessageAsync(this Channel channel, Message message)
        => DeleteMessageAsync(channel.Client.Rest, channel.Id, message.Id);

    /// <inheritdoc cref="DeleteMessageAsync(RevoltRestClient, string, string)" />
    public static Task DeleteMessageAsync(this Channel channel, string messageId)
        => DeleteMessageAsync(channel.Client.Rest, channel.Id, messageId);

    /// <inheritdoc cref="DeleteMessageAsync(RevoltRestClient, string, string)" />
    public static Task DeleteMessageAsync(this RevoltRestClient rest, Channel channel, Message message)
        => DeleteMessageAsync(rest, channel.Id, message.Id);

    /// <inheritdoc cref="DeleteMessageAsync(RevoltRestClient, string, string)" />
    public static Task DeleteMessageAsync(this RevoltRestClient rest, Channel channel, string messageId)
        => DeleteMessageAsync(rest, channel.Id, messageId);

    /// <summary>
    /// Delete a message from a channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdLength(channelId, nameof(DeleteMessageAsync));
        Conditions.MessageIdLength(messageId, nameof(DeleteMessageAsync));

        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}");
    }

    /// <inheritdoc cref="DeleteMessagesAsync(RevoltRestClient, string, string[])" />
    public static Task DeleteMessagesAsync(this Channel channel, Message[] messages)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messages.Select(x => x.Id).ToArray());

    /// <inheritdoc cref="DeleteMessagesAsync(RevoltRestClient, string, string[])" />
    public static Task DeleteMessagesAsync(this Channel channel, string[] messageIds)
        => DeleteMessagesAsync(channel.Client.Rest, channel.Id, messageIds);

    /// <summary>
    /// Delete a list of messages from a channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task DeleteMessagesAsync(this RevoltRestClient rest, string channelId, string[] messageIds)
    {
        Conditions.ChannelIdLength(channelId, nameof(DeleteMessagesAsync));
        Conditions.MessageIdsCount(messageIds, nameof(DeleteMessagesAsync));

        await rest.DeleteAsync($"channels/{channelId}/messages/bulk", new BulkDeleteMessagesRequest
        {
            ids = messageIds
        });
    }

    /// <inheritdoc cref="CloseDMChannelAsync(RevoltRestClient, string)" />
    public static Task CloseAsync(this DMChannel dm)
        => CloseDMChannelAsync(dm.Client.Rest, dm.Id);

    /// <inheritdoc cref="CloseDMChannelAsync(RevoltRestClient, string)" />
    public static Task CloseAsync(this RevoltRestClient rest, DMChannel dm)
        => CloseDMChannelAsync(rest, dm.Id);

    /// <summary>
    /// Close a DM channel.
    /// </summary>
    /// <exception cref="RevoltArgumentException"></exception>
    /// <exception cref="RevoltRestException"></exception>
    public static async Task CloseDMChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdLength(channelId, nameof(CloseDMChannelAsync));

        await rest.DeleteAsync($"channels/{channelId}");
    }
}