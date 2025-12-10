using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GigaChatv2.Models.Response;
using GigaChatv2.Models;
using System.IO;

namespace GigaChatv2.Service
{
    public static class GigaChatService
    {
        public static string ClientId = "019b0341-e893-71fe-983a-cf99db3031f1";
        public static string AuthorizationKey = "MDE5YjAzNDEtZTg5My03MWZlLTk4M2EtY2Y5OWRiMzAzMWYxOjYwNzRhYTdhLTcwOTctNDBmMy04ZjVkLTBkZDIwN2YxZGM3Yw==";

        public static List<Request.Message> DialogHistory =
            new List<Request.Message>()
            {
                //new Request.Message
                //{
                //    role = "system",
                //    content = "Ты — Василий Кандинский"
                //}
            };

        public static async Task<string> GetToken()
        {
            return await GetToken(ClientId, AuthorizationKey);
        }

        public static async Task<string> GetToken(string rqUID, string bearer)
        {
            string ReturnToken = null;
            string Url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

            using (HttpClientHandler Handler = new HttpClientHandler())
            {
                Handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true;

                using (HttpClient Client = new HttpClient(Handler))
                {
                    HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);
                    Request.Headers.Add("Accept", "application/json");
                    Request.Headers.Add("RqUID", rqUID);
                    Request.Headers.Add("Authorization", $"Bearer {bearer}");

                    var Data = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("scope","GIGACHAT_API_PERS")
                    };

                    Request.Content = new FormUrlEncodedContent(Data);

                    HttpResponseMessage Responce = await Client.SendAsync(Request);

                    if (Responce.IsSuccessStatusCode)
                    {
                        string ResponseContent = await Responce.Content.ReadAsStringAsync();
                        ResponseToken Token = JsonConvert.DeserializeObject<ResponseToken>(ResponseContent);
                        ReturnToken = Token.access_token;
                    }
                }
            }

            return ReturnToken;
        }
        public static async Task<byte[]> DownloadImage(string token, string fileId)
        {
            string url = $"https://gigachat.devices.sberbank.ru/api/v1/files/{fileId}/content";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback =
                    (msg, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
        public static async Task<ResponseMessage> GetAnswer(string token, List<Request.Message> messages)
        {
            ResponseMessage responseMessage = null;

            string Url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

            using (HttpClientHandler Handler = new HttpClientHandler())
            {
                Handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true;

                using (HttpClient Client = new HttpClient(Handler))
                {
                    HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);

                    Request.Headers.Add("Accept", "application/json");
                    Request.Headers.Add("Authorization", $"Bearer {token}");

                    Models.Request DataRequest = new Models.Request()
                    {
                        model = "GigaChat:2.0.28.2",
                        messages = messages,
                        function_call = "auto",
                        temperature = 0.3,
                        max_tokens = 1500
                    };

                    string JsonContent = JsonConvert.SerializeObject(DataRequest);
                    Request.Content = new StringContent(JsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage Responce = await Client.SendAsync(Request);

                    if (Responce.IsSuccessStatusCode)
                    {
                        string ResponseContent = await Responce.Content.ReadAsStringAsync();
                        responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(ResponseContent);
                    }
                }
            }

            return responseMessage;
        }

        public static string ExtractImageId(string content)
        {
            var start = content.IndexOf("src=\"") + 5;
            var end = content.IndexOf("\"", start);

            if (start < 5 || end < 0) return null;

            return content.Substring(start, end - start);
        }

        public static string SaveImage(byte[] data)
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "gigachat_wallpaper.jpg"
            );

            File.WriteAllBytes(filePath, data);
            return filePath;
        }

   
    }
}
