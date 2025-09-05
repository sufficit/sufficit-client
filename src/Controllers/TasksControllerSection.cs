using Microsoft.AspNetCore.Authorization;
using Sufficit.EndPoints;
using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Tasks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    /// <summary>
    /// Client controller section for managing scheduled tasks and background worker operations
    /// </summary>
    public sealed class TasksControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/tasks";

        private readonly JsonSerializerOptions _json;

        public TasksControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Executes a task synchronously without schedule, forcing immediate execution
        /// </summary>
        /// <param name="id">The unique identifier of the task to execute</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Response containing execution results</returns>
        [Authorize(Roles = ManagerRole.NormalizedName)]
        public Task<EndPointResponse?> ExecuteTask(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/execute?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EndPointResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Executes a task asynchronously in background without schedule, returns immediately
        /// </summary>
        /// <param name="id">The unique identifier of the task to execute</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Response confirming execution started</returns>
        [Authorize(Roles = ManagerRole.NormalizedName)]
        public Task<EndPointResponse?> ExecuteTaskBackground(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/execute?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request<EndPointResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Creates a new scheduled task or updates an existing one
        /// </summary>
        /// <param name="task">The schedule task configuration to create or update</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>The ID of the created or updated task</returns>
        [Authorize(Roles = ManagerRole.NormalizedName)]
        public async Task<Guid?> CreateOrUpdateTask(ScheduleTask task, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(task, null, _json);

            var response = await Request<EndPointResponse>(message, cancellationToken);

            if (response?.Data is JsonElement json && json.ValueKind == JsonValueKind.String)
            {
                var guidString = json.GetString();
                if (Guid.TryParse(guidString, out var parsedGuid))
                    return parsedGuid;
            }
            return null;
        }

        /// <summary>
        /// Removes a scheduled task from the background worker
        /// </summary>
        /// <param name="id">The unique identifier of the task to remove</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>The ID of the removed task</returns>
        [Authorize(Roles = ManagerRole.NormalizedName)]
        public async Task<Guid?> DeleteTask(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);

            var response = await Request<EndPointResponse>(message, cancellationToken);

            if (response?.Data is JsonElement json && json.ValueKind == JsonValueKind.String)
            {
                var guidString = json.GetString();
                if (Guid.TryParse(guidString, out var parsedGuid))
                    return parsedGuid;
            }
            return null;
        }

        /// <summary>
        /// Retrieves all scheduled tasks from the system
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Collection of all scheduled tasks</returns>
        [Authorize(Roles = ManagerRole.NormalizedName)]
        public Task<IEnumerable<ScheduleTask>> GetAllTasks(CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ScheduleTask>(message, cancellationToken);
        }
    }
}
