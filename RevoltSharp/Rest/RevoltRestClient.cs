using Newtonsoft.Json;
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
            HttpClient.DefaultRequestHeaders.Add("x-bot-token", Client.Token);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent + " v" + Client.Version);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            FileHttpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(Client.Config.Debug.UploadUrl)
            };
            FileHttpClient.DefaultRequestHeaders.Add("User-Agent", Client.Config.UserAgent);
        }

        public RevoltClient Client { get; private set; }
        internal HttpClient HttpClient;
        internal HttpClient FileHttpClient;

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
            }
            return HttpMethod.Get;
        }

        public Task<FileAttachment> UploadFileAsync(string path, UploadFileType type)
            => InternalUploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);


        public Task<FileAttachment> UploadFileAsync(byte[] bytes, string name, UploadFileType type)
           => InternalUploadFileAsync(bytes, name, type);

        internal async Task<FileAttachment> InternalUploadFileAsync(byte[] bytes, string name, UploadFileType type)
        {
            HttpRequestMessage Mes = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
            MultipartFormDataContent MP = new System.Net.Http.MultipartFormDataContent("file");
            ByteArrayContent Image = new ByteArrayContent(bytes);
            MP.Add(Image, "file", name);
            Mes.Content = MP;
            HttpResponseMessage Req = await FileHttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
            return Req.IsSuccessStatusCode ? new FileAttachment { Id = DeserializeJson<FileAttachmentJson>(Req.Content.ReadAsStream()).id } : null;
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
            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestJson)
                    Console.WriteLine("--- Rest Request Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine("--- Rest Request ---\n" + JsonConvert.SerializeObject(Req, Formatting.Indented));

            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
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
            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestJson)
                    Console.WriteLine("--- Rest Request Json ---\n" + JsonConvert.SerializeObject(request, Formatting.Indented));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(JsonConvert.SerializeObject("--- Rest Request ---\n" + Req, Formatting.Indented));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
            TResponse Response = null;
            if (Req.IsSuccessStatusCode)
            {
                Response = DeserializeJson<TResponse>(Req.Content.ReadAsStream());
                if (Client.Config.Debug.LogRestResponseJson)
                    Console.WriteLine("--- Rest Response Json ---\n" + JsonConvert.SerializeObject(Response, Formatting.Indented));
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

        internal T DeserializeJson<T>(Stream jsonStream)
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
        Patch
    }
}
