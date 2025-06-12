using Sufficit.EndPoints;
using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Storage;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public IEnumerable<StorageObjectRecord> Search (StorageObjectMetadataSearchParameters parameters, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public Task AddOrUpdate (StorageObjectRecord item, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public Task RemoveIfExists (Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

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

        public Task Delete(Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

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
