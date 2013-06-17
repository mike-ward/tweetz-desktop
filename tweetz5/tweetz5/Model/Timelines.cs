// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace tweetz5.Model
{
    public interface ITimelines : INotifyPropertyChanged
    {
        void HomeTimeline();
        void MentionsTimeline();
        void DirectMessagesTimeline();
        void FavoritesTimeline();
        void UpdateTimeStamps();
        void UpdateStatus(string[] timelines, Status[] statuses, string tweetType);
        void SwitchTimeline(string timelineName);
        void RemoveStatus(Tweet tweet);
        void ClearSearchTimeline();
        void ClearAllTimelines();
        string TimelineName { get; set; }
        void RemoveTweet(string timelineName, Tweet tweet);
    }

    public class Timelines : ITimelines
    {
        private string _timelineName;
        private ObservableCollection<Tweet> _timeline;
        private readonly Dictionary<string, Timeline> _timelineMap;
        private Timeline _unified { get { return _timelineMap[UnifiedName]; } }
        private Timeline _home { get { return _timelineMap[HomeName]; } }
        private Timeline _mentions { get { return _timelineMap[MentionsName]; } }
        private Timeline _directMessages { get { return _timelineMap[MessagesName]; } }
        private Timeline _favorites { get { return _timelineMap[FavoritesName]; } }
        private Timeline _search { get { return _timelineMap[SearchName]; } }
        private Visibility _searchVisibility;

        public const string UnifiedName = "unified";
        public const string HomeName = "home";
        public const string MentionsName = "mentions";
        public const string MessagesName = "messages";
        public const string FavoritesName = "favorites";
        public const string SearchName = "search";

        public Action<Action> DispatchInvokerOverride { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public Timelines()
        {
            _timelineMap = new Dictionary<string, Timeline>
            {
                {UnifiedName, new Timeline()},
                {HomeName, new Timeline()},
                {MentionsName, new Timeline()},
                {MessagesName, new Timeline()},
                {FavoritesName, new Timeline()},
                {SearchName, new Timeline()}
            };
            SwitchTimeline(UnifiedName);
        }

        public ObservableCollection<Tweet> Timeline
        {
            get { return _timeline; }
            set
            {
                if (_timeline != value)
                {
                    _timeline = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TimelineName
        {
            get { return _timelineName; }
            set
            {
                if (_timelineName != value)
                {
                    _timelineName = value;
                    OnPropertyChanged();
                }
            }
        }

        public void RemoveTweet(string timelineName, Tweet tweet)
        {
            var result =_timelineMap[timelineName].Tweets.Remove(tweet);
        }

        public Visibility SearchVisibility
        {
            get { return _searchVisibility; }
            set
            {
                if (_searchVisibility != value)
                {
                    _searchVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ClearAllTimelines()
        {
            foreach (var timeline in _timelineMap)
            {
                timeline.Value.Clear();
            }
        }

        private static bool UpdateTimelines(Timeline[] timelines, IEnumerable<Status> statuses, string tweetType)
        {
            var updated = false;
            var screenName = string.Empty;
            foreach (var status in statuses.Where(status => timelines.All(timeline => timeline.Tweets.All(t => t.StatusId != status.Id))))
            {
                var createdAt = DateTime.ParseExact(status.CreatedAt, "ddd MMM dd HH:mm:ss zzz yyyy",
                                                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                var displayStatus = status.RetweetedtStatus ?? status;

                // Direct messages don't have a User. Instead, dm's use sender and recipient collections.
                if (displayStatus.User == null)
                {
                    if (string.IsNullOrWhiteSpace(screenName)) screenName = new OAuth().ScreenName;
                    displayStatus.User = (status.Recipient.ScreenName == screenName) ? status.Sender : status.Recipient;
                }

                var tweet = new Tweet
                {
                    StatusId = status.Id,
                    Name = displayStatus.User.Name,
                    ScreenName = displayStatus.User.ScreenName,
                    ProfileImageUrl = displayStatus.User.ProfileImageUrl,
                    Text = displayStatus.Text,
                    MarkupText = MarkupText(displayStatus.Text, displayStatus.Entities),
                    CreatedAt = createdAt,
                    TimeAgo = TimeAgo(createdAt),
                    TweetType = tweetType,
                    Favorited = status.Favorited,
                    IsRetweet = status.Retweeted,
                    RetweetedBy = RetweetedBy(status),
                    RetweetStatusId = (status.RetweetedtStatus != null) ? status.RetweetedtStatus.Id : string.Empty
                };

                foreach (var timeline in timelines.Where(timeline => timeline.Tweets.Any(t => t.StatusId == status.Id) == false))
                {
                    timeline.Tweets.Add(tweet);
                }
                updated = true;
            }
            foreach (var timeline in timelines)
            {
                var i = 0;
                foreach (var item in timeline.Tweets.OrderByDescending(s => s.CreatedAt))
                {
                    timeline.Tweets.Move(timeline.Tweets.IndexOf(item), i++);
                }
            }
            return updated;
        }

        private void PlayNotification()
        {
            if (Application.Current != null)
            {
                MediaCommands.Play.Execute(string.Empty, Application.Current.MainWindow);
            }
        }

        public static string RetweetedBy(Status status)
        {
            if (status.RetweetedtStatus != null)
            {
                var oauth = new OAuth();
                return oauth.ScreenName != status.User.ScreenName ? status.User.Name : string.Empty;
            }
            return string.Empty;
        }

        internal class MarkupItem
        {
            public string Markup { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }

        private static string MarkupText(string text, Entities entities)
        {
            var markupItems = new List<MarkupItem>();

            if (entities.Urls != null)
            {
                markupItems.AddRange(entities.Urls.Select(url => new MarkupItem
                {
                    Markup = string.Format("<a{0}>", url.Url),
                    Start = url.Indices[0],
                    End = url.Indices[1]
                }));
            }

            if (entities.Mentions != null)
            {
                markupItems.AddRange(entities.Mentions.Select(mention => new MarkupItem
                {
                    Markup = string.Format("<m@{0}>", mention.ScreenName),
                    Start = mention.Indices[0],
                    End = mention.Indices[1]
                }));
            }

            if (entities.HashTags != null)
            {
                markupItems.AddRange(entities.HashTags.Select(hashtag => new MarkupItem
                {
                    Markup = string.Format("<h#{0}>", hashtag.Text),
                    Start = hashtag.Indices[0],
                    End = hashtag.Indices[1]
                }));
            }

            if (entities.Media != null)
            {
                markupItems.AddRange(entities.Media.Select(media => new MarkupItem
                {
                    Markup = string.Format("<a{0}>", media.Url),
                    Start = media.Indices[0],
                    End = media.Indices[1]
                }));
            }

            // Sort list so largest start item is first. Filling in the items
            // from the "back" of the text string preserves the indicies.
            markupItems.Sort((l, r) => r.Start - l.Start);

            return markupItems
                .Aggregate(text, (current, markupItem) => current
                                                              .Remove(markupItem.Start, markupItem.End - markupItem.Start)
                                                              .Insert(markupItem.Start, markupItem.Markup));
        }

        private static string TimeAgo(DateTime time)
        {
            var timespan = DateTime.UtcNow - time;
            if (timespan.TotalSeconds < 60) return (int)timespan.TotalSeconds + "s";
            if (timespan.TotalMinutes < 60) return (int)timespan.TotalMinutes + "m";
            if (timespan.TotalHours < 24) return (int)timespan.TotalHours + "h";
            if (timespan.TotalDays < 3) return (int)timespan.TotalDays + "d";
            return time.ToString("MMM d");
        }

        private void DispatchInvoker(Action callback)
        {
            var invoker = DispatchInvokerOverride ?? Application.Current.Dispatcher.Invoke;
            invoker(callback);
        }

        public void SwitchTimeline(string timelineName)
        {
            Timeline timeline;
            if (_timelineMap.TryGetValue(timelineName, out timeline))
            {
                Timeline = timeline.Tweets;
                TimelineName = timelineName;
                SearchVisibility = timelineName == SearchName ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void RemoveStatus(Tweet tweet)
        {
            foreach (var timeline in _timelineMap.Values)
            {
                timeline.Tweets.Remove(tweet);
            }
        }

        public void ClearSearchTimeline()
        {
            _search.Clear();
        }

        private static ulong MaxSinceId(ulong currentSinceId, Status[] statuses)
        {
            return statuses.Length > 0 ? Math.Max(currentSinceId, statuses.Max(s => ulong.Parse(s.Id))) : currentSinceId;
        }

        public void HomeTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.HomeTimeline(_home.SinceId);
            _home.SinceId = MaxSinceId(_home.SinceId, statuses);
            UpdateStatus(new[] { HomeName, UnifiedName }, statuses, "h");
        }

        public void UpdateStatus(string[] timelineNames, Status[] statuses, string tweetType)
        {
            DispatchInvoker(() =>
            {
                var timelines = timelineNames.Select(timeline => _timelineMap[timeline]).ToArray();
                if (UpdateTimelines(timelines, statuses, tweetType))
                {
                    if (timelineNames.Contains(HomeName)) PlayNotification();
                }
            });
        }

        public void MentionsTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.MentionsTimeline(_mentions.SinceId);
            _mentions.SinceId = MaxSinceId(_mentions.SinceId, statuses);
            DispatchInvoker(() =>
                {
                    if (UpdateTimelines(new[] { _mentions, _unified }, statuses, "m")) PlayNotification();
                    foreach (var tweet in _unified.Tweets.Where(h => statuses.Any(s => s.Id == h.StatusId)))
                    {
                        tweet.TweetType += "m";
                    }
                });
        }

        public void FavoritesTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.FavoritesTimeline(_favorites.SinceId);
            _favorites.SinceId = MaxSinceId(_favorites.SinceId, statuses);
            DispatchInvoker(() =>
            {
                UpdateTimelines(new[] {_favorites}, statuses, "f");
                foreach (var tweet in _home.Tweets.Where(t => statuses.Any(s => s.Id == t.StatusId || s.Id == t.RetweetStatusId)))
                {
                    tweet.Favorited = true;
                }
            });
        }

        public void DirectMessagesTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.DirectMessagesTimeline(_directMessages.SinceId);
            _directMessages.SinceId = MaxSinceId(_favorites.SinceId, statuses);
            DispatchInvoker(() =>
                {
                    if (UpdateTimelines(new[] { _directMessages, _unified }, statuses, "d")) PlayNotification();
                    foreach (var tweet in _unified.Tweets.Where(h => statuses.Any(s => s.Id == h.StatusId)))
                    {
                        tweet.TweetType += "d";
                    }
                });
        }

        public void UpdateTimeStamps()
        {
            DispatchInvoker(() =>
                {
                    foreach (var tweet in Timeline)
                    {
                        tweet.TimeAgo = TimeAgo(tweet.CreatedAt);
                    }
                });
        }
    }
}