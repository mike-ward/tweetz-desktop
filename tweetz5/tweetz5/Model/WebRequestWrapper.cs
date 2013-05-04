// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.IO;
using System.Net;

namespace tweetz5.Model
{
    public interface IWebRequest
    {
        WebHeaderCollection Headers { get; set; }
        IWebResponse GetResponse();
        string Method { get; set; }
        string ContentType { get; set; }
        Stream GetRequestStream();
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

        public WebHeaderCollection Headers
        {
            get { return _request.Headers; }
            set { _request.Headers = value; }
        }

        public IWebResponse GetResponse()
        {
            return new WebResponseWrapper(_request.GetResponse());
        }

        public string Method
        {
            get { return _request.Method; }
            set { _request.Method = value; }
        }

        public string ContentType
        {
            get { return _request.ContentType; }
            set { _request.ContentType = value; }
        }

        public Stream GetRequestStream()
        {
            return _request.GetRequestStream();
        }
    }
}