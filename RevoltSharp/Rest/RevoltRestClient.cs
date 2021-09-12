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
            HttpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(HostUrl)
            };
            HttpClient.DefaultRequestHeaders.Add("x-bot-token", Client.Token);
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Revolt Bot (RevoltSharp)");
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            FileHttpClient = new HttpClient()
            {
                BaseAddress = new System.Uri(FileHostUrl)
            };
            FileHttpClient.DefaultRequestHeaders.Add("User-Agent", "Revolt Bot (RevoltSharp)");
        }

        public RevoltClient Client { get; private set; }
        internal HttpClient HttpClient;
        internal HttpClient FileHttpClient;
        internal static string HostUrl = "https://api.revolt.chat/";
        internal static string FileHostUrl = "https://autumn.revolt.chat/";

        public async Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint)
        {
            HttpMethod Method = GetMethod(method);
            return await InternalJsonRequest(Method, endpoint, null);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, Dictionary<string, object> json = null)
        {
            HttpMethod Method = GetMethod(method);
            if (json == null)
                return await InternalRequest(Method, endpoint);
            return await InternalJsonRequest(Method, endpoint, json);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(RequestType method, string endpoint, RevoltRequest json = null)
        {
            HttpMethod Method = GetMethod(method);
            if (json == null)
                return await InternalRequest(Method, endpoint);
            return await InternalJsonRequest(Method, endpoint, json);
        }


        public async Task<TResponse> SendRequestAsync<TResponse>(RequestType method, string endpoint, RevoltRequest json = null)
            where TResponse : class
        {
            HttpMethod Method = GetMethod(method);
            return await InternalJsonRequest<TResponse>(Method, endpoint, json);
        }

        internal HttpMethod GetMethod(RequestType method)
        {
            switch (method)
            {
                case RequestType.Post:
                    return HttpMethod.Post;
                case RequestType.Put:
                    return HttpMethod.Put;
                case RequestType.Delete:
                    return HttpMethod.Delete;
                case RequestType.Patch:
                    return HttpMethod.Patch;
            }
            return HttpMethod.Get;
        }

        public async Task<FileAttachment> UploadFileAsync(string path, UploadFileType type)
        {
            return await UploadFileAsync(File.ReadAllBytes(path), path.Split('.').Last(), type);
        }

        public async Task<FileAttachment> UploadFileAsync(byte[] bytes, string name, UploadFileType type)
        {
            HttpRequestMessage Mes = new HttpRequestMessage(HttpMethod.Post, GetUploadType(type));
            MultipartFormDataContent MP = new System.Net.Http.MultipartFormDataContent("file");
            var Image = new ByteArrayContent(bytes);
            MP.Add(Image, "file", name);
            Mes.Content = MP;
            HttpResponseMessage Req = await FileHttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Req, Formatting.Indented));
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
            Attachment, Avatar, Icon, Banner, Background
        }

        internal async Task<HttpResponseMessage> InternalRequest(HttpMethod method, string endpoint)
        {
            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Req, Formatting.Indented));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
            return Req;
        }
        internal async Task<HttpResponseMessage> InternalJsonRequest(HttpMethod method, string endpoint, object request)
        {
            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestContent)
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(request, Formatting.Indented));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Req, Formatting.Indented));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
            return Req;
        }
        internal async Task<TResponse> InternalJsonRequest<TResponse>(HttpMethod method, string endpoint, object request)
            where TResponse : class
        {
            HttpRequestMessage Mes = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                Mes.Content = new StringContent(SerializeJson(request), Encoding.UTF8, "application/json");
                if (Client.Config.Debug.LogRestRequestContent)
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(request, Formatting.Indented));
            }
            HttpResponseMessage Req = await HttpClient.SendAsync(Mes);
            if (Client.Config.Debug.LogRestRequest)
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Req, Formatting.Indented));
            if (Client.Config.Debug.CheckRestRequest)
                Req.EnsureSuccessStatusCode();
            return Req.IsSuccessStatusCode ? DeserializeJson<TResponse>(Req.Content.ReadAsStream()) : null;
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
        Get, Post, Put, Delete, Patch
    }
}
