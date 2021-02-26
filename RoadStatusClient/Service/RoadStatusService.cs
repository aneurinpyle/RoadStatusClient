using Newtonsoft.Json;
using RoadStatusClient.Common;
using RoadStatusClient.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RoadStatusClient.Service
{
    public class RoadStatusService : IRoadStatusService
    {
        private readonly ApplicationConfiguration _appConfig;
        private readonly IApiService _tflRoadApiService;

        public RoadStatusService(IApiService tflRoadApiService, ApplicationConfiguration appConfig)
        {
            _appConfig = appConfig;
            _tflRoadApiService = tflRoadApiService;
        }

        public async Task<IEnumerable<RoadStatusDto>> GetRoadStatusFromApiAsync(string roadIds)
        {
            var urltouse = $"{_appConfig.TflRoadApiUrl}/{roadIds}";
            var queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, _appConfig.TflApiKey},
                };

            var response = await _tflRoadApiService.GetAsync(urltouse, queryParams);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roadStatusList = JsonConvert.DeserializeObject<IList<RoadStatusDto>>(content);
                return roadStatusList;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ArgumentException($"Error: '{roadIds}' is not a valid road or list of roads. Please check the full list of supported road identifiers and try again.");
            if (response.StatusCode == HttpStatusCode.TooManyRequests) // The API appears to return 'TooManyRequests' when the AppKey is invalid. Should return 401.
                throw new ArgumentException("Error: Too many API requests and/or App Key supplied is invalid and may have expired. Please check the status of your App Key on the TfL API Developer Portal.");
            else
            {
                throw new Exception($"Error: API request failed. Reason: {response.ReasonPhrase}. ErrorCode: {(int)response.StatusCode}.");
            }
            
        }

        public async Task RetreiveRoadStatus(string roadIds)
        {
            try
            {
                var roadsStatuses = await GetRoadStatusFromApiAsync(roadIds);
                foreach (var roadStatus in roadsStatuses)
                {
                    Console.WriteLine($"The status of the {roadStatus.DisplayName} is as follows:\n\tRoad Status is: {roadStatus.StatusSeverity}\n\tRoad Status Description is: {roadStatus.StatusSeverityDescription}.");
                }
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }
    }
}
