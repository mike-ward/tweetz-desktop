// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

// ReSharper disable AssignNullToNotNullAttribute

namespace tweetz5.Model
{
    public static class LongUrl
    {
        private static readonly ConcurrentDictionary<string, string> ShortToLongUrl = new ConcurrentDictionary<string, string>();

        public async static Task<string> Lookup(string link)
        {
            try
            {
                string longUrl;
                if (ShortToLongUrl.TryGetValue(link, out longUrl))
                {
                    return longUrl;
                }

                var url = "http://api.longurl.org/v2/expand?format=json&url=" + OAuth.UrlEncode(link);
                var request = WebRequestWrapper.Create(new Uri(url));
                request.UserAgent = "tweetz/5.0";
                request.Timeout = 1500;
                using (var response = await request.GetResponseAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof (LongUrlResponse));
                    var longUrlResponse = (LongUrlResponse)serializer.ReadObject(response.GetResponseStream());
                    if (string.IsNullOrWhiteSpace(longUrlResponse.LongUrl) == false)
                    {
                        if (ShortToLongUrl.Count > 1000) ShortToLongUrl.Clear();
                        ShortToLongUrl.TryAdd(link, longUrlResponse.LongUrl);
                        return longUrlResponse.LongUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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