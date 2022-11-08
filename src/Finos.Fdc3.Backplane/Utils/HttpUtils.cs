using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Finos.Fdc3.Backplane.Utils
{
    /// <summary>
    /// Http Utils
    /// </summary>
    public class HttpUtils
    {
        /// <summary>
        /// HTTP Post 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClientFactory"></param>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsync<T>(IHttpClientFactory httpClientFactory, Uri uri, T body, TimeSpan timeOut, CancellationToken ct = default
            )
        {
            string json = JsonConvert.SerializeObject(body, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient httpClient = httpClientFactory.CreateClient("Backplane");
            using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, data, ct))
            {
                return httpResponseMessage;
            }
        }
    }
}
