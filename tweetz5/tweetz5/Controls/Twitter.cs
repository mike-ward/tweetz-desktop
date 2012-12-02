// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace tweetz5.Model
{
    public class Twitter
    {
        private static IWebRequest WebRequest(string url, IEnumerable<string[]> parameters)
        {
            var nonce = OAuth.Nonce();
            var timestamp = OAuth.TimeStamp();
            var oauth = new OAuth();
            var signature = oauth.Signature("GET", url, nonce, timestamp, parameters);
            var authorizeHeader = oauth.AuthorizationHeader(nonce, timestamp, signature);
            var fullUrl = url + "?" + string.Join("&", parameters.Select(p => string.Format("{0}={1}", p[0], p[1])));

            var request = WebRequestWrapper.Create(new Uri(fullUrl));
            request.Headers.Add("Authorization", authorizeHeader);
            return request;
        }

        private static string Get(string url, IEnumerable<string[]> parameters)
        {
            var request = WebRequest(url, parameters);
            using (var response = request.GetResponse())
            {
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return stream.ReadToEnd();
                }
            }
        }

        //private static void GetAsync(string url, IEnumerable<string[]> parameters, Action<Status[]> process)
        //{
        //    var request = WebRequest(url, parameters);
        //    request.BeginGetRequestStream(ar =>
        //    {
        //        using (var response = request.EndGetResponse(ar))
        //        using (var reader = new StreamReader(response.GetResponseStream()))
        //        {
        //            while (!reader.EndOfStream)
        //            {
        //                var json = reader.ReadLine();
        //                var statuses = Status.ParseJson(json);
        //                process(statuses);
        //            }
        //        }
        //    });
        //}

        private static ulong _sinceIdHome = 1;

        public Status[] HomeTimeline()
        {
            try
            {
                var parameters = new[]
                {
                    new[] {"count", "50"},
                    new[] {"include_rts", "true"},
                    new[] {"include_entities", "true"},
                    new[] {"since_id", _sinceIdHome.ToString(CultureInfo.InvariantCulture)}
                };
                var json = Get("https://api.twitter.com/1.1/statuses/home_timeline.json", parameters);
                var statuses = Status.ParseJson(json);
                if (statuses.Length > 0)
                {
                    _sinceIdHome = Math.Max(_sinceIdHome, statuses.Max(s => ulong.Parse(s.Id)));
                }
                return statuses;
            }
            catch (WebException)
            {
                return new Status[0];
            }
        }

        private static ulong _sinceIdMentions = 1;

        public Status[] MentionsTimeline()
        {
            try
            {
                var parameters = new[]
                {
                    new[] {"count", "50"},
                    new[] {"include_entities", "true"},
                    new[] {"since_id", _sinceIdMentions.ToString(CultureInfo.InvariantCulture)}
                };
                var json = Get("https://api.twitter.com/1.1/statuses/mentions_timeline.json", parameters);
                var statuses = Status.ParseJson(json);
                if (statuses.Length > 0)
                {
                    _sinceIdMentions = Math.Max(_sinceIdMentions, statuses.Max(s => ulong.Parse(s.Id)));
                }
                return statuses;
            }
            catch (WebException)
            {
                return new Status[0];
            }
        }
    }
}