using Sufficit.EndPoints;
using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Storage;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Storage
{
    public sealed class StorageControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/storage";
        private readonly IAuthenticatedControllerBase _cb;

        public StorageControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
            _cb = cb;
        }

        public Task<StorageObjectRecord?> ById (Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            string requestEndpoint = $"{Controller}/ById?{query}";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            return Request<StorageObjectRecord>(message, cancellationToken);
        }

        public async Task<IEnumerable<StorageObjectRecord>> Search(StorageObjectMetadataSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/Search";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _cb.Json);

            return await RequestMany<StorageObjectRecord>(message, cancellationToken);
        }

        public Task AddOrUpdate (StorageObjectRecord item, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/Record";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _cb.Json);

            return Request<EndPointResponse<StorageObjectRecord>>(message, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                if (response?.Success != true)
                    throw new InvalidOperationException($"AddOrUpdate failed: {response?.Message ?? "Unknown error"}");
            }, cancellationToken);
        }

        public Task RemoveIfExists (Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            string requestEndpoint = $"{Controller}/Record?{query}";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);

            return Request<EndPointResponse>(message, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                if (response?.Success != true)
                    throw new InvalidOperationException($"RemoveIfExists failed: {response?.Message ?? "Unknown error"}");
            }, cancellationToken);
        }

        public async Task<StorageObjectRecord> Upload (StorageObjectRecord record, byte[] bytes, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/object";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            var jsonRecord = record.ToJson<StorageObjectRecord>();
            var parameters = new StringContent(jsonRecord);
            var fileStreamContent = new ByteArrayContent(bytes);

            if (!string.IsNullOrWhiteSpace(record.MIME))
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(record.MIME);

            using var formData = new MultipartFormDataContent() {
                { parameters, "parameters" },
                { fileStreamContent, "file", record.Title! }
            };

            message.Content = formData;
            var response = await Request<EndPointResponse<StorageObjectRecord>>(message, cancellationToken);
            return response?.Data ?? throw new InvalidOperationException("Upload failed, no data returned.");
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            string requestEndpoint = $"{Controller}/Object?{query}";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);

            var response = await Request<EndPointResponse<StorageObjectRecord>>(message, cancellationToken);
            if (response?.Success != true)
                throw new InvalidOperationException($"Delete failed: {response?.Message ?? "Unknown error"}");
        }

        public string DownloadLink(Guid id, string? nocache = null)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            if (nocache != null) 
                query["nocache"] = nocache;

            var basepath = _cb.Client.BaseAddress!.ToString().TrimEnd('/');
            return $"{basepath}{Controller}/object?{query}";
        }

        public async Task<byte[]?> Download(Guid id, string? nocache = null, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            if (nocache != null)
                query["nocache"] = nocache;

            string requestEndpoint = $"{Controller}/object?{query}";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            return await RequestBytes(message, cancellationToken);
        }
    }
}
