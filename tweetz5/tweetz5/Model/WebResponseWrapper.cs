// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.IO;
using System.Net;

namespace tweetz5.Model
{
    public interface IWebResponse : IDisposable
    {
        Stream GetResponseStream();
    }

    public class WebResponseWrapper : IWebResponse
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            _response.Dispose();
            _response = null;
        }
    }
}