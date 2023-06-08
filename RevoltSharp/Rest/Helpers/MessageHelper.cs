using Optionals;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Revolt http/rest methods for messages.
/// </summary>
public static class MessageHelper
{
    public static Task<UserMessage> SendMessageAsync(this Channel channel, string text, Embed[] embeds = null, string[] attachments = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
        => SendMessageAsync(channel.Client.Rest, channel.Id, text, embeds, attachments, masquerade, interactions, replies);

    public static async Task<UserMessage> SendMessageAsync(this RevoltRestClient rest, string channelId, string text, Embed[] embeds = null, string[] attachments = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
    {
        Conditions.ChannelIdEmpty(channelId, "SendMessageAsync");

        if (string.IsNullOrEmpty(text) && (attachments == null || attachments.Length == 0) && (embeds == null || embeds.Length == 0))
            throw new RevoltArgumentException("Message content, attachments and embed can't be empty on SendMessageAsync");

        if (text.Length > 2000)
            throw new RevoltArgumentException("Message content can't be more than 2000 on SendMessageAsync");

        if (rest.Client.UserBot && embeds != null)
            throw new RevoltRestException("User accounts can't send embeds on SendMessageAsync", 401, RevoltErrorType.NotAllowedForUsers);

        if (embeds != null)
        {
            IEnumerable<Task> uploadTasks = embeds.Where(x => !string.IsNullOrEmpty(x.Image)).Select(async x =>
            {
                if (x.Image.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || x.Image.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] Bytes = await rest.FileHttpClient.GetByteArrayAsync(x.Image);
                    FileAttachment Upload = await rest.UploadFileAsync(Bytes, "image.png", UploadFileType.Attachment);
                    x.Image = Upload.Id;
                }
                else if (x.Image.Contains('/') || x.Image.Contains('\\'))
                {
                    if (!System.IO.File.Exists(x.Image))
                        throw new RevoltArgumentException("Embed image url path does not exist.");
                    FileAttachment Upload = await rest.UploadFileAsync(x.Image, UploadFileType.Attachment);
                    x.Image = Upload.Id;
                }

            });
            if (uploadTasks.Any())
                await Task.WhenAll(uploadTasks);
        }

        if (string.IsNullOrEmpty(text))
            text = null;
        SendMessageRequest Req = new SendMessageRequest
        {
            nonce = Guid.NewGuid().ToString()
        };
        if (!string.IsNullOrEmpty(text))
            Req.content = Optional.Some(text);

        if (attachments != null)
            Req.attachments = Optional.Some(attachments);

        if (embeds != null)
            Req.embeds = Optional.Some(embeds.Select(x => x.ToJson()).ToArray());

        if (masquerade != null)
            Req.masquerade = Optional.Some(masquerade.ToJson());

        if (replies != null)
            Req.replies = Optional.Some(replies.Select(x => x.ToJson()).ToArray());

        if (interactions != null)
            Req.interactions = Optional.Some(interactions.ToJson());


        MessageJson Data = await rest.PostAsync<MessageJson>($"channels/{channelId}/messages", Req);
        return (UserMessage)Message.Create(rest.Client, Data);
    }

    public static Task<UserMessage> SendFileAsync(this Channel channel, string filePath, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
    => SendFileAsync(channel.Client.Rest, channel.Id, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies);

    public static Task<UserMessage> SendFileAsync(this Channel channel, byte[] bytes, string fileName, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
    => SendFileAsync(channel.Client.Rest, channel.Id, bytes, fileName, text, embeds, masquerade, interactions, replies);

    public static Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, string filePath, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
    => SendFileAsync(rest, channelId, System.IO.File.ReadAllBytes(filePath), filePath.Split('/').Last().Split('\\').Last(), text, embeds, masquerade, interactions, replies);

    public static async Task<UserMessage> SendFileAsync(this RevoltRestClient rest, string channelId, byte[] bytes, string fileName, string text = null, Embed[] embeds = null, MessageMasquerade masquerade = null, MessageInteractions interactions = null, MessageReply[] replies = null)
    {
        Conditions.FileBytesEmpty(bytes, "SendFileAsync");
        Conditions.FileNameEmpty(fileName, "SendFileAsync");

        if (text.Length > 2000)
            throw new RevoltArgumentException("Message content can't be more than 2000 on SendFileAsync");

        if (rest.Client.UserBot && embeds != null)
            throw new RevoltRestException("User accounts can't send embeds on SendFileAsync", 401, RevoltErrorType.NotAllowedForUsers);

        FileAttachment File = await rest.UploadFileAsync(bytes, fileName, UploadFileType.Attachment);
        return await rest.SendMessageAsync(channelId, text, embeds, new string[] { File.Id }, masquerade, interactions, replies).ConfigureAwait(false);
    }

    public static Task<IReadOnlyCollection<Message>> GetMessagesAsync(this Channel channel, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
        => GetMessagesAsync(channel.Client.Rest, channel.Id, messageCount, includeUserDetails, beforeMessageId, afterMessageId);

    public static async Task<IReadOnlyCollection<Message>> GetMessagesAsync(this RevoltRestClient rest, string channelId, int messageCount = 100, bool includeUserDetails = false, string beforeMessageId = "", string afterMessageId = "")
    {
        Conditions.ChannelIdEmpty(channelId, "GetMessagesAsync");

        GetMessagesRequest Req = new GetMessagesRequest
        {
            limit = messageCount,
            include_users = includeUserDetails
        };
        if (!string.IsNullOrEmpty(afterMessageId))
            Req.after = Optional.Some(afterMessageId);
        if (!string.IsNullOrEmpty(beforeMessageId))
            Req.after = Optional.Some(beforeMessageId);
        MessageJson[]? Data = await rest.GetAsync<MessageJson[]>($"channels/{channelId}/messages", Req);
        if (Data == null)
            return Array.Empty<Message>();

        return Data.Select(x => Message.Create(rest.Client, x)).ToImmutableArray();
    }

    public static Task<Message?> GetMessageAsync(this Channel channel, string messageId)
        => GetMessageAsync(channel.Client.Rest, channel.Id, messageId);

    public static async Task<Message?> GetMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdEmpty(channelId, "GetMessageAsync");
        Conditions.MessageIdEmpty(messageId, "GetMessageAsync");

        MessageJson? Data = await rest.GetAsync<MessageJson>($"channels/{channelId}/messages/{messageId}");
        if (Data == null)
            return null;

        return Message.Create(rest.Client, Data);
    }

    public static Task<UserMessage> EditMessageAsync(this UserMessage msg, Option<string> content = null, Option<Embed[]> embeds = null)
        => EditMessageAsync(msg.Client.Rest, msg.ChannelId, msg.Id, content, embeds);

    public static async Task<UserMessage> EditMessageAsync(this RevoltRestClient rest, string channelId, string messageId, Option<string> content = null, Option<Embed[]> embeds = null)
    {
        Conditions.ChannelIdEmpty(channelId, "EditMessageAsync");
        Conditions.MessageIdEmpty(messageId, "EditMessageAsync");

        SendMessageRequest Req = new SendMessageRequest();
        if (content != null)
            Req.content = Optional.Some(content.Value);

        if (embeds != null)
            Req.embeds = embeds.Value != null ? Optional.Some(embeds.Value.Select(x => x.ToJson()).ToArray()) : Optional.Some(Array.Empty<EmbedJson>()) ;

        MessageJson Data = await rest.PatchAsync<MessageJson>($"channels/{channelId}/messages/{messageId}", Req);
        return (UserMessage)Message.Create(rest.Client, Data);
    }


    public static Task DeleteAsync(this Message mes)
      => DeleteMessageAsync(mes.Channel.Client.Rest, mes.ChannelId, mes.Id);

    public static Task DeleteMessageAsync(this Channel channel, Message message)
        => DeleteMessageAsync(channel.Client.Rest, channel.Id, message.Id);

    public static Task DeleteMessageAsync(this Channel channel, string messageId)
        => DeleteMessageAsync(channel.Client.Rest, channel.Id, messageId);

    public static async Task DeleteMessageAsync(this RevoltRestClient rest, string channelId, string messageId)
    {
        Conditions.ChannelIdEmpty(channelId, "DeleteMessageAsync");
        Conditions.MessageIdEmpty(messageId, "DeleteMessageAsync");


        await rest.DeleteAsync($"channels/{channelId}/messages/{messageId}");
    }

    public static Task CloseAsync(this DMChannel dm)
        => CloseDMChannelAsync(dm.Client.Rest, dm.Id);

    public static async Task CloseDMChannelAsync(this RevoltRestClient rest, string channelId)
    {
        Conditions.ChannelIdEmpty(channelId, "CloseDMChannelAsync");

        await rest.DeleteAsync($"channels/{channelId}");
    }
}
