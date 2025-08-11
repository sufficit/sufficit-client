using Sufficit.Net.Http;
using Sufficit.Statistics;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public class StatisticsControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/statistics";

        public StatisticsControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        /// <summary>
        /// Remove a specific metric
        /// </summary>
        /// <param name="category">Metric category</param>
        /// <param name="period">Aggregation period</param>
        /// <param name="metricName">Metric name</param>
        /// <param name="subtype">Subtype</param>
        /// <param name="contextId">Context ID</param>
        /// <param name="timestamp">Timestamp of the metric to remove</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public async Task<int> RemoveMetricAsync(
            string category,
            string period,
            string metricName,
            string subtype,
            Guid contextId,
            DateTime timestamp,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/{category}/{period}/{metricName}/{subtype}/{contextId}/{timestamp:O}";
            var message = new HttpRequestMessage(HttpMethod.Delete, requestEndpoint);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Search metrics with flexible filtering
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of metrics matching the criteria</returns>
        public async Task<IEnumerable<Metric>> SearchAsync(
            MetricsSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/search";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            message.Content = JsonContent.Create(parameters);
            
            return await RequestMany<Metric>(message, cancellationToken);
        }

        /// <summary>
        /// Search metrics with streaming response for large datasets
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Streaming response of metrics</returns>
        public IAsyncEnumerable<Metric> SearchStreamAsync(MetricsSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/search/stream";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            message.Content = JsonContent.Create(parameters);
            
            return RequestManyAsAsyncEnumerable<Metric>(message, cancellationToken);
        }

        /// <summary>
        /// Count metrics matching search criteria
        /// </summary>
        /// <param name="parameters">Search parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of matching metrics</returns>
        public async Task<int> CountAsync(
            MetricsSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/count";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            message.Content = JsonContent.Create(parameters);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Create or update a metric
        /// </summary>
        /// <param name="metric">Metric to upsert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public async Task<int> UpsertMetricAsync(
            Metric metric,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            message.Content = JsonContent.Create(metric);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Bulk insert metrics for better performance
        /// </summary>
        /// <param name="metrics">List of metrics to insert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of affected rows</returns>
        public async Task<int> BulkInsertMetricsAsync(
            IEnumerable<Metric> metrics,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/bulk";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            message.Content = JsonContent.Create(metrics);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Clean up expired metrics
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of cleaned up metrics</returns>
        public async Task<int> CleanupExpiredMetricsAsync(
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/cleanup";
            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Get total metrics count
        /// </summary>
        /// <param name="category">Optional category filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Total number of metrics</returns>
        public async Task<int> GetMetricsCountAsync(
            string? category = null,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/count";
            if (!string.IsNullOrEmpty(category))
            {
                requestEndpoint += $"?category={Uri.EscapeDataString(category)}";
            }
            
            var message = new HttpRequestMessage(HttpMethod.Get, requestEndpoint);
            
            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);
            
            return await response.Content.ReadFromJsonAsync<int>(cancellationToken);
        }

        /// <summary>
        /// Get distinct categories
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of distinct categories</returns>
        public async Task<IEnumerable<string>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/categories";
            var message = new HttpRequestMessage(HttpMethod.Get, requestEndpoint);
            
            return await RequestManyStruct<string>(message, cancellationToken);
        }

        /// <summary>
        /// Get distinct subtypes for a category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of distinct subtypes</returns>
        public async Task<IEnumerable<string>> GetSubtypesAsync(
            string category,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/{Uri.EscapeDataString(category)}/subtypes";
            var message = new HttpRequestMessage(HttpMethod.Get, requestEndpoint);
            
            return await RequestManyStruct<string>(message, cancellationToken);
        }

        /// <summary>
        /// Get distinct metric names for a category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of distinct metric names</returns>
        public async Task<IEnumerable<string>> GetMetricNamesAsync(
            string category,
            CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/{Uri.EscapeDataString(category)}/metrics";
            var message = new HttpRequestMessage(HttpMethod.Get, requestEndpoint);
            
            return await RequestManyStruct<string>(message, cancellationToken);
        }
    }
}
