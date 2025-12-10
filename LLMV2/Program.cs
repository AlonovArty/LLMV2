using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LLMV2.Models.Responce;
using MongoDB.Bson.IO;

namespace LLMV2
{
    public class Program
    {

        static string ClientId = "019b0341-e893-71fe-983a-cf99db3031f1";
        static string AuthorizationKey = "MDE5YjAzNDEtZTg5My03MWZlLTk4M2EtY2Y5OWRiMzAzMWYxOjYwNzRhYTdhLTcwOTctNDBmMy04ZjVkLTBkZDIwN2YxZGM3Yw==";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }



        public static async Task<string> GetToken(string rqUID, string bearer)
        {
            string ReturnToken = null;
            string Url = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";

            using (HttpClientHandler Handler = new HttpClientHandler())
            {
                Handler.ServerCertificateCustomValidationCallback = (message, cert, chain, SslPolicyErrors) => true;
                using (HttpClient Client = new HttpClient(Handler))
                {
                    HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Post, Url);
                    Request.Headers.Add("Accept", "application/json");
                    Request.Headers.Add("RqUID", rqUID);
                    Request.Headers.Add("Authorization", $"Bearer {bearer}");

                    var Data = new List<KeyValuePair<string, string>>
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
    }
}
