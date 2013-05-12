// Copyright (c) 2013 Blue Onion Software - All rights reserved

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
        private static IWebRequest WebRequest(string url, IEnumerable<string[]> parameters, bool post = false)
        {
            var nonce = OAuth.Nonce();
            var timestamp = OAuth.TimeStamp();
            var oauth = new OAuth();
            var signature = oauth.Signature(post ? "POST" : "GET", url, nonce, timestamp, parameters);
            var authorizeHeader = oauth.AuthorizationHeader(nonce, timestamp, signature);
            var fullUrl = url;
            if (!post) fullUrl += "?" + string.Join("&", parameters.Select(p => string.Format("{0}={1}", p[0], p[1])));

            var request = WebRequestWrapper.Create(new Uri(fullUrl));
            request.Headers.Add("Authorization", authorizeHeader);
            request.Method = post ? "POST" : "GET";
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

        private static string Post(string url, IEnumerable<string[]> parameters)
        {
            var request = WebRequest(url, parameters, true);
            request.ContentType = "application/x-www-form-urlencoded";

            if (parameters != null)
            {
                var parameterStrings = new List<string>();
                parameterStrings.AddRange(parameters.Select(par => string.Format("{0}={1}", par[0], OAuth.UrlEncode(par[1]))));
                parameterStrings.Sort();
                var parameterString = string.Join("&", parameterStrings);
                var requestStream = request.GetRequestStream();
                var buffer = Encoding.ASCII.GetBytes(parameterString);
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
            }

            using (var response = request.GetResponse())
            {
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var result = stream.ReadToEnd();
                    return result;
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

        public static string UpdateStatus(string message, string replyToStatusId = null)
        {
            var parameters = string.IsNullOrWhiteSpace(replyToStatusId)
                ? new[] {new[] {"status", message}}
                : new[] {new[] {"status", message}, new[] {"in_reply_to_status_id", replyToStatusId}};

            return Post("https://api.twitter.com/1.1/statuses/update.json", parameters);
        }

        public static string Favorite(string id)
        {
            var parameters = new[] {new[] {"id", id}};
            return Post("https://api.twitter.com/1.1/favorites/create.json", parameters);
        }
    }
}