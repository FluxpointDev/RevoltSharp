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

namespace RevoltSharp.Rest
{
    public class RevoltRestClient
    {
        public RevoltRestClient(RevoltClient client)
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
            HttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent + " v" + Client.Version + (Client.Config.UserBot ? " user" : ""));
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            FileHttpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(Client.Config.Debug.UploadUrl)
            };
            FileHttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent + (Client.Config.UserBot ? " user" : ""));
        }

        public RevoltClient Client { get; private set; }
        internal HttpClient HttpClient;
        internal HttpClient FileHttpClient;

        private static readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint)
            => InternalRequest(GetMethod(method), endpoint, null);

        public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, Dictionary<string, object> json = null)
           => json == null ? InternalRequest(GetMethod(method), endpoint, null) : InternalRequest(GetMethod(method), endpoint, json);
        
        public Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, RevoltRequest json = null)
        => json == null ? InternalRequest(GetMethod(method), endpoint, null) : InternalRequest(GetMethod(method), endpoint, json);

        public Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, Dictionary<string, object> json) where TResponse : class
            => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);

        public Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, RevoltRequest json = null) where TResponse : class
            => InternalJsonRequest<TResponse>(GetMethod(method), endpoint, json);
        

        internal HttpMethod GetMethod(RequestType method)
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

        public Task<FileAttachment> UploadFileAsync(string path, UploadFileType type)
            => InternalUploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);


        public Task<FileAttachment> UploadFileAsync(byte[] bytes, string name, UploadFileType type)
           => InternalUploadFileAsync(bytes, name, type);

        internal async Task<FileAttachment> InternalUploadFileAsync(byte[] bytes, string name, UploadFileType type)
        {
            if (bytes == null || bytes.Length == 0)
                throw new RevoltArgumentException("Image bytes is empty on file upload.");

            if (string.IsNullOrEmpty(name))
                throw new RevoltArgumentException("File upload name can't be empty for this request.");

            HttpRequestMessage Mes = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
            MultipartFormDataContent MP = new System.Net.Http.MultipartFormDataContent("file");
            ByteArrayContent Image = new ByteArrayContent(bytes);
            MP.Add(Image, "file", name);
            Mes.Content = MP;
            HttpResponseMessage Req = await FileHttpClient.SendAsync(Mes);

            if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                RetryRequest Retry = null;
                int BufferSizeRet = (int)Req.Content.Headers.ContentLength.Value;
                using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSizeRet))
                {
                    await Req.Content.CopyToAsync(Stream);
                    Stream.Position = 0;
                    Retry = DeserializeJson<RetryRequest>(Stream);
                }

                if (Retry != null)
                {
                    await Task.Delay(Retry.retry_after + 2);
                    HttpRequestMessage MesRetry = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
                    MesRetry.Content = MP;
                    Req = await HttpClient.SendAsync(MesRetry);
                }
            }

            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();

            if (Client.Config.RestThrowException && !Req.IsSuccessStatusCode)
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode);

            if (!Req.IsSuccessStatusCode)
                return null;

            int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
            using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
            {
                await Req.Content.CopyToAsync(Stream);
                Stream.Position = 0;
                return new FileAttachment(DeserializeJson<FileAttachmentJson>(Stream).id);
            }
        }

        internal string GetUploadType(UploadFileType type)
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

        internal async Task<HttpResponseMessage> InternalRequest(HttpMethod method, string endpoint, object request)
        {
            if (Client.UserBot && method == HttpMethod.Post && (endpoint.StartsWith("/invites/", StringComparison.OrdinalIgnoreCase) || endpoint.StartsWith("invites/", StringComparison.OrdinalIgnoreCase)))
                throw new RevoltException("Joining servers with a userbot has been blocked.");

            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestJson)
                    Console.WriteLine("--- Rest Request Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                RetryRequest Retry = null;
                int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
                using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                {
                    await Req.Content.CopyToAsync(Stream);
                    Stream.Position = 0;
                    Retry = DeserializeJson<RetryRequest>(Stream);
                }
                if (Retry != null)
                {
                    await Task.Delay(Retry.retry_after + 2);
                    HttpRequestMessage MesRetry = new HttpRequestMessage(method, endpoint);
                    if (request != null)
                        MesRetry.Content = Mes.Content;
                    Req = await HttpClient.SendAsync(MesRetry);
                }
            }

            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));

            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();

            if (method != HttpMethod.Get && Client.Config.RestThrowException && !Req.IsSuccessStatusCode)
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode);

            if (Req.IsSuccessStatusCode && Client.Config.Debug.LogRestResponseJson)
            {
                string Content = await Req.Content.ReadAsStringAsync();
                Console.WriteLine("--- Rest Response ---\n" + Content);
            }
            return Req;
        }
        internal async Task<TResponse> InternalJsonRequest<TResponse>(HttpMethod method, string endpoint, object request)
            where TResponse : class
        {
            if (Client.UserBot && method == HttpMethod.Post && (endpoint.StartsWith("/invites/", StringComparison.OrdinalIgnoreCase) || endpoint.StartsWith("invites/", StringComparison.OrdinalIgnoreCase)))
                throw new RevoltException("Joining servers with a userbot has been blocked.");

            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestJson)
                    Console.WriteLine("--- Rest REQ Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Req.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                RetryRequest Retry = null;

                int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
                using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                {
                    await Req.Content.CopyToAsync(Stream);
                    Stream.Position = 0;
                    Retry = DeserializeJson<RetryRequest>(Stream);
                }

                if (Retry != null)
                {
                    await Task.Delay(Retry.retry_after + 2);
                    HttpRequestMessage MesRetry = new HttpRequestMessage(method, endpoint);
                    if (request != null)
                        MesRetry.Content = Mes.Content;
                    Req = await HttpClient.SendAsync(MesRetry);
                }
            }

            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(JsonConvert.SerializeObject("--- Rest Request ---\n" + Req, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();

            if (method != HttpMethod.Get && Client.Config.RestThrowException && !Req.IsSuccessStatusCode)
                throw new RevoltRestException(Req.ReasonPhrase, (int)Req.StatusCode);

            TResponse Response = null;
            if (Req.IsSuccessStatusCode)
            {
                int BufferSize = (int)Req.Content.Headers.ContentLength.Value;
                using (MemoryStream Stream = recyclableMemoryStreamManager.GetStream("RevoltSharp-SendRequest", BufferSize))
                {
                    await Req.Content.CopyToAsync(Stream);
                    Stream.Position = 0;
                    Response = DeserializeJson<TResponse>(Stream);
                }
                if (Client.Config.Debug.LogRestResponseJson)
                    Console.WriteLine("--- Rest RS Json ---\n" + JsonConvert.SerializeObject(Response, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new OptionConverter() } }));
            }
            return Response;
        }

        internal string SerializeJson(object value)
        {
            StringBuilder sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                Client.Serializer.Serialize(writer, value);
            return sb.ToString();
        }

        internal T DeserializeJson<T>(MemoryStream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return Client.Serializer.Deserialize<T>(reader);
        }
    }
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
}
