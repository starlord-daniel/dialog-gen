using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using Newtonsoft.Json;

namespace DialogGen.Lib.Services
{
    public static class AzureSearchService
    {
        public static async Task<string> GetAzureSearchAnswersAsync(string question, string hostUrl, string endpointKey)
        {
            var searchQuery = hostUrl + "&search=";

            var uri = searchQuery + HttpUtility.UrlEncode(question);

            String rawSearchResult = await CallAzureSearchAsync(uri, endpointKey);

            return rawSearchResult;
        }

        private static async Task<string> CallAzureSearchAsync(string uri, string endpointKey)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("api-key", endpointKey);

                var response = await client.SendAsync(request);
                var responseObject = await response.Content.ReadAsStringAsync();

                return responseObject;
            }
        }
    } 
}