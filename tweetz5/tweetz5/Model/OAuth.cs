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
            //AccessToken = "14410002-tTUqt6ujPyLLcn3OjgdlmSdl9e6ta1OISAWS1Gs8I";
            //AccessTokenSecret = "zaBS7G3f8n0F6zxGwNJEmlU4zg1P6VgL5cPyEgShI";
            //ScreenName = "mikeward_aa";
        }

        public static string UrlEncode(string value)
        {
            var result = new StringBuilder();
            const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            foreach (var val in value)
            {
                result.Append((unreservedChars.IndexOf(val) != -1)
                    ? val.ToString(CultureInfo.InvariantCulture)
                    : String.Format("%{0:X2}", (int)val));
            }
            return result.ToString();
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
            const string format = "{0}={1}";
            var parameterStrings = new List<string>
            {
                string.Format(format, "oauth_version", "1.0"),
                string.Format(format, "oauth_nonce", nonce),
                string.Format(format, "oauth_timestamp", timestamp),
                string.Format(format, "oauth_signature_method", "HMAC-SHA1"),
                string.Format(format, "oauth_consumer_key", UrlEncode(ConsumerKey)),
            };
            if (string.IsNullOrWhiteSpace(accessToken) == false)
                parameterStrings.Add(string.Format(format, "oauth_token", UrlEncode(accessToken)));

            if (parameters != null)
                parameterStrings.AddRange(parameters.Select(par => string.Format(format, UrlEncode(par[0]), UrlEncode(par[1]))));
            
            parameterStrings.Sort();
            var parameterString = string.Join("&", parameterStrings);
            var signatureBaseString = string.Format("{0}&{1}&{2}", httpMethod, UrlEncode(url), UrlEncode(parameterString));
            var compositeKey = string.Format("{0}&{1}", UrlEncode(ConsumerSecret), UrlEncode(accessTokenSecret));
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));
            }
        }

        public static string AuthorizationHeader(string nonce, string timestamp, string accessToken, string signature, IEnumerable<string[]> parameters = null)
        {
            const string format = "{0}=\"{1}\"";
            var parameterStrings = new List<string>
            {
                string.Format(format, "oauth_version", "1.0"),
                string.Format(format, "oauth_nonce", UrlEncode(nonce)),
                string.Format(format, "oauth_timestamp", UrlEncode(timestamp)),
                string.Format(format, "oauth_signature", UrlEncode(signature)),
                string.Format(format, "oauth_signature_method", "HMAC-SHA1"),
                string.Format(format, "oauth_consumer_key", UrlEncode(ConsumerKey)),
            };
            if (string.IsNullOrWhiteSpace(accessToken) == false)
                parameterStrings.Add(string.Format(format, "oauth_token", UrlEncode(accessToken)));
            
            if (parameters != null)
                parameterStrings.AddRange(parameters.Select(par => string.Format(format, UrlEncode(par[0]), UrlEncode(par[1]))));

            parameterStrings.Sort();
            var header = "OAuth " + string.Join(",", parameterStrings);
            return header;
        }
    }
}