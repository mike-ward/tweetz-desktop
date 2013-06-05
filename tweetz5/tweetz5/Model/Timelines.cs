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
    public interface ITimelines
    {
        void HomeTimeline();
        void MentionsTimeline();
        void DirectMessagesTimeline();
        void FavoritesTimeline();
        void UpdateTimeStamps();
        void UpdateStatus(Status[] statuses);
        void SwitchTimeline(string timeline);
    }

    public class Timelines : ITimelines, INotifyPropertyChanged
    {
        public Action<Action> DispatchInvokerOverride { get; set; }
        private ObservableCollection<Tweet> _timeline;
        private readonly ObservableCollection<Tweet> _unified = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _home = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _mentions = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _directMessages = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _favorites = new ObservableCollection<Tweet>();
        private readonly Dictionary<string, ObservableCollection<Tweet>> _timelineMap;

        public event PropertyChangedEventHandler PropertyChanged;

        public Timelines()
        {
            Timeline = _unified;
            _timelineMap = new Dictionary<string, ObservableCollection<Tweet>>
            {
                {"unified", _unified},
                {"home", _home},
                {"mentions", _mentions},
                {"messages", _directMessages},
                {"favorites", _favorites}
            };
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static bool UpdateTimeline(ObservableCollection<Tweet>[] timelines, IEnumerable<Status> statuses, string tweetType)
        {
            var updated = false;
            var screenName = string.Empty;
            foreach (var status in statuses.Where(status => timelines.All(timeline => timeline.All(t => t.StatusId != status.Id))))
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
                        RetweetedBy = RetweetedBy(status)
                    };

                foreach (var timeline in timelines.Where(timeline => timeline.Any(t => t.StatusId == status.Id) == false))
                {
                    timeline.Add(tweet);
                }
                updated = true;
            }
            foreach (var timeline in timelines)
            {
                var i = 0;
                foreach (var item in timeline.OrderByDescending(s => s.CreatedAt))
                {
                    timeline.Move(timeline.IndexOf(item), i++);
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
            if (status.Retweeted) return "you";
            if (status.RetweetedtStatus != null) return status.User.Name;
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
            ObservableCollection<Tweet> timeline;
            if (_timelineMap.TryGetValue(timelineName, out timeline))
            {
                Timeline = timeline;
            }
        }

        public void HomeTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.HomeTimeline();
            UpdateStatus(statuses);
        }

        public void UpdateStatus(Status[] statuses)
        {
            DispatchInvoker(() =>
            {
                if (UpdateTimeline(new[] { _home, _unified }, statuses, "h")) PlayNotification();
            });
        }

        public void MentionsTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.MentionsTimeline();
            DispatchInvoker(() =>
            {
                if (UpdateTimeline(new[] { _mentions, _unified }, statuses, "m")) PlayNotification();
                foreach (var tweet in _unified.Where(h => statuses.Any(s => s.Id == h.StatusId)))
                {
                    tweet.TweetType += "m";
                }
            });
        }

        public void FavoritesTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.FavoritesTimeline();
            DispatchInvoker(() => UpdateTimeline(new[] { _favorites }, statuses, "f"));
        }

        public void DirectMessagesTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.DirectMessagesTimeline();
            DispatchInvoker(() =>
            {
                if (UpdateTimeline(new[] { _directMessages, _unified }, statuses, "d")) PlayNotification();
                foreach (var tweet in _unified.Where(h => statuses.Any(s => s.Id == h.StatusId)))
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