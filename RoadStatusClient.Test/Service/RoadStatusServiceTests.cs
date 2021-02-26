using Moq;
using NUnit.Framework;
using RoadStatusClient.Common;
using RoadStatusClient.Model;
using RoadStatusClient.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RoadStatusClient.Test.Service
{
    [TestFixture]
    public class RoadStatusServiceTests
    {
        private string _singleRoadApiResponse;
        private string _multipleRoadApiResponse;
        private ApplicationConfiguration _applicationConfig;
        private Dictionary<string, string> _queryParams;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _singleRoadApiResponse = File.ReadAllText($"./TestData/TfLSingleRoadApiMockResponse.json");
            _multipleRoadApiResponse = File.ReadAllText($"./TestData/TfLMultipleRoadsApiMockResponse.json");

            _applicationConfig = new ApplicationConfiguration { TflApiKey = "123456", TflRoadApiUrl = "https://mockTflApiUrl" };

            _queryParams = new Dictionary<string, string>
                {
                    {Constants.AppKey, "123456"},
                };
        }

        [Test]
        public async Task GetRoadStatusFromApiAsync_WhenInvokedSingleRoad_ReturnsSingleRoadResponse()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_singleRoadApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            //Act
            var result = await roadStatusService.GetRoadStatusFromApiAsync("A2");

            //Assert
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2", _queryParams), Times.Once);
            Assert.IsInstanceOf<IEnumerable<RoadStatusDto>>(result);
            Assert.AreEqual(1, result.Count());

            var singleRoad = result.First();
            Assert.Multiple(() =>
            {
                Assert.That(singleRoad.Id, Is.EqualTo("a2"));
                Assert.That(singleRoad.DisplayName, Is.EqualTo("A2"));
                Assert.That(singleRoad.Group, Is.EqualTo(null));
                Assert.That(singleRoad.StatusSeverity, Is.EqualTo("Serious"));
                Assert.That(singleRoad.StatusSeverityDescription, Is.EqualTo("Serious Delays"));
                Assert.That(singleRoad.Bounds, Is.EqualTo("[[-0.0857,51.44091],[0.17118,51.49438]]"));
                Assert.That(singleRoad.Envelope, Is.EqualTo("[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]"));
                Assert.That(singleRoad.StatusAggregationStartDate, Is.EqualTo(null));
                Assert.That(singleRoad.StatusAggregationEndDate, Is.EqualTo(null));
                Assert.That(singleRoad.Url, Is.EqualTo("/Road/a2"));
            });
        }

        [TestCase("A2,A20")]
        [TestCase("")]
        public async Task GetRoadStatusFromApiAsync_WhenInvokedMultipleRoadsOrNoRoads_ReturnsMultipleRoadsResponse(string roadString)
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_multipleRoadApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            //Act
            var result = await roadStatusService.GetRoadStatusFromApiAsync(roadString);

            //Assert
            mockApiService.Verify(x => x.GetAsync($"https://mockTflApiUrl/{roadString}", _queryParams), Times.Once);
            Assert.IsInstanceOf<IEnumerable<RoadStatusDto>>(result);
            Assert.AreEqual(2, result.Count());

            var firstRoad = result.First();
            Assert.Multiple(() =>
            {
                Assert.That(firstRoad.Id, Is.EqualTo("a2"));
                Assert.That(firstRoad.DisplayName, Is.EqualTo("A2"));
                Assert.That(firstRoad.Group, Is.EqualTo(null));
                Assert.That(firstRoad.StatusSeverity, Is.EqualTo("Serious"));
                Assert.That(firstRoad.StatusSeverityDescription, Is.EqualTo("Serious Delays"));
                Assert.That(firstRoad.Bounds, Is.EqualTo("[[-0.0857,51.44091],[0.17118,51.49438]]"));
                Assert.That(firstRoad.Envelope, Is.EqualTo("[[-0.0857,51.44091],[-0.0857,51.49438],[0.17118,51.49438],[0.17118,51.44091],[-0.0857,51.44091]]"));
                Assert.That(firstRoad.StatusAggregationStartDate, Is.EqualTo(null));
                Assert.That(firstRoad.StatusAggregationEndDate, Is.EqualTo(null));
                Assert.That(firstRoad.Url, Is.EqualTo("/Road/a2"));
            });

            var lastRoad = result.Last();
            Assert.Multiple(() =>
            {
                Assert.That(lastRoad.Id, Is.EqualTo("a20"));
                Assert.That(lastRoad.DisplayName, Is.EqualTo("A20"));
                Assert.That(lastRoad.Group, Is.EqualTo(null));
                Assert.That(lastRoad.StatusSeverity, Is.EqualTo("Serious"));
                Assert.That(lastRoad.StatusSeverityDescription, Is.EqualTo("Serious Delays"));
                Assert.That(lastRoad.Bounds, Is.EqualTo("[[-0.11925,51.40825],[0.14918,51.48643]]"));
                Assert.That(lastRoad.Envelope, Is.EqualTo("[[-0.11925,51.40825],[-0.11925,51.48643],[0.14918,51.48643],[0.14918,51.40825],[-0.11925,51.40825]]"));
                Assert.That(lastRoad.StatusAggregationStartDate, Is.EqualTo(null));
                Assert.That(lastRoad.StatusAggregationEndDate, Is.EqualTo(null));
                Assert.That(lastRoad.Url, Is.EqualTo("/Road/a20"));
            });
        }

        [Test]
        public void GetRoadStatusFromApiAsync_WhenErrorNotFound_ThrowsArgumentException()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.NotFound
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(() => roadStatusService.GetRoadStatusFromApiAsync("A2345"));

            Assert.That(ex.Message, Is.EqualTo("Error: 'A2345' is not a valid road or list of roads. Please check the full list of supported road identifiers and try again."));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2345", _queryParams), Times.Once);
        }

        [Test]
        public void GetRoadStatusFromApiAsync_WhenErrorTooManyRequests_ThrowsArgumentException()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.TooManyRequests
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(() => roadStatusService.GetRoadStatusFromApiAsync("A2"));

            Assert.That(ex.Message, Is.EqualTo("Error: Too many API requests and/or App Key supplied is invalid and may have expired. Please check the status of your App Key on the TfL API Developer Portal."));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2", _queryParams), Times.Once);
        }

        [Test]
        public void GetRoadStatusFromApiAsync_WhenInternalServerError_ThrowsException()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.InternalServerError
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            //Act
            //Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => roadStatusService.GetRoadStatusFromApiAsync("A2"));

            Assert.That(ex.Message, Is.EqualTo("Error: API request failed. Reason: Internal Server Error. ErrorCode: 500."));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2", _queryParams), Times.Once);
        }

        [Test]
        public async Task RetrieveRoadStatusAsync_WhenSingleRoadGiven_SingleRoadReturnedInConsole()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_singleRoadApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            var expectedOutput = "The status of the A2 is as follows:\n\tRoad Status is: Serious\n\tRoad Status Description is: Serious Delays.\r\n";
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            //Act
            await roadStatusService.RetreiveRoadStatus("A2");

            //Assert
            Assert.That(consoleOutput.ToString(), Is.EqualTo(expectedOutput));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2", _queryParams), Times.Once);
        }

        [Test]
        public async Task RetrieveRoadStatusAsync_WhenMultipleRoadsGiven_MultipleRoadsReturnedInConsole()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_multipleRoadApiResponse),
                StatusCode = HttpStatusCode.OK
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            var expectedOutput = "The status of the A2 is as follows:\n\tRoad Status is: Serious\n\tRoad Status Description is: Serious Delays.\r\nThe status of the A20 is as follows:\n\tRoad Status is: Serious\n\tRoad Status Description is: Serious Delays.\r\n";
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            //Act
            await roadStatusService.RetreiveRoadStatus("A2,A20");

            //Assert
            Assert.That(consoleOutput.ToString(), Is.EqualTo(expectedOutput));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A2,A20", _queryParams), Times.Once);
        }

        [Test]
        public async Task RetrieveRoadStatusAsync_WhenInvalidRoadGiven_InvalidRoadErrorReturnedInConsole()
        {
            //Arrange
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(_singleRoadApiResponse),
                StatusCode = HttpStatusCode.NotFound
            });

            var roadStatusService = new RoadStatusService(mockApiService.Object, _applicationConfig);

            var expectedOutput = "Error: 'A23456' is not a valid road or list of roads. Please check the full list of supported road identifiers and try again.\r\n";
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            //Act
            await roadStatusService.RetreiveRoadStatus("A23456");

            //Assert
            Assert.That(consoleOutput.ToString(), Is.EqualTo(expectedOutput));
            mockApiService.Verify(x => x.GetAsync("https://mockTflApiUrl/A23456", _queryParams), Times.Once);
        }
    }
}


