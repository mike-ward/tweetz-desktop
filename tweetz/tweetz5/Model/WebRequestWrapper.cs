// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace tweetz5.Model
{
    public interface IWebRequest
    {
        WebHeaderCollection Headers { get; }
        IWebResponse GetResponse();
        string Method { set; }
        string ContentType { set; }
        Stream GetRequestStream();
        string UserAgent { set; }
        int Timeout { set; }
        Task<IWebResponse> GetResponseAsync();
    }

    public class WebRequestWrapper : IWebRequest
    {
        private readonly WebRequest _request;

        private WebRequestWrapper(Uri address)
        {
            _request = WebRequest.Create(address);
        }

        public static Func<Uri, IWebRequest> OverrideImplementation;

        public static IWebRequest Create(Uri address)
        {
            return (OverrideImplementation != null)
                ? OverrideImplementation(address)
                : new WebRequestWrapper(address);
        }

        public WebHeaderCollection Headers => _request.Headers;

        public IWebResponse GetResponse()
        {
            return new WebResponseWrapper(_request.GetResponse());
        }


        public string Method
        {
            set { _request.Method = value; }
        }

        public string ContentType
        {
            set { _request.ContentType = value; }
        }

        public Stream GetRequestStream()
        {
            return _request.GetRequestStream();
        }

        public string UserAgent
        {
            set { ((HttpWebRequest) _request).UserAgent = value; }
        }

        public int Timeout
        {
            set { _request.Timeout = value; }
        }

        public async Task<IWebResponse> GetResponseAsync()
        {
            return new WebResponseWrapper(await _request.GetResponseAsync());
        }
    }
}