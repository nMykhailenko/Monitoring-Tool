using OneOf;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Interfaces.Services.Communication.Http;
using MonitoringTool.Application.Models.ResponseModels.Errors;

namespace MonitoringTool.Infrastructure.Services.Communication.Http
{
    public class HttpCommunicationService : IHttpCommunicationService
    {
        private const string ErrorMessage = "HttpRequestFailed";
        
        private readonly HttpClient _httpClient;
        public HttpCommunicationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<OneOf<TResponse, ErrorResponse>> SendAsync<TRequest, TResponse>(
            string url,
            TRequest request, 
            HttpMethod method,
            CancellationToken cancellationToken)
        {
            var content = new StringContent(JsonSerializer.Serialize(request));
            var requestMessage = new HttpRequestMessage(method, url) { Content = content };

            return SendRequestAsync<TResponse>(requestMessage, cancellationToken);
        }

        public Task<OneOf<TResponse, ErrorResponse>> SendAsync<TResponse>(
            string url,
            HttpMethod method,
            CancellationToken cancellationToken)
        {
            var requestMessage = new HttpRequestMessage(method, url);

            return SendRequestAsync<TResponse>(requestMessage, cancellationToken);
        }

        private async Task<OneOf<TResponse, ErrorResponse>> SendRequestAsync<TResponse>(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

            var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (responseMessage.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<TResponse>(responseBody);
                return response!;
            }

            return new ErrorResponse(ErrorMessage, responseBody);
        }
    }
}