using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

// ReSharper disable AssignNullToNotNullAttribute

namespace tweetz5.Model
{
    public static class LongUrl
    {
        private static readonly ConcurrentDictionary<string, string> ShortToLongUrl = new ConcurrentDictionary<string, string>();

        public static async Task<string> Lookup(string link)
        {
            try
            {
                string longUrl;
                if (ShortToLongUrl.TryGetValue(link, out longUrl))
                {
                    return longUrl;
                }

                var request = WebRequestWrapper.Create(new Uri(link));
                request.Timeout = 1500;
                request.Method = "HEAD";
                request.UserAgent = "tweetz/5.0";
                using (var response = await request.GetResponseAsync())
                {
                    var uri = response.ResponseUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(uri) == false)
                    {
                        if (ShortToLongUrl.Count > 1000) ShortToLongUrl.Clear();
                        ShortToLongUrl.TryAdd(link, uri);
                        return uri;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return link;
        }
    }

    [DataContract]
    public class LongUrlResponse
    {
        [DataMember(Name = "long-url")]
        public string LongUrl { get; set; }
    }
}