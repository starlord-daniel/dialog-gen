using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using Newtonsoft.Json;

namespace DialogGen.Lib.Services
{
    // This class is using sample code from the Qna Maker documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/get-answer-from-knowledge-base-csharp
    public static class QnaMakerService
    {
        public static async Task<QnaResponse> GetQnaResultAsync(string userInput, string host, string route, string endpointKey)
        {
            var question = $"{{ \"question\": \"{userInput}\",\"top\": 1 }}";

            // Create http client
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // POST method
                request.Method = HttpMethod.Post;

                // Add host + service to get full URI
                request.RequestUri = new Uri(host + route);

                // set question
                request.Content = new StringContent(question, Encoding.UTF8, "application/json");
                
                // set authorization
                request.Headers.Add("Authorization", "EndpointKey " + endpointKey);

                // Send request to Azure service, get response
                var response = await client.SendAsync(request);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var qnaResponse = JsonConvert.DeserializeObject<QnaResponse>(jsonResponse);

                return qnaResponse;
                
            }
        }
    }
}