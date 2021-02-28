using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoadStatusClient.Service
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsync(string targetUrl, Dictionary<string, string> queryParams);
    }
}
