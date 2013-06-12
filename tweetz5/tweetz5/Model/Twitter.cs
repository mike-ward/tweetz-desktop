// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable AssignNullToNotNullAttribute

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
            if (!post) fullUrl += "?" + string.Join("&", parameters.Select(p => string.Format("{0}={1}", OAuth.UrlEncode(p[0]), OAuth.UrlEncode(p[1]))));

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
            var parameters = new[]
            {
                new[] {"count", "50"},
                new[] {"include_rts", "true"},
                new[] {"include_entities", "true"},
                new[] {"since_id", _sinceIdHome.ToString(CultureInfo.InvariantCulture)}
            };
            return GetTimeline("https://api.twitter.com/1.1/statuses/home_timeline.json", parameters, ref _sinceIdHome);
        }

        private static ulong _sinceIdMentions = 1;

        public Status[] MentionsTimeline()
        {
            var parameters = new[]
            {
                new[] {"count", "50"},
                new[] {"include_entities", "true"},
                new[] {"since_id", _sinceIdMentions.ToString(CultureInfo.InvariantCulture)}
            };
            return GetTimeline("https://api.twitter.com/1.1/statuses/mentions_timeline.json", parameters, ref _sinceIdMentions);
        }

        private static ulong _sinceIdDirectMessages = 1;

        public Status[] DirectMessagesTimeline()
        {
            var parameters = new[]
            {
                new[] {"include_entities", "true"},
                new[] {"since_id", _sinceIdDirectMessages.ToString(CultureInfo.InvariantCulture)}
            };
            return GetTimeline("https://api.twitter.com/1.1/direct_messages.json", parameters, ref _sinceIdDirectMessages);
        }

        private static ulong _sinceIdFavorites = 1;

        public Status[] FavoritesTimeline()
        {
            var parameters = new[]
            {
                new[] { "count", "50" },
                new[] {"since_id", _sinceIdFavorites.ToString(CultureInfo.InvariantCulture)}
            };
            return GetTimeline("https://api.twitter.com/1.1/favorites/list.json", parameters, ref _sinceIdFavorites);

        }

        private static Status[] GetTimeline(string url, IEnumerable<string[]> parameters, ref ulong sinceId)
        {
            try
            {
                var json = Get(url, parameters);
                var statuses = Status.ParseJson(json);
                if (statuses.Length > 0)
                {
                    sinceId = Math.Max(sinceId, statuses.Max(s => ulong.Parse(s.Id)));
                }
                return statuses;
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                return new Status[0];
            }
        }

        public static string UpdateStatus(string message, string replyToStatusId = null)
        {
            var parameters = string.IsNullOrWhiteSpace(replyToStatusId)
                ? new[] { new[] { "status", message } }
                : new[] { new[] { "status", message }, new[] { "in_reply_to_status_id", replyToStatusId } };

            try
            {
                return Post("https://api.twitter.com/1.1/statuses/update.json", parameters);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static string CreateFavorite(string id)
        {
            try
            {
                var parameters = new[] { new[] { "id", id } };
                return Post("https://api.twitter.com/1.1/favorites/create.json", parameters);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static string DestroyFavorite(string id)
        {
            try
            {
                var parameters = new[] { new[] { "id", id } };
                return Post("https://api.twitter.com/1.1/favorites/destroy.json", parameters);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static string RetweetStatus(string id)
        {
            try
            {
                return Post(string.Format("https://api.twitter.com/1.1/statuses/retweet/{0}.json", id), new string[0][]);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static string DestroyStatus(string id)
        {
            try
            {
                return Post(string.Format("https://api.twitter.com/1.1/statuses/destroy/{0}.json", id), new string[0][]);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static string GetTweet(string id)
        {
            var parameters = new[]
                {
                    new[] {"id", id},
                    new[] {"include_my_retweet", "true"}
                };
            try
            {
                return Get("https://api.twitter.com/1.1/statuses/show.json", parameters);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public static User GetUserInformation(string screenName)
        {
            try
            {
                var parameters = new[]
                {
                    new[] {"screen_name", screenName},
                    new[] {"include_entities", "true"}
                };
                var json = Get("https://api.twitter.com/1.1/users/show.json", parameters);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(User));
                    return (User)serializer.ReadObject(stream);
                }
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return new User { Name = "Error!" };
            }
        }

        public static bool Following(string screenName)
        {
            try
            {
                var parameters = new[] { new[] { "screen_name", screenName } };
                var json = Get("https://api.twitter.com/1.1/friendships/lookup.json", parameters);
                return json.Contains("\"following\"");
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool Follow(string screenName)
        {
            try
            {
                var parameters = new[]
                {
                    new[] {"screen_name", screenName},
                    new[] {"following", "true"}
                };
                try
                {
                    var json = Post("https://api.twitter.com/1.1/friendships/create.json", parameters);
                    return json.Contains(screenName);
                }
                catch (WebException e)
                {
                    ShowAlert(e.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                ShowAlert(e.Message);
                return false;
            }
        }

        public static bool Unfollow(string screenName)
        {
            try
            {
                var parameters = new[] { new[] { "screen_name", screenName } };
                var json = Post("https://api.twitter.com/1.1/friendships/destroy.json", parameters);
                return json.Contains(screenName);
            }
            catch (Exception e)
            {
                ShowAlert(e.Message);
                return false;
            }
        }

        public static string Search(string query, string sinceId = "1")
        {
            var parameters = new[]
            {
                new[] { "q", query }, 
                new[] { "count", "50" }, 
                new[] { "since_id", sinceId }
            };
            try
            {
                return Get("https://api.twitter.com/1.1/search/tweets.json", parameters);
            }
            catch (WebException e)
            {
                ShowAlert(e.Message);
                return string.Empty;
            }
        }

        public class OAuthTokens
        {
            public string OAuthToken { get; set; }
            public string OAuthSecret { get; set; }
            public string UserId { get; set; }
            public string ScreenName { get; set; }
        }

        public static OAuthTokens GetRequestToken()
        {
            var nonce = OAuth.Nonce();
            var timestamp = OAuth.TimeStamp();
            var oauth = new OAuth();


            const string format = "{0}={1}";
            var parameterStrings = new List<string>
            {
                string.Format(format, "oauth_callback", "oob"),
                string.Format(format, "oauth_consumer_key", OAuth.UrlEncode(oauth.ConsumerKey)),
                string.Format(format, "oauth_nonce", nonce),
                string.Format(format, "oauth_signature_method", "HMAC-SHA1"),
                string.Format(format, "oauth_timestamp", timestamp),
                string.Format(format, "oauth_version", "1.0"),
            };

            const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
            var parameterString = string.Join("&", parameterStrings);
            var signatureBaseString = string.Format("{0}&{1}&{2}", "POST", OAuth.UrlEncode(requestTokenUrl), OAuth.UrlEncode(parameterString));
            var compositeKey = string.Format("{0}&", OAuth.UrlEncode(oauth.ConsumerSecret));
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));

                const string headerFormat =
                                "OAuth " +
                                    "oauth_callback=\"{0}\"," +
                                    "oauth_consumer_key=\"{1}\"," +
                                    "oauth_nonce=\"{2}\"," +
                                    "oauth_timestamp=\"{3}\"," +
                                    "oauth_signature=\"{4}\"," +
                                    "oauth_signature_method=\"HMAC-SHA1\"," +
                                    "oauth_version=\"1.0\"";

                var authorizeHeader = string.Format(headerFormat,
                    OAuth.UrlEncode("oob"),
                    OAuth.UrlEncode(oauth.ConsumerKey),
                    OAuth.UrlEncode(nonce),
                    OAuth.UrlEncode(timestamp),
                    OAuth.UrlEncode(signature));

                var request = System.Net.WebRequest.Create(requestTokenUrl);
                request.Method = "POST";
                request.Headers.Add("Authorization", authorizeHeader);

                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var body = stream.ReadToEnd();
                        var tokens = body.Split('&');
                        var oauthToken = Token(tokens[0]);
                        var oauthSecret = Token(tokens[1]);
                        var callbackConfirmed = Token(tokens[2]);

                        if (callbackConfirmed != "true") throw new InvalidProgramException("callback token not confirmed");
                        return new OAuthTokens { OAuthToken = oauthToken, OAuthSecret = oauthSecret };
                    }
                }
            }
        }

        public static OAuthTokens GetAccessToken(string oauthVerifier)
        {
            var parameters = new[] { new[] { "oauth_verifier", oauthVerifier } };
            var response = Post("https://api.twitter.com/oauth/access_token", parameters);

            var tokens = response.Split('&');
            var oauthTokens = new OAuthTokens
            {
                OAuthToken = Token(tokens[0]),
                OAuthSecret = Token(tokens[1]),
                UserId = Token(tokens[2]),
                ScreenName = Token(tokens[3])
            };
            return oauthTokens;
        }

        private static string Token(string pair)
        {
            return pair.Substring(pair.IndexOf('=') + 1);
        }

        private static void ShowAlert(string message)
        {
            Application.Current.Dispatcher.Invoke(() => MainWindow.AlertCommand.Execute(message, Application.Current.MainWindow));
        }
    }
}