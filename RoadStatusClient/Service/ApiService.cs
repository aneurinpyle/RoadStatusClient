using Microsoft.AspNetCore.WebUtilities;
using RoadStatusClient.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoadStatusClient.Service
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> GetAsync(string targetUrl, Dictionary<string, string> queryParams)
        {
            var httpClient = _httpClientFactory.CreateClient(Constants.TflHttpClient);
            return await httpClient.GetAsync(QueryHelpers.AddQueryString(targetUrl, queryParams));
        }
    }
}
