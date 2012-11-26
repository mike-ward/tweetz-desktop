// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Net;

namespace tweetz5.Model
{
    public interface IWebRequest
    {
        WebHeaderCollection Headers { get; set; }
        IWebResponse GetResponse();
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
    }
}