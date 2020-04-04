using Books.OtherClasses;
using Books.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Books
{
    public static class RequestsHelper
    {

        public static string Root = "";

        public static string baseUrl = $"{Root}api";
        public static string authUrl = $"{Root}token";

        public static string Bearer = null;

        public async static Task<ResponseType> MakeGetRequest<ResponseType>(string parameters, string url = null, bool hasISBN = false)
        {
            if(Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);

                var response = await client.GetAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, parameters));

                var responseString = await response.Content.ReadAsStringAsync();

                if(hasISBN)
                {
                    string pattern = @"ISBN:\d+";
                    string replacement = "ISBN";
                    Regex regex = new Regex(pattern);
                    responseString = regex.Replace(responseString, replacement);
                }

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }

        public async static Task<ResponseType> MakeDeleteRequest<ResponseType>(string parameters, string url = null)
        {
            if (Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);

                var response = await client.DeleteAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, parameters));

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }

        public async static Task<ResponseType> MakePostRequest<ResponseType>(string path, object request, string url = null, string bearer = null)
        {
            if (Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);

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
            if (Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);

                var json = JsonConvert.SerializeObject(request);

                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PutAsync(string.Format("{0}/{1}", string.IsNullOrEmpty(url) ? baseUrl : url, path), byteContent);

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
        }

        public async static Task<HttpResponseMessage> NotificationHubRegister(string platform, string channel, List<string> tags)
        {
            if (Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);

                var postUri = baseUrl + "/notifications";

                if(Settings.InstallationId == string.Empty)
                {
                    Settings.InstallationId = Guid.NewGuid().ToString();
                }

                Installation installation = new Installation
                {
                    installationId = Settings.InstallationId,
                    platform = platform,
                    pushChannel = channel,
                    tags = tags.ToArray()
                };

                var response = await httpClient.PostAsync(postUri, new StringContent(JsonConvert.SerializeObject(installation), Encoding.UTF8, "application/json"));

                return response;
            }
        }

        public async static Task<HttpResponseMessage> NotificationHubDelete()
        {
            if (Bearer == null)
            {
                Bearer = await GetAuthToken();
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Bearer);
                string installationId = Settings.InstallationId;
                var deleteUri = baseUrl + $"/notifications/{installationId}";
                var response = await httpClient.DeleteAsync(deleteUri);
                return response;
            }
        }

        private async static Task<string> GetAuthToken()
        {
            using (var client = new HttpClient())
            {
                Dictionary<string, string> postParameters = new Dictionary<string, string>();
                postParameters.Add("grant_type", "");
                postParameters.Add("username", "");
                postParameters.Add("password", "");

                var content = new FormUrlEncodedContent(postParameters);

                var response = client.PostAsync(authUrl, content).Result;

                var responseString = await response.Content.ReadAsStringAsync();

                AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);

                return authResponse.access_token;
            }
        }

        public async static Task<GoodReadsResponse.GoodreadsResponse> MakeGoodReadsRequest(string query)
        {
            using (var client = new HttpClient())
            {
                string url = @"https://www.goodreads.com/search/index.xml";
                var response = await client.GetAsync($"{url}?key=1TxIJsocjTrg4SjkDVxROA&q={query}");

                var responseString = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(GoodReadsResponse.GoodreadsResponse));
                GoodReadsResponse.GoodreadsResponse book = null;
                using (StringReader reader = new StringReader(responseString))
                {
                    book = (GoodReadsResponse.GoodreadsResponse)(serializer.Deserialize(reader));
                }

                return book;
            }
        }

        private class AuthResponse
        {
            public string access_token { get; set; }
        }
    }
}
