using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DialogGen.Lib.Model;
using Newtonsoft.Json;

namespace DialogGen.Lib.Services
{
    // This class is using sample code from the Qna Maker documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/quickstarts/get-answer-from-knowledge-base-csharp
    public static class LuisService
    {
        private const string URL_BASE = "https://{region}.api.cognitive.microsoft.com/luis/v2.0/apps/"; 

        public static async Task<LuisResponse> GetLuisResultAsync(string userInput, string region, string appId, string key, float threshold)
        {
            try
            {
                string getUrl = $"{URL_BASE.Replace("{region}", region)}{appId}?q={userInput}&verbose=True";

                // Create http client
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // POST method
                    request.Method = HttpMethod.Get;

                    // Add host + service to get full URI
                    request.RequestUri = new Uri(getUrl);
                    
                    // set authorization
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    // Send request to Azure service, get response
                    var response = await client.SendAsync(request);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var luisResponse = JsonConvert.DeserializeObject<LuisResponse>(jsonResponse);

                    return luisResponse;
                    
                }
            }
            catch (System.Exception e)
            {
                throw new Exception("Error while performing REST GET call to LUIS.", e);
            }
            
        }
    }
}