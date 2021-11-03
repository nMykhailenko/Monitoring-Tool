using OneOf;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonitoringTool.Application.Models.ResponseModels.Errors;

namespace MonitoringTool.Application.Interfaces.Services.Communication.Http
{
    public interface IHttpCommunicationService
    {
        /// <summary>
        /// Send HTTP request.
        /// </summary>
        /// <param name="url">URL of request.</param>
        /// <param name="request">Request model.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns>Response model.</returns>
        Task<OneOf<TResponse, ErrorResponse>> SendAsync<TRequest, TResponse>(
            string url,
            TRequest request, 
            HttpMethod method,
            CancellationToken cancellationToken);
        
        /// <summary>
        /// Send HTTP request.
        /// </summary>
        /// <param name="url">URL of request.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <typeparam name="TResponse"></typeparam>
        Task<OneOf<TResponse, ErrorResponse>> SendAsync<TResponse>(
            string url,
            HttpMethod method,
            CancellationToken cancellationToken);
    }
}