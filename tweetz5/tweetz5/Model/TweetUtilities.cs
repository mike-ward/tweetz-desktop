using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using tweetz5.Utilities.Translate;

namespace tweetz5.Model
{
    public static class TweetUtilities
    {
        public static Tweet CreateTweet(this Status status, TweetClassification tweetType)
        {
            var isMention = false;
            var isDirectMessage = false;
            var username = new OAuth().ScreenName;
            var displayStatus = status.RetweetedStatus ?? status;

            // Direct messages don't have a User. Instead, dm's use sender and recipient collections.
            if (displayStatus.User == null)
            {
                isDirectMessage = true;
                displayStatus.User = (status.Recipient.ScreenName == username) ? status.Sender : status.Recipient;
            }

            if (status.Entities != null
                && status.Entities.Mentions != null
                && status.Entities.Mentions.Any(m => m.ScreenName == username))
            {
                isMention = true;
            }

            var createdAt = DateTime.ParseExact(status.CreatedAt, "ddd MMM dd HH:mm:ss zzz yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            var tweet = new Tweet
            {
                StatusId = status.Id,
                Name = displayStatus.User.Name,
                ScreenName = displayStatus.User.ScreenName,
                ProfileImageUrl = displayStatus.User.ProfileImageUrl,
                Text = displayStatus.Text,
                MarkupNodes = BuildMarkupNodes(displayStatus.Text, displayStatus.Entities),
                CreatedAt = createdAt,
                TimeAgo = TimeAgo(createdAt),
                IsRetweet = status.Retweeted,
                RetweetedBy = RetweetedBy(status, username),
                RetweetStatusId = (status.RetweetedStatus != null) ? status.RetweetedStatus.Id : String.Empty,
                MediaLinks = status.Entities.Media != null ? status.Entities.Media.Select(m => m.MediaUrl).ToArray() : new string[0],
                IsMyTweet = displayStatus.User.ScreenName == username,
                IsHome = tweetType == TweetClassification.Home,
                IsMention = tweetType == TweetClassification.Mention | isMention,
                IsDirectMessage = tweetType == TweetClassification.DirectMessage | isDirectMessage,
                IsFavorite = tweetType == TweetClassification.Favorite | status.Favorited,
                IsSearch = tweetType == TweetClassification.Search
            };

            return tweet;
        }

        private static string RetweetedBy(Status status, string username)
        {
            if (status.RetweetedStatus != null)
            {
                return username != status.User.ScreenName ? status.User.Name : string.Empty;
            }
            return string.Empty;
        }

        private class MarkupItem
        {
            public string NodeType { get; set; }
            public string Text { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }

        private static MarkupNode[] BuildMarkupNodes(string text, Entities entities)
        {
            var markupItems = new List<MarkupItem>();

            if (entities.Urls != null)
            {
                markupItems.AddRange(entities.Urls.Select(url => new MarkupItem
                {
                    NodeType = "url",
                    Text = url.Url,
                    Start = url.Indices[0],
                    End = url.Indices[1]
                }));
            }

            if (entities.Mentions != null)
            {
                markupItems.AddRange(entities.Mentions.Select(mention => new MarkupItem
                {
                    NodeType = "mention",
                    Text = mention.ScreenName,
                    Start = mention.Indices[0],
                    End = mention.Indices[1]
                }));
            }

            if (entities.HashTags != null)
            {
                markupItems.AddRange(entities.HashTags.Select(hashtag => new MarkupItem
                {
                    NodeType = "hashtag",
                    Text = hashtag.Text,
                    Start = hashtag.Indices[0],
                    End = hashtag.Indices[1]
                }));
            }

            if (entities.Media != null)
            {
                markupItems.AddRange(entities.Media.Select(media => new MarkupItem
                {
                    NodeType = "media",
                    Text = media.Url,
                    Start = media.Indices[0],
                    End = media.Indices[1]
                }));
            }

            var start = 0;
            var nodes = new List<MarkupNode>();
            markupItems.Sort((l, r) => l.Start - r.Start);
            foreach (var item in markupItems)
            {
                if (item.Start >= start) nodes.Add(new MarkupNode("text", text.Substring(start, item.Start - start)));
                nodes.Add(new MarkupNode(item.NodeType, item.Text));
                start = item.End;
            }
            if (start < text.Length) nodes.Add(new MarkupNode("text", text.Substring(start)));
            return nodes.ToArray();
        }

        public static string TimeAgo(this DateTime time)
        {
            var timespan = DateTime.UtcNow - time;
            Func<string, double, string> format = (s, t) => String.Format((string)TranslationService.Instance.Translate(s), (int)t);
            if (timespan.TotalSeconds < 60) return format("time_ago_seconds", timespan.TotalSeconds);
            if (timespan.TotalMinutes < 60) return format("time_ago_minutes", timespan.TotalMinutes);
            if (timespan.TotalHours < 24) return format("time_ago_hours", timespan.TotalHours);
            if (timespan.TotalDays < 3) return format("time_ago_days", timespan.TotalDays);
            return time.ToString((string)TranslationService.Instance.Translate("time_ago_date"));
        }
    }
}