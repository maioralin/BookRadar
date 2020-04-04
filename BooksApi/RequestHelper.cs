using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace BooksApi
{
    public static class RequestHelper
    {

        private static string baseUrl = "https://api.isbndb.com";

        public async static Task<ResponseType> MakeGetRequest<ResponseType>(string parameters, string url = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", "");

                string url2 = string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, parameters);

                var response = await client.GetAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, parameters));

                var responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonConvert.DeserializeObject<ResponseType>(responseString);
                }
                catch
                {
                    return default(ResponseType);
                }
            }
        }

        public async static Task<ResponseType> MakeDeleteRequest<ResponseType>(string parameters, string url = null)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, parameters));

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }

        public async static Task<ResponseType> MakePostRequest<ResponseType>(string path, object request, string url = null)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(request);

                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, path), byteContent);

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }

        public async static Task<ResponseType> MakePutRequest<ResponseType>(string path, object request, string url = null)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(request);

                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PutAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, path), byteContent);

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }
    }
}