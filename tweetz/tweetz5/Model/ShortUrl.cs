using System.Net;
using System.Text.RegularExpressions;

namespace tweetz5.Model
{
    public static class ShortUrl
    {
        private static string Shorten(string link)
        {
            var url = "http://is.gd/create.php?format=simple&url=" + OAuth.UrlEncode(link);
            using (var client = new WebClient())
            {
                var shortUrl = client.DownloadString(url);
                return shortUrl;
            }
        }

        public static string ShortenUrls(string text)
        {
            var newText = Regex.Replace(
                text,
                @"((http|https)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z‌​0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*)",
                match => Shorten(match.Groups[1].Value));
            return newText;
        }
    }
}