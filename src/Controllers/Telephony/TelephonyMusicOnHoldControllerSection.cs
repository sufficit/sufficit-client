﻿using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Json;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyMusicOnHoldControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/moh";

        private readonly System.Text.Json.JsonSerializerOptions _json;

        public TelephonyMusicOnHoldControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        #region CLASSES

        public Task<IEnumerable<MusicOnHoldClass>> Search(MusicOnHoldSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<MusicOnHoldClass>(message, cancellationToken);
        }

        public async Task AddOrUpdate(MusicOnHoldInfo info, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/class";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(info, null, _json);
            var response = await Request<EndPointResponse>(message, cancellationToken);
            if (response != null)
            {
                var json = response.Data.ToJsonOrDefault();
                if (json != null)
                {
                    var infor = JsonSerializer.Deserialize<MusicOnHoldInfo>(json);
                    if (infor != null)
                        info.Id = infor.Id;
                }                
            }
        }

        public Task Remove(Guid id, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/class";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        #endregion
        #region FILES ENTRIES

        public Task<IEnumerable<MusicOnHoldStorageObject>> Files(Guid id, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/list";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<MusicOnHoldStorageObject>(message, cancellationToken);
        }

        public async Task Upload(Guid id, byte[] bytes, string filename, string? contentType, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/audio";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            var idContent = new StringContent(id.ToString());
            var filenameContent = new StringContent(filename);
            var fileStreamContent = new ByteArrayContent(bytes);

            if (!string.IsNullOrWhiteSpace(contentType))
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            using var formData = new MultipartFormDataContent() {
                { idContent, "id" },
                { filenameContent, "title" },
                { fileStreamContent, "audio", filename }
            };

            message.Content = formData;
            await Request(message, cancellationToken);
        }

        public Task Delete(Guid id, string title, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/audio";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            query["title"] = title;

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        #endregion
    }
}
