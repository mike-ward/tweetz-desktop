using System;
using System.IO;
using System.Net;

namespace tweetz5.Model
{
    public interface IWebResponse : IDisposable
    {
        Stream GetResponseStream();
    }

    public sealed class WebResponseWrapper : IWebResponse
    {
        private WebResponse _response;

        public WebResponseWrapper(WebResponse response)
        {
            _response = response;
        }

        public Stream GetResponseStream()
        {
            return _response.GetResponseStream();
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _response.Dispose();
            _response = null;
        }
    }
}