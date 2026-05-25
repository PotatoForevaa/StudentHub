using System.Net;
using StudentHub.Infrastructure.Services;
using Xunit;

namespace StudentHub.Tests
{
    public class ToxicFilterServiceTests
    {
        [Fact]
        public async Task GetTaskResultAsync_WithPendingResponse_ReturnsPending()
        {
            var service = CreateService("""{"status":"pending"}""");

            var result = await service.GetTaskResultAsync("task-1");

            Assert.True(result.IsSuccess);
            Assert.Equal("pending", result.Value!.Status);
            Assert.False(result.Value.IsToxic);
        }

        [Fact]
        public async Task GetTaskResultAsync_WithFailureResponse_ReturnsFailureStatusAndError()
        {
            var service = CreateService("""{"status":"failure","error":"model unavailable"}""");

            var result = await service.GetTaskResultAsync("task-1");

            Assert.True(result.IsSuccess);
            Assert.Equal("failure", result.Value!.Status);
            Assert.Equal("model unavailable", result.Value.ErrorMessage);
        }

        [Fact]
        public async Task GetTaskResultAsync_WithNestedCeleryResult_ParsesModelResult()
        {
            var service = CreateService("""
            {
              "status": "success",
              "result": {
                "comment_id": "comment-1",
                "result": {
                  "prediction": "toxic",
                  "toxic_probability": 0.97
                }
              }
            }
            """);

            var result = await service.GetTaskResultAsync("task-1");

            Assert.True(result.IsSuccess);
            Assert.Equal("success", result.Value!.Status);
            Assert.True(result.Value.IsToxic);
            Assert.Equal("toxic", result.Value.Prediction);
            Assert.Equal(0.97f, result.Value.ToxicProbability, precision: 2);
        }

        [Fact]
        public async Task GetTaskResultAsync_WithDirectResult_StillParsesModelResult()
        {
            var service = CreateService("""
            {
              "status": "success",
              "result": {
                "prediction": "not toxic",
                "toxic_probability": 0.04
              }
            }
            """);

            var result = await service.GetTaskResultAsync("task-1");

            Assert.True(result.IsSuccess);
            Assert.Equal("success", result.Value!.Status);
            Assert.False(result.Value.IsToxic);
            Assert.Equal("not toxic", result.Value.Prediction);
            Assert.Equal(0.04f, result.Value.ToxicProbability, precision: 2);
        }

        private static ToxicFilterService CreateService(string responseBody)
        {
            var httpClient = new HttpClient(new StubHttpMessageHandler(responseBody))
            {
                BaseAddress = new Uri("http://toxic-filter.test/")
            };

            return new ToxicFilterService(httpClient);
        }

        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseBody;

            public StubHttpMessageHandler(string responseBody)
            {
                _responseBody = responseBody;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseBody)
                };

                return Task.FromResult(response);
            }
        }
    }
}
