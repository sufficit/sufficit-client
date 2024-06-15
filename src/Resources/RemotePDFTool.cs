using Sufficit.Resources;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Resources
{
    public class RemotePDFTool : IPDFTool
    {
        private readonly APIClientService _service;
        public RemotePDFTool(APIClientService service) => _service = service;

        public byte[] GetPDFFromHTML(string html)
        {
            var task = _service.Resources.HTMLToPDF(html, CancellationToken.None);
            task.Wait();

            return task.Result ?? Array.Empty<byte>();
        }
    }
}
