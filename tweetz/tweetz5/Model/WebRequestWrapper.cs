using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace tweetz5.Model
{
    public interface IWebRequest
    {
        WebHeaderCollection Headers { get; }
        string Method { set; }
        string ContentType { set; }
        string UserAgent { set; }
        int Timeout { set; }
        IWebResponse GetResponse();
        Stream GetRequestStream();
        Task<IWebResponse> GetResponseAsync();
    }

    public class WebRequestWrapper : IWebRequest
    {
        public static Func<Uri, IWebRequest> OverrideImplementation;
        private readonly WebRequest _request;

        private WebRequestWrapper(Uri address)
        {
            _request = WebRequest.Create(address);
        }

        public WebHeaderCollection Headers => _request.Headers;

        public IWebResponse GetResponse() => new WebResponseWrapper(_request.GetResponse());

        public string Method
        {
            set { _request.Method = value; }
        }

        public string ContentType
        {
            set { _request.ContentType = value; }
        }

        public Stream GetRequestStream() => _request.GetRequestStream();

        public string UserAgent
        {
            set { ((HttpWebRequest)_request).UserAgent = value; }
        }

        public int Timeout
        {
            set { _request.Timeout = value; }
        }

        public async Task<IWebResponse> GetResponseAsync() => new WebResponseWrapper(await _request.GetResponseAsync());

        public static IWebRequest Create(Uri address)
        {
            return OverrideImplementation != null
                ? OverrideImplementation(address)
                : new WebRequestWrapper(address);
        }
    }
}