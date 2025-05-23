﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp.Rest;


/// <summary>
/// The internal http client used for sending requests to the Revolt instance API and built-in extension methods.
/// </summary>
public class RevoltRestClient
{
    internal RevoltRestClient(RevoltClient client)
    {
        Client = client;

        if (string.IsNullOrEmpty(Client.Config.ApiUrl))
        {
            Client.Logger.LogMessage("Client config ApiUrl can not be empty.", RevoltLogSeverity.Error);
            throw new RevoltException("Client config ApiUrl can not be empty.");
        }


        if (!Uri.IsWellFormedUriString(client.Config.ApiUrl, UriKind.Absolute))
        {
            Client.Logger.LogMessage("Client config ApiUrl is an invalid format.", RevoltLogSeverity.Error);
            throw new RevoltException("Client config ApiUrl is an invalid format.");
        }

        if (!Client.Config.ApiUrl.EndsWith('/'))
            Client.Config.ApiUrl = Client.Config.ApiUrl + "/";

        if (string.IsNullOrEmpty(Client.Config.Debug.UploadUrl))
        {
            Client.Logger.LogMessage("Client config UploadUrl can not be empty.", RevoltLogSeverity.Error);
            throw new RevoltException("Client config UploadUrl can not be empty.");
        }


        if (!Uri.IsWellFormedUriString(client.Config.Debug.UploadUrl, UriKind.Absolute))
        {
            Client.Logger.LogMessage("Client config UploadUrl is an invalid format.", RevoltLogSeverity.Error);
            throw new RevoltException("Client config UploadUrl is an invalid format.");
        }

        if (!Client.Config.Debug.UploadUrl.EndsWith('/'))
            Client.Config.Debug.UploadUrl = Client.Config.Debug.UploadUrl + "/";

        var ClientHandler = new HttpClientHandler() { UseProxy = Client.Config.RestProxy != null };
        ClientHandler.Proxy = Client.Config.RestProxy;

        Http = new HttpClient(ClientHandler)
        {
            BaseAddress = new Uri(Client.Config.ApiUrl)
        };

        Http.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent);
        Http.DefaultRequestHeaders.Add("Accept", "application/json");
        var FileHandler = new HttpClientHandler() { UseProxy = Client.Config.RestProxy != null };
        FileHandler.Proxy = Client.Config.RestProxy;
        FileHttpClient = new HttpClient(FileHandler)
        {
            BaseAddress = new Uri(Client.Config.Debug.UploadUrl)
        };
        FileHttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent);

        if (!string.IsNullOrEmpty(Client.Config.CfClearance))
        {
            string cookie = $"cf_clearance={Client.Config.CfClearance}";
            Http.DefaultRequestHeaders.Add("Cookie", cookie);
            FileHttpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    internal RevoltClient Client { get; private set; }
    internal HttpClient Http;
    internal HttpClient FileHttpClient;

    /// <summary>
    /// Send a custom request to the Revolt instance API.
    /// </summary>
    /// <remarks>
    /// Optionally you can also send a C# class as the json body for the request, this is useful for POST/PUT requests.
    /// <para />
    /// You need to interface your custom class using <see cref="IRevoltRequest"/><br/>
    /// CustomClass : RevoltRequest<br/>
    /// {<br/>
    ///     public string option = "Hi"<br/>
    /// }
    /// </remarks>
    /// <returns><see cref="HttpResponseMessage"/></returns>
    public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, IRevoltRequest? json = null)
    => InternalRequest(GetMethod(method), endpoint, json);

    internal Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, object> json) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);

    internal Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, string> json) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);

    internal Task<TResponse?> GetAsync<TResponse>(string endpoint, IRevoltRequest json = null, bool throwGetRequest = false) where TResponse : class
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        => SendRequestAsync<TResponse>(RequestType.Get, endpoint, json, throwGetRequest);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    internal Task DeleteAsync(string endpoint, IRevoltRequest json = null)
        => SendRequestAsync(RequestType.Delete, endpoint, json);

    internal Task<TResponse> DeleteAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Delete, endpoint, json);

    internal Task<TResponse> PatchAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Patch, endpoint, json);

    internal Task PatchAsync(string endpoint, IRevoltRequest json = null)
        => SendRequestAsync(RequestType.Patch, endpoint, json);

    internal Task<TResponse> PutAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Put, endpoint, json);

    internal Task PutAsync(string endpoint, IRevoltRequest json = null)
        => SendRequestAsync(RequestType.Put, endpoint, json);

    internal Task<TResponse> PostAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Post, endpoint, json);

    internal Task PostAsync(string endpoint, IRevoltRequest json = null)
        => SendRequestAsync(RequestType.Post, endpoint, json);

    /// <summary>
    /// Send a custom request to the Revolt instance API.
    /// </summary>
    /// <remarks>
    /// Optionally you can also send a C# class as the json body for the request, this is useful for POST/PUT requests.
    /// <para />
    /// You need to interface your custom class using <see cref="IRevoltRequest"/><br/>
    /// CustomClass : RevoltRequest<br/>
    /// {<br/>
    ///     public string option = "Hi"<br/>
    /// }
    /// </remarks>
    /// <returns>Input your own <see langword="class" /> object to parse the response data from json.</returns>
    public Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, IRevoltRequest json = null, bool throwGetRequest = false) where TResponse : class
        => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json, throwGetRequest);


    internal static HttpMethod GetMethod(RequestType method)
    {
        switch (method)
        {
            case RequestType.Post:
                return HttpMethod.Post;
            case RequestType.Delete:
                return HttpMethod.Delete;
            case RequestType.Patch:
                return HttpMethod.Patch;
            case RequestType.Put:
                return HttpMethod.Put;
            case RequestType.Get:
                break;
        }
        return HttpMethod.Get;
    }

    /// <inheritdoc cref="UploadFileAsync(byte[], string, UploadFileType)" />
    public Task<FileAttachment> UploadFileAsync(string path, UploadFileType type)
        => UploadFileAsync(System.IO.File.ReadAllBytes(path), path.Split('.').Last(), type);


    /// <summary>
    /// Upload a file to the Revolt instance CDN that can be used for attachments, avatars, banners, ect.
    /// </summary>
    /// <returns>Created <see cref="FileAttachment"/></returns>
    /// <exception cref="RevoltRestException"></exception>
    public async Task<FileAttachment> UploadFileAsync(byte[] bytes, string name, UploadFileType type)
    {
        Conditions.FileBytesEmpty(bytes, nameof(UploadFileAsync));
        Conditions.FileNameEmpty(name, nameof(UploadFileAsync));

        HttpRequestMessage Mes = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
        MultipartFormDataContent MP = new MultipartFormDataContent("file");
        ByteArrayContent Image = new ByteArrayContent(bytes);
        MP.Add(Image, "file", name);
        Mes.Content = MP;
        HttpResponseMessage Req = await FileHttpClient.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest? Retry = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                    {
                        Retry = DeserializeJson<RetryRequest>(Stream);
                    }
                }
                catch { }
            }

            if (Retry != null)
            {
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type))
                {
                    Content = MP
                };
                Req = await Http.SendAsync(MesRetry);
            }
        }

        if (Client.Config.Debug.LogRestRequest)
        {
            Client.Logger.LogRestMessage(Req, HttpMethod.Post, "upload: " + GetUploadType(type));

            if (Client.Config.LogMode == RevoltLogSeverity.Debug)
                Client.Logger.LogJson("Rest Send", Req);
        }

        if (!Req.IsSuccessStatusCode)
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                    {
                        Error = DeserializeJson<RestError>(Stream);
                    }
                }
                catch { }
            }
            if (Error != null)
            {
                if (string.IsNullOrEmpty(Error.Permission))
                    throw new RevoltRestException($"Request failed due to {Error.Type} ({(int)Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
                else
                    throw new RevoltPermissionException(Error.Permission, (int)Req.StatusCode, Error.Type == RevoltErrorType.MissingUserPermission);
            }
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        try
        {
            using (Stream Stream = await Req.Content.ReadAsStreamAsync())
            {
                return new FileAttachment(Client, DeserializeJson<FileAttachmentJson>(Stream).id);
            }
        }
        catch (Exception ex)
        {
            throw new RevoltRestException(ex.Message, 500, RevoltErrorType.Unknown);
        }
    }

    internal static string GetUploadType(UploadFileType type)
    {
        switch (type)
        {
            case UploadFileType.Avatar:
                return "avatars";
            case UploadFileType.Background:
                return "backgrounds";
            case UploadFileType.Banner:
                return "banners";
            case UploadFileType.Icon:
                return "icons";
            case UploadFileType.Emoji:
                return "emojis";
            case UploadFileType.Attachment:
                break;
        }
        return "attachments";
    }

    internal async Task<HttpResponseMessage> InternalRequest(HttpMethod method, string endpoint, object? request)
    {
        HttpRequestMessage Mes = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            if (Client.Config.Debug.LogRestRequestJson)
                Client.Logger.LogJson("Rest Request", request);
        }

        HttpResponseMessage Req = await Http.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest Retry = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                {
                    Retry = DeserializeJson<RetryRequest>(Stream);
                }
            }

            if (Retry != null)
            {
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
                if (request != null)
                    MesRetry.Content = Mes.Content;
                Req = await Http.SendAsync(MesRetry);
            }
        }

        if (Client.Config.Debug.LogRestRequest)
        {
            Client.Logger.LogRestMessage(Req, method, endpoint);

            if (Client.Config.LogMode == RevoltLogSeverity.Debug)
                Client.Logger.LogJson("Rest Send", Req);
        }


        if (method != HttpMethod.Get && !Req.IsSuccessStatusCode)
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                    {
                        Error = DeserializeJson<RestError>(Stream);
                    }
                }
                catch { }
            }
            if (Error != null)
            {
                if (string.IsNullOrEmpty(Error.Permission))
                    throw new RevoltRestException($"Request failed due to {Error.Type} ({(int)Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
                else
                    throw new RevoltPermissionException(Error.Permission, (int)Req.StatusCode, Error.Type == RevoltErrorType.MissingUserPermission);
            }
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        if (Req.IsSuccessStatusCode && Req.Content.Headers.ContentLength.HasValue && Client.Config.Debug.LogRestResponseJson)
        {
            string Content = await Req.Content.ReadAsStringAsync();
            Client.Logger.LogJson("Rest Request", Content);
        }
        return Req;
    }
    internal async Task<TResponse> InternalJsonRequest<TResponse>(HttpMethod method, string endpoint, object request, bool throwGetRequest = false)
        where TResponse : class
    {
        HttpRequestMessage Mes = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            if (Client.Config.Debug.LogRestRequestJson)
                Client.Logger.LogJson("Rest Request", request);
        }
        HttpResponseMessage Req = await Http.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest Retry = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                    {
                        Retry = DeserializeJson<RetryRequest>(Stream);
                    }
                }
                catch { }
            }

            if (Retry != null)
            {
                Client.InvokeLog($"Retrying request: {endpoint} for {Retry.retry_after}s", RevoltLogSeverity.Warn);
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
                if (request != null)
                    MesRetry.Content = Mes.Content;
                Req = await Http.SendAsync(MesRetry);
            }

        }

        if (Client.Config.Debug.LogRestRequest)
        {
            Client.Logger.LogRestMessage(Req, method, endpoint);

            if (Client.Config.LogMode == RevoltLogSeverity.Debug)
                Client.Logger.LogJson("Rest Send", Req);
        }


        if (endpoint == "/" && !Req.IsSuccessStatusCode)
        {
            Client.InvokeLog("Revolt API is down", RevoltLogSeverity.Warn);
            throw new RevoltRestException("The Revolt API is down. Please try again later.", 500, RevoltErrorType.Unknown);
        }


        if (!Req.IsSuccessStatusCode && (throwGetRequest || method != HttpMethod.Get))
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                    {
                        Error = DeserializeJson<RestError>(Stream);
                    }
                }
                catch { }
            }
            if (Client.Config.Debug.LogRestRequest)
                Client.Logger.LogRestMessage(Req, method, endpoint);

            if (Error != null)
            {
                if (string.IsNullOrEmpty(Error.Permission))
                    throw new RevoltRestException($"Request failed due to {Error.Type} ({(int)Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
                else
                    throw new RevoltPermissionException(Error.Permission, (int)Req.StatusCode, Error.Type == RevoltErrorType.MissingUserPermission);
            }
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        TResponse Response = null;
        if (Req.IsSuccessStatusCode)
        {
            string Data = await Req.Content.ReadAsStringAsync();
            try
            {
                using (Stream Stream = await Req.Content.ReadAsStreamAsync())
                {
                    Response = DeserializeJson<TResponse>(Stream);
                }
            }
            catch (Exception ex)
            {
                Client.InvokeLog($"Failed to parse json for {endpoint}", RevoltLogSeverity.Error);
                throw new RevoltRestException("Failed to parse json response: " + ex.Message, 500, RevoltErrorType.Unknown);
            }

            if (Response != null && Client.Config.Debug.LogRestResponseJson)
                Client.Logger.LogJson("Rest Response", Response);
        }
#pragma warning disable CS8603 // Possible null reference return.
        return Response;
#pragma warning restore CS8603 // Possible null reference return.
    }

    internal static string SerializeJson(object value)
    {
        StringBuilder sb = new StringBuilder(256);
        using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
        using (JsonWriter writer = new JsonTextWriter(text))
            RevoltClient.Serializer.Serialize(writer, value);
        return sb.ToString();
    }

    internal static string SerializeJsonPretty(object value)
    {
        StringBuilder sb = new StringBuilder(256);
        using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
        using (JsonWriter writer = new JsonTextWriter(text))
            RevoltClient.SerializerPretty.Serialize(writer, value);
        return sb.ToString();
    }

    internal static T? DeserializeJson<T>(Stream jsonStream)
    {
        using (TextReader text = new StreamReader(jsonStream))
        using (JsonReader reader = new JsonTextReader(text))
            return RevoltClient.Deserializer.Deserialize<T>(reader);
    }
}

/// <summary>
/// File type to upload to the Revolt instance CDN.
/// </summary>
public enum UploadFileType
{
    /// <summary>
    /// Upload a normal file e.g txt, mp4, ect.
    /// </summary>
    Attachment,
    /// <summary>
    /// Set the bot's avatar with this image.
    /// </summary>
    Avatar,
    /// <summary>
    /// Set a server or channel icon with this image.
    /// </summary>
    Icon,
    /// <summary>
    /// Set a server banner with this image.
    /// </summary>
    Banner,
    /// <summary>
    /// Set the bot's profile background with this image.
    /// </summary>
    Background,
    /// <summary>
    /// Create a server emoji with this image.
    /// </summary>
    Emoji
}

/// <summary>
/// The request method type to use for sending requests to the Revolt instance API.
/// </summary>
public enum RequestType
{
    /// <summary>
    /// Get data from the API.
    /// </summary>
    Get,
    /// <summary>
    /// Post new messages or create channels.
    /// </summary>
    Post,
    /// <summary>
    /// Delete a message, channel, ect.
    /// </summary>
    Delete,
    /// <summary>
    /// Update an existing channel, server, ect.
    /// </summary>
    Patch,
    /// <summary>
    /// Post new emojis.
    /// </summary>
    Put
}