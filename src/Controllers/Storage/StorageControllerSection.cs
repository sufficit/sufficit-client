using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Storage
{
    public sealed class StorageControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/storage";

        public StorageControllerSection (IAuthenticatedControllerBase cb) : base(cb)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public Task<StorageObjectRecord?> ById (Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public IEnumerable<StorageObjectRecord> Search (StorageObjectMetadataSearchParameters parameters, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public async Task AddOrUpdate (StorageObjectRecord item, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public Task RemoveIfExists (Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public async Task Upload (StorageObjectRecord record, byte[] bytes, CancellationToken cancellationToken)
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
            await Request(message, cancellationToken);
        }

        public Task Delete(Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");

        public Task Download(Guid id, CancellationToken cancellationToken)
            => throw new NotImplementedException("Method not implemented in StorageControllerSection.");
    }
}
