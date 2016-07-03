using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using tweetz5.Commands;

namespace tweetz5.Model
{
    public static class Twitter
    {
        private static async Task<string> Get(string url, IEnumerable<string[]> parameters)
        {
            return await WebRequest(url, parameters);
        }

        private static async Task<string> Post(string url, IEnumerable<string[]> parameters)
        {
            return await WebRequest(url, parameters, true);
        }

        private static async Task<string> WebRequest(string url, IEnumerable<string[]> parameters, bool post = false)
        {
            var oauth = new OAuth();
            var nonce = OAuth.Nonce();
            var timestamp = OAuth.TimeStamp();
            var signature = OAuth.Signature(post ? "POST" : "GET", url, nonce, timestamp, oauth.AccessToken, oauth.AccessTokenSecret, parameters);
            var authorizeHeader = OAuth.AuthorizationHeader(nonce, timestamp, oauth.AccessToken, signature);
            var parameterStrings = parameters.Select(p => $"{OAuth.UrlEncode(p[0])}={OAuth.UrlEncode(p[1])}").ToList();
            if (!post) url += "?" + string.Join("&", parameterStrings);

            var request = WebRequestWrapper.Create(new Uri(url));
            request.Headers.Add("Authorization", authorizeHeader);
            request.Method = post ? "POST" : "GET";

            if (post)
            {
                Trace.TraceInformation(string.Join("&", parameterStrings));
                request.ContentType = "application/x-www-form-urlencoded";
                if (parameters != null)
                {
                    using (var requestStream = request.GetRequestStream())
                    {
                        WriteStream(requestStream, string.Join("&", parameterStrings));
                    }
                }
            }

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var result = stream.ReadToEnd();
                    return result;
                }
            }
        }

        public static async Task<Status[]> HomeTimeline(ulong sinceId)
        {
            var parameters = new[]
            {
                new[] {"count", "150"},
                new[] {"include_rts", "true"},
                new[] {"include_entities", "true"},
                new[] {"since_id", sinceId.ToString(CultureInfo.InvariantCulture)}
            };
            return await GetStatus("https://api.twitter.com/1.1/statuses/home_timeline.json", parameters);
        }

        public static async Task<Status[]> MentionsTimeline(ulong sinceId)
        {
            var parameters = new[]
            {
                new[] {"count", "50"},
                new[] {"include_entities", "true"},
                new[] {"since_id", sinceId.ToString(CultureInfo.InvariantCulture)}
            };
            return await GetStatus("https://api.twitter.com/1.1/statuses/mentions_timeline.json", parameters);
        }

        public static async Task<Status[]> DirectMessages(ulong sinceId)
        {
            var parameters = new[]
            {
                new[] {"include_entities", "true"},
                new[] {"since_id", sinceId.ToString(CultureInfo.InvariantCulture)}
            };
            return await GetStatus("https://api.twitter.com/1.1/direct_messages.json", parameters);
        }

        public static async Task<Status[]> DirectMessagesSent(ulong sinceId)
        {
            var parameters = new[]
            {
                new[] {"include_entities", "true"},
                new[] {"since_id", sinceId.ToString(CultureInfo.InvariantCulture)}
            };
            return await GetStatus("https://api.twitter.com/1.1/direct_messages/sent.json", parameters);
        }

        public static async Task<Status[]> Favorites(ulong sinceId)
        {
            var parameters = new[]
            {
                new[] {"count", "50"},
                new[] {"since_id", sinceId.ToString(CultureInfo.InvariantCulture)}
            };
            return await GetStatus("https://api.twitter.com/1.1/favorites/list.json", parameters);
        }

        private static async Task<Status[]> GetStatus(string url, IEnumerable<string[]> parameters)
        {
            try
            {
                var json = await Get(url, parameters);
                var statuses = Status.ParseJson(json);
                return statuses;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new Status[0];
            }
        }

        public static async Task<string> UpdateStatus(string message, string replyToStatusId = null)
        {
            var parameters = string.IsNullOrWhiteSpace(replyToStatusId)
                ? new[] {new[] {"status", message}}
                : new[] {new[] {"status", message}, new[] {"in_reply_to_status_id", replyToStatusId}};

            return await RequestHandler(
                () => Post("https://api.twitter.com/1.1/statuses/update.json", parameters),
                () => string.Empty);
        }

        public static async Task CreateFavorite(string id)
        {
            var parameters = new[] {new[] {"id", id}};
            await RequestHandler(
                () => Post("https://api.twitter.com/1.1/favorites/create.json", parameters),
                () => string.Empty);
        }

        public static async Task DestroyFavorite(string id)
        {
            var parameters = new[] {new[] {"id", id}};
            await RequestHandler(
                () => Post("https://api.twitter.com/1.1/favorites/destroy.json", parameters),
                () => string.Empty);
        }

        public static async Task RetweetStatus(string id)
        {
            await RequestHandler(
                () => Post($"https://api.twitter.com/1.1/statuses/retweet/{id}.json", new string[0][]),
                () => string.Empty);
        }

        public static async Task DestroyStatus(string id)
        {
            await RequestHandler(
                () => Post($"https://api.twitter.com/1.1/statuses/destroy/{id}.json", new string[0][]),
                () => string.Empty);
        }

        public static async Task<string> GetTweet(string id)
        {
            var parameters = new[]
            {
                new[] {"id", id},
                new[] {"include_my_retweet", "true"}
            };
            return await RequestHandler(
                () => Get("https://api.twitter.com/1.1/statuses/show.json", parameters),
                () => string.Empty);
        }

        public static async Task<User> GetUserInformation(string screenName)
        {
            return await RequestHandler(async () =>
            {
                var parameters = new[]
                {
                    new[] {"screen_name", screenName},
                    new[] {"include_entities", "true"}
                };
                var json = await Get("https://api.twitter.com/1.1/users/show.json", parameters);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(User));
                    return (User)serializer.ReadObject(stream);
                }
            }, () => new User {Name = "Error!"});
        }

        public static async Task<Friendship> Friendship(string screenName)
        {
            return await RequestHandler(async () =>
            {
                var parameters = new[] {new[] {"screen_name", screenName}};
                var json = await Get("https://api.twitter.com/1.1/friendships/lookup.json", parameters);
                var friendship = new Friendship {Following = json.Contains("\"following\""), FollowedBy = json.Contains("\"followed_by\"")};
                return friendship;
            }, () => new Friendship());
        }

        public static async Task<bool> Follow(string screenName)
        {
            return await RequestHandler(async () =>
            {
                var parameters = new[]
                {
                    new[] {"screen_name", screenName},
                    new[] {"following", "true"}
                };
                var json = await Post("https://api.twitter.com/1.1/friendships/create.json", parameters);
                return json.Contains(screenName);
            });
        }

        public static async Task<bool> Unfollow(string screenName)
        {
            return await RequestHandler(async () =>
            {
                var parameters = new[] {new[] {"screen_name", screenName}};
                var json = await Post("https://api.twitter.com/1.1/friendships/destroy.json", parameters);
                return json.Contains(screenName);
            });
        }

        public static async Task<string> SendDirectMessage(string text, string screenName)
        {
            return await RequestHandler(async () =>
            {
                var parameters = new[]
                {
                    new[] {"screen_name", screenName},
                    new[] {"text", text}
                };

                var json = await Post("https://api.twitter.com/1.1/direct_messages/new.json", parameters);
                return json;
            }, () => string.Empty);
        }

        public static async Task<string> Search(string query, string sinceId = "1")
        {
            return await RequestHandler(() =>
            {
                var parameters = new[]
                {
                    new[] {"q", query},
                    new[] {"count", "100"},
                    new[] {"since_id", sinceId}
                };
                return Get("https://api.twitter.com/1.1/search/tweets.json", parameters);
            }, () => string.Empty);
        }

        public static async Task<OAuthTokens> GetRequestToken()
        {
            return await RequestHandler(async () =>
            {
                const string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
                var nonce = OAuth.Nonce();
                var timestamp = OAuth.TimeStamp();
                var parameters = new[] {new[] {"oauth_callback", "oob"}};
                var signature = OAuth.Signature("POST", requestTokenUrl, nonce, timestamp, "", "", parameters);
                var authorizationHeader = OAuth.AuthorizationHeader(nonce, timestamp, null, signature, parameters);

                var request = System.Net.WebRequest.Create(new Uri(requestTokenUrl));
                request.Method = "POST";
                request.Headers.Add("Authorization", authorizationHeader);
                using (var response = await request.GetResponseAsync())
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var body = stream.ReadToEnd();
                    var tokens = body.Split('&');
                    var oauthToken = Token(tokens[0]);
                    var oauthSecret = Token(tokens[1]);
                    var callbackConfirmed = Token(tokens[2]);

                    if (callbackConfirmed != "true") throw new InvalidProgramException("callback token not confirmed");
                    return new OAuthTokens {OAuthToken = oauthToken, OAuthSecret = oauthSecret};
                }
            });
        }

        public static async Task<OAuthTokens> GetAccessToken(string accessToken, string accessTokenSecret, string oauthVerifier)
        {
            return await RequestHandler(async () =>
            {
                const string requestTokenUrl = "https://api.twitter.com/oauth/access_token";
                var nonce = OAuth.Nonce();
                var timestamp = OAuth.TimeStamp();
                var parameters = new[] {new[] {"oauth_verifier", oauthVerifier}};
                var signature = OAuth.Signature("POST", requestTokenUrl, nonce, timestamp, accessToken, accessTokenSecret, parameters);
                var authorizationHeader = OAuth.AuthorizationHeader(nonce, timestamp, accessToken, signature, parameters);

                var request = System.Net.WebRequest.Create(new Uri(requestTokenUrl));
                request.Method = "POST";
                request.Headers.Add("Authorization", authorizationHeader);

                using (var response = await request.GetResponseAsync())
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var tokens = stream.ReadToEnd().Split('&');
                    var oauthTokens = new OAuthTokens
                    {
                        OAuthToken = Token(tokens[0]),
                        OAuthSecret = Token(tokens[1]),
                        UserId = Token(tokens[2]),
                        ScreenName = Token(tokens[3])
                    };
                    return oauthTokens;
                }
            });
        }

        public static async Task<string> UpdateStatusWithMedia(string message, string filename)
        {
            return await RequestHandler(async () =>
            {
                var media = File.ReadAllBytes(filename);
                var mediaName = Path.GetFileName(filename);

                const string url = "https://api.twitter.com/1.1/statuses/update_with_media.json";
                var oauth = new OAuth();
                var nonce = OAuth.Nonce();
                var timestamp = OAuth.TimeStamp();
                var signature = OAuth.Signature("POST", url, nonce, timestamp, oauth.AccessToken, oauth.AccessTokenSecret, null);
                var authorizeHeader = OAuth.AuthorizationHeader(nonce, timestamp, oauth.AccessToken, signature);

                var request = System.Net.WebRequest.Create(new Uri(url));
                request.Headers.Add("Authorization", authorizeHeader);
                request.Method = "POST";

                var formDataBoundary = $"{Guid.NewGuid():N}";
                var contentType = "multipart/form-data; boundary=" + formDataBoundary;
                request.ContentType = contentType;

                using (var requestStream = request.GetRequestStream())
                {
                    var header = $"--{formDataBoundary}\r\nContent-Disposition: form-data; name=\"status\"\r\n\r\n";
                    var footer = $"\r\n--{formDataBoundary}--\r\n";
                    WriteStream(requestStream, header);
                    WriteStream(requestStream, message);

                    header = $"\r\n--{formDataBoundary}\r\nContent-Type: application/octet-stream\r\n"
                             + $"Content-Disposition: form-data; name=\"media[]\"; filename=\"{mediaName}\"\r\n\r\n";
                    WriteStream(requestStream, header);
                    requestStream.Write(media, 0, media.Length);
                    WriteStream(requestStream, footer);
                }

                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var result = stream.ReadToEnd();
                        return result;
                    }
                }
            }, () => string.Empty);
        }

        private static async Task<T> RequestHandler<T>(Func<Task<T>> request, Func<T> onError = null)
        {
            try
            {
                return await request();
            }
            catch (WebException ex)
            {
                ShowAlert(GetWebErrorResponse(ex));
                return onError != null ? onError() : default(T);
            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                return onError != null ? onError() : default(T);
            }
        }

        private static string GetWebErrorResponse(WebException ex)
        {
            try
            {
                using (var stream = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
                {
                    var serializer = new JavaScriptSerializer();
                    var json = stream.ReadToEnd();
                    var errors = serializer.Deserialize<TwitterErrors>(json);
                    return string.Join("\n", errors.Errors.Select(e => $"code: {e.Code}, message: {e.Message}"));
                }
            }
            catch (Exception)
            {
                return ex.Message;
            }
        }

        private static void WriteStream(Stream stream, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static string Token(string pair)
        {
            return pair.Substring(pair.IndexOf('=') + 1);
        }

        private static void ShowAlert(string message)
        {
            Application.Current.Dispatcher.InvokeAsync(() => AlertCommand.Command.Execute(message, Application.Current.MainWindow));
        }

        public class OAuthTokens
        {
            public string OAuthToken { get; set; }
            public string OAuthSecret { get; set; }
            public string UserId { get; set; }
            public string ScreenName { get; set; }
        }
    }
}