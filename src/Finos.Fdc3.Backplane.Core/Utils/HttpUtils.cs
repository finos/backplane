using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Finos.Fdc3.Backplane.Core.Utils
{
    public class HttpUtils
    {
        public static async Task<HttpResponseMessage> PostAsync<T>(IHttpClientFactory httpClientFactory, Uri uri, T body)
        {
            string json = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient httpClient = httpClientFactory.CreateClient();
            using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, data))
            {
                return httpResponseMessage;
            }
        }
    }
}
