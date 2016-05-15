using System;
using System.IO;
using System.Net;

namespace tweetz5.Model
{
    public interface IWebResponse : IDisposable
    {
        Stream GetResponseStream();
        Uri ResponseUri { get; }
    }

    public sealed class WebResponseWrapper : IWebResponse
    {
        private bool _disposed;
        private WebResponse _response;

        public WebResponseWrapper(WebResponse response)
        {
            _response = response;
        }

        public Stream GetResponseStream() => _response.GetResponseStream();

        public Uri ResponseUri => _response.ResponseUri;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _response.Dispose();
            _response = null;
        }
    }
}