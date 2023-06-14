using Microsoft.IO;
using Newtonsoft.Json;
using RevoltSharp.Core;
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
            throw new RevoltException("Client config API_URL can not be empty.");

        if (!Uri.IsWellFormedUriString(client.Config.ApiUrl, UriKind.Absolute))
            throw new RevoltException("Client config API_URL is an invalid format.");

        if (!Client.Config.ApiUrl.EndsWith('/'))
            Client.Config.ApiUrl = Client.Config.ApiUrl + "/";

        if (string.IsNullOrEmpty(Client.Config.Debug.UploadUrl))
            throw new RevoltException("Client config UPLOAD_URL can not be empty.");

        if (!Uri.IsWellFormedUriString(client.Config.Debug.UploadUrl, UriKind.Absolute))
            throw new RevoltException("Client config UPLOAD_URL is an invalid format.");

        if (!Client.Config.Debug.UploadUrl.EndsWith('/'))
            Client.Config.Debug.UploadUrl = Client.Config.Debug.UploadUrl + "/";

        HttpClient = new HttpClient()
        {
            BaseAddress = new System.Uri(Client.Config.ApiUrl)
        };
        HttpClient.DefaultRequestHeaders.Add(Client.Config.UserBot ? "x-session-token" : "x-bot-token", Client.Token);
        HttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent);
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        FileHttpClient = new HttpClient()
        {
            BaseAddress = new System.Uri(Client.Config.Debug.UploadUrl)
        };
        FileHttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent);

        if (!string.IsNullOrEmpty(Client.Config.CfClearance))
        {
            string cookie = $"cf_clearance={Client.Config.CfClearance}";
            HttpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            FileHttpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    internal RevoltClient Client { get; private set; }
    internal HttpClient HttpClient;
    internal HttpClient FileHttpClient;

    private static readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

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
    public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, IRevoltRequest json = null)
    => json == null ? InternalRequest(GetMethod(method), endpoint, null) : InternalRequest(GetMethod(method), endpoint, json);

    internal Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, object> json) where TResponse : class
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

    internal Task<TResponse> PutAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Put, endpoint, json);

    internal Task<TResponse> PostAsync<TResponse>(string endpoint, IRevoltRequest json = null) where TResponse : class
        => SendRequestAsync<TResponse>(RequestType.Post, endpoint, json);

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
        }
        return HttpMethod.Get;
    }

    /// <inheritdoc cref="UploadFileAsync(byte[], string, UploadFileType)" />
    public Task<FileAttachment> UploadFileAsync(string path, UploadFileType type)
        => UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);


    /// <summary>
    /// Upload a file to the Revolt instance CDN that can be used for attachments, avatars, banners, ect.
    /// </summary>
    /// <returns>Created <see cref="FileAttachment"/></returns>
    /// <exception cref="RevoltRestException"></exception>
    public async Task<FileAttachment> UploadFileAsync(byte[] bytes, string name, UploadFileType type)
    {
        Conditions.FileBytesEmpty(bytes, "SendFileAsync");
        Conditions.FileNameEmpty(name, "SendFileAsync");

        HttpRequestMessage Mes = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
        MultipartFormDataContent MP = new System.Net.Http.MultipartFormDataContent("file");
        ByteArrayContent Image = new ByteArrayContent(bytes);
        MP.Add(Image, "file", name);
        Mes.Content = MP;
        HttpResponseMessage Req = await FileHttpClient.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest Retry = null;
            int BufferSizeRet = (int)Req.Content.Headers.ContentLength.GetValueOrDefault();
            using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSizeRet))
            {
                await Req.Content.CopyToAsync(Stream);
                Stream.Position = 0;
                Retry = DeserializeJson<RetryRequest>(Stream);
            }

            if (Retry != null)
            {
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type))
                {
                    Content = MP
                };
                Req = await HttpClient.SendAsync(MesRetry);
            }
        }

        if (Client.Config.Debug.LogRestRequest)
            Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented, Client.SerializerSettings));

        if (!Req.IsSuccessStatusCode)
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    int ErrorBufferSize = (int)Req.Content.Headers.ContentLength.Value;
                    using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", ErrorBufferSize))
                    {
                        await Req.Content.CopyToAsync(Stream);
                        Stream.Position = 0;
                        Error = DeserializeJson<RestError>(Stream);
                    }

                }
                catch { }
            }
            if (Error != null)
            {
                throw new RevoltRestException($"Request failed due to {Error.Type}", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
            }
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        int BufferSize = (int)Req.Content.Headers.ContentLength.GetValueOrDefault();
        using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
        {
            await Req.Content.CopyToAsync(Stream);
            Stream.Position = 0;
            return new FileAttachment(Client, DeserializeJson<FileAttachmentJson>(Stream).id);
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
        }
        return "attachments";
    }

    internal async Task<HttpResponseMessage> InternalRequest(HttpMethod method, string endpoint, object request)
    {
        if (Client.UserBot && method == HttpMethod.Post && (endpoint.StartsWith("/invites/", StringComparison.OrdinalIgnoreCase) || endpoint.StartsWith("invites/", StringComparison.OrdinalIgnoreCase)))
            throw new RevoltRestException("Joining servers with a userbot has been blocked.", 400, RevoltErrorType.NotAllowedForUsers);

        HttpRequestMessage Mes = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            if (Client.Config.Debug.LogRestRequestJson)
                Console.WriteLine("--- Rest Request Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented, Client.SerializerSettings));
        }
        HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest Retry = null;
            int BufferSize = (int)Req.Content.Headers.ContentLength.GetValueOrDefault();
            using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
            {
                await Req.Content.CopyToAsync(Stream);
                Stream.Position = 0;
                Retry = DeserializeJson<RetryRequest>(Stream);
            }
            if (Retry != null)
            {
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
                if (request != null)
                    MesRetry.Content = Mes.Content;
                Req = await HttpClient.SendAsync(MesRetry);
            }
        }

        if (Client.Config.Debug.LogRestRequest)
            Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented, Client.SerializerSettings));



        if (method != HttpMethod.Get && !Req.IsSuccessStatusCode)
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
                    using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                    {
                        await Req.Content.CopyToAsync(Stream);
                        Stream.Position = 0;
                        Error = DeserializeJson<RestError>(Stream);
                    }

                }
                catch { }
            }
            if (Error != null)
            {
                throw new RevoltRestException($"Request failed due to {Error.Type}", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
            }
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        if (Req.IsSuccessStatusCode && Client.Config.Debug.LogRestResponseJson)
        {
            string Content = await Req.Content.ReadAsStringAsync();
            Console.WriteLine("--- Rest Response ---\n" + Content);
        }
        return Req;
    }
    internal async Task<TResponse> InternalJsonRequest<TResponse>(HttpMethod method, string endpoint, object request, bool throwGetRequest = false)
        where TResponse : class
    {
        if (Client.UserBot && method == HttpMethod.Post && (endpoint.StartsWith("/invites/", StringComparison.OrdinalIgnoreCase) || endpoint.StartsWith("invites/", StringComparison.OrdinalIgnoreCase)))
            throw new RevoltRestException("Joining servers with a user account has been blocked.", 400, RevoltErrorType.NotAllowedForUsers);



        HttpRequestMessage Mes = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
        if (request != null)
        {
            Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
            if (Client.Config.Debug.LogRestRequestJson)
                Console.WriteLine("--- Rest REQ Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented, Client.SerializerSettings));
        }
        HttpResponseMessage Req = await HttpClient.SendAsync(Mes);

        if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            RetryRequest Retry = null;

            int BufferSize = (int)Req.Content.Headers.ContentLength.GetValueOrDefault();
            using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
            {
                await Req.Content.CopyToAsync(Stream);
                Stream.Position = 0;
                Retry = DeserializeJson<RetryRequest>(Stream);
            }

            if (Retry != null)
            {
                await Task.Delay(Retry.retry_after + 2);
                HttpRequestMessage MesRetry = new HttpRequestMessage(method, Client.Config.ApiUrl + endpoint);
                if (request != null)
                    MesRetry.Content = Mes.Content;
                Req = await HttpClient.SendAsync(MesRetry);
            }
        }

        if (Client.Config.Debug.LogRestRequest)
            Console.WriteLine(JsonConvert.SerializeObject("--- Rest Request ---\n" + Req, Formatting.Indented, Client.SerializerSettings));


        if (endpoint == "/" && !Req.IsSuccessStatusCode)
            throw new RevoltRestException("The Revolt API is down. Please try again later.", 500, RevoltErrorType.Unknown);



        if (!Req.IsSuccessStatusCode && (throwGetRequest || method != HttpMethod.Get))
        {
            RestError Error = null;
            if (Req.Content.Headers.ContentLength.HasValue)
            {
                try
                {
                    int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
                    using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                    {
                        await Req.Content.CopyToAsync(Stream);
                        Stream.Position = 0;
                        Error = DeserializeJson<RestError>(Stream);
                    }
                }
                catch { }
            }
            if (Error != null)
                throw new RevoltRestException($"Request failed due to {Error.Type} ({Req.StatusCode})", (int)Req.StatusCode, Error.Type) { Permission = Error.Permission };
            else
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode, RevoltErrorType.Unknown);
        }

        TResponse Response = null;
        if (Req.IsSuccessStatusCode)
        {
            int BufferSize = (int)Req.Content.Headers.ContentLength.GetValueOrDefault();
            try
            {
                using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                {
                    await Req.Content.CopyToAsync(Stream);
                    Stream.Position = 0;
                    Response = DeserializeJson<TResponse>(Stream);
                }
            }
            catch (Exception ex)
            {
                throw new RevoltRestException("Failed to parse json response: " + ex.Message, 500, RevoltErrorType.Unknown);
            }
            if (Response != null && Client.Config.Debug.LogRestResponseJson)
                Console.WriteLine("--- Rest RS Json ---\n" + JsonConvert.SerializeObject(Response, Formatting.Indented, Client.SerializerSettings));
        }
#pragma warning disable CS8603 // Possible null reference return.
        return Response;
#pragma warning restore CS8603 // Possible null reference return.
    }

    internal string SerializeJson(object value)
    {
        StringBuilder sb = new StringBuilder(256);
        using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
        using (JsonWriter writer = new JsonTextWriter(text))
            Client.Serializer.Serialize(writer, value);
        return sb.ToString();
    }

    internal T? DeserializeJson<T>(MemoryStream jsonStream)
    {
        using (TextReader text = new StreamReader(jsonStream))
        using (JsonReader reader = new JsonTextReader(text))
            return Client.Deserializer.Deserialize<T>(reader);
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
    Background
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
