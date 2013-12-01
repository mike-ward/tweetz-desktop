// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace tweetz5.Model
{
    public class OAuth
    {
        public static string ConsumerKey { get; private set; }
        public static string ConsumerSecret { get; private set; }
        public string AccessTokenSecret { get; set; }
        public string AccessToken { get; set; }
        public string ScreenName { get; set; }

        static OAuth()
        {
            ServicePointManager.Expect100Continue = false;
            ConsumerKey = "ZScn2AEIQrfC48Zlw";
            ConsumerSecret = "8gKdPBwUfZCQfUiyeFeEwVBQiV3q50wIOrIjoCxa2Q";
        }

        public OAuth()
        {
            AccessToken = Properties.Settings.Default.AccessToken;
            AccessTokenSecret = Properties.Settings.Default.AccessTokenSecret;
            ScreenName = Properties.Settings.Default.ScreenName;
        }

        public static string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }

        public static string Nonce()
        {
            return Guid.NewGuid().ToString();
        }

        public static string TimeStamp()
        {
            var timespan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(timespan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public static string Signature(string httpMethod, string url, string nonce, string timestamp, string accessToken, string accessTokenSecret, IEnumerable<string[]> parameters)
        {
            var parameterList = OrderedParameters(nonce, timestamp, accessToken, null, parameters);
            var parameterStrings = parameterList.Select(p => string.Format("{0}={1}", p.Item1, p.Item2));
            var parameterString = string.Join("&", parameterStrings);
            var signatureBaseString = string.Format("{0}&{1}&{2}", httpMethod, UrlEncode(url), UrlEncode(parameterString));
            var compositeKey = string.Format("{0}&{1}", UrlEncode(ConsumerSecret), UrlEncode(accessTokenSecret));
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(compositeKey)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString)));
            }
        }

        public static string AuthorizationHeader(string nonce, string timestamp, string accessToken, string signature, IEnumerable<string[]> parameters = null)
        {
            var parameterList = OrderedParameters(nonce, timestamp, accessToken, signature, parameters);
            var parameterStrings = parameterList.Select(p => string.Format("{0}=\"{1}\"", p.Item1, p.Item2));
            var header = "OAuth " + string.Join(",", parameterStrings);
            return header;
        }

        private static IEnumerable<Tuple<string, string>> OrderedParameters(string nonce, string timestamp, string accessToken, string signature, IEnumerable<string[]> parameters)
        {
            var components = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("oauth_version", "1.0"),
                new Tuple<string, string>("oauth_nonce", UrlEncode(nonce)),
                new Tuple<string, string>("oauth_timestamp", UrlEncode(timestamp)),
                new Tuple<string, string>("oauth_signature_method", "HMAC-SHA1"),
                new Tuple<string, string>("oauth_consumer_key", UrlEncode(ConsumerKey))
            };

            if (string.IsNullOrWhiteSpace(signature) == false)
            {
                components.Add(new Tuple<string, string>("oauth_signature", UrlEncode(signature)));                
            }
            if (string.IsNullOrWhiteSpace(accessToken) == false)
            {
                components.Add(new Tuple<string, string>("oauth_token", UrlEncode(accessToken)));
            }

            if (parameters != null)
            {
                components.AddRange(parameters.Select(par => new Tuple<string, string>(UrlEncode(par[0]), UrlEncode(par[1]))));
            }
            return components.OrderBy(c => c.Item1);
        }
    }
}