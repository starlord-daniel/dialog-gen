using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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

        public static async Task<string> GetAzureSearchFilterAnswersAsync(string filterInputs, string[] parameterInputs, 
            Dictionary<string, string> stateValues, string hostUrl, string endpointKey)
        {
            try
            {
                var searchQuery = hostUrl + "&search=*";

                // Build the filter query. Should start with &$filter
                var filterQuery = BuildFilterQuery(filterInputs, parameterInputs, stateValues);

                var uri = hostUrl + filterQuery;

                String rawFilterResult = await CallAzureSearchAsync(uri, endpointKey);

                return rawFilterResult;
            }
            catch (System.Exception e)
            {
                throw new Exception("[AzureSearchService] Error while sending the AzureSearchFilterRequest",e);
            }
            
        }

        private static async Task<string> CallAzureSearchAsync(string uri, string endpointKey)
        {
            try
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
            catch (System.Exception e)
            {
                throw new Exception($"[AzureSearchService] Error while performing the Azure Search call with the provided url: {uri}", e);
            }
            
        }

        private static string BuildFilterQuery(string filterInputs, string[] parameterInputs, Dictionary<string, string> stateValues)
        {
            try 
            {
                var replacedFilterValueList = new List<string>();

                // The list of all filter expressions, that should be applied
                var filterExpressionList = filterInputs.Split(',').ToList();

                // The list of all values in the filter expression, that need to be replaced
                var filterValueList = GetListOfOccurences(filterInputs);

                foreach (var filterValue in filterValueList)
                {
                    // get the state variable value with the specified name
                    var stateValue = stateValues[filterValue];

                    // if the value in the stateValues is valid, add the new value
                    if( !String.IsNullOrWhiteSpace(stateValue) && !parameterInputs.Contains(stateValue) )
                    {
                        // Search for the expression that contains the un-replaced value
                        var filterExpressionToExtend = filterExpressionList.Where(x => x.Contains(filterValue)).First();

                        // Replace the value in the filter expression
                        var extendedFilterExpression = filterExpressionToExtend.Replace("{" + filterValue + "}", "'" + stateValue + "'");
                        replacedFilterValueList.Add(extendedFilterExpression);
                    }
                }

                // Build the final filter Query to return
                var filterQuery = CombineFilterExpressions(replacedFilterValueList);

                return filterQuery;
            }
            catch(Exception e)
            {
                throw new Exception("[AzureSearchService] Error while trying to build the filter query.",e);
            }
            
        }

        private static string CombineFilterExpressions(List<string> expressionList)
        {
            var filterQuery = "&$filter=";
            var numOfExpressions = expressionList.Count;

            if(numOfExpressions == 1)
            {
                filterQuery += expressionList.First();
            }
            else if(numOfExpressions > 1)
            {
                for (int i = 0; i < numOfExpressions; i++)
                {
                    if(i == numOfExpressions - 1)
                    {
                        filterQuery += expressionList[i];
                    }
                    else
                    {
                        filterQuery += expressionList[i] + " and ";
                    }
                }
            }
            else
            {
                // Return empty result and don't apply any filter
                return "";
            }

            return filterQuery;
        }

        private static List<string> GetListOfOccurences(string inputString, string regexFilterPattern = @"[{]{1}(\w+)[}]{1}")
        {
            var occurenceList = new List<string>();

            RegexOptions options = RegexOptions.Singleline;
            
            foreach (Match m in Regex.Matches(inputString, regexFilterPattern, options))
            {
                occurenceList.Add(m.Value.Substring(1, m.Value.Length - 2) );
            }

            return occurenceList;
        }
    } 
}