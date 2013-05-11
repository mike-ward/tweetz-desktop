// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace tweetz5.Model
{
    public interface ITimelines
    {
        void HomeTimeline();
        void MentionsTimeline();
        void UpdateTimeStamps();
        void UpdateStatus(Status[] statuses);
    }

    public class Timelines : ITimelines, INotifyPropertyChanged
    {
        public ObservableCollection<Tweet> Timeline { get; set; }
        public Action<Action> DispatchInvokerOverride { get; set; }
        private readonly ObservableCollection<Tweet> _unified = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _home = new ObservableCollection<Tweet>();
        private readonly ObservableCollection<Tweet> _mentions = new ObservableCollection<Tweet>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Timelines()
        {
            Timeline = _unified;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static void UpdateTimeline(ObservableCollection<Tweet> timeline, IEnumerable<Status> statuses, string tweetType)
        {
            var newStatuses = statuses
                .Where(status => timeline.All(t => t.StatusId != status.Id))
                .ToList();

            if (newStatuses.Count > 0)
            {
                foreach (var status in newStatuses)
                {
                    var createdAt = DateTime.ParseExact(status.CreatedAt, "ddd MMM dd HH:mm:ss zzz yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                    var displayStatus = status.RetweeetedtStatus ?? status;

                    timeline.Add(new Tweet
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
                        RetweetedBy = (status.RetweeetedtStatus != null) ? status.User.Name : string.Empty
                    });
                }
                newStatuses.Clear();
                var sorted = timeline.OrderByDescending(s => s.CreatedAt).ToList();
                for (var i = 0; i < sorted.Count; ++i)
                {
                    timeline.Move(timeline.IndexOf(sorted[i]), i);
                }
            }
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

        public void HomeTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.HomeTimeline();
            UpdateStatus(statuses);
        }

        public void UpdateStatus(Status[] statuses)
        {
            if (statuses.Length > 0)
            {
                DispatchInvoker(() =>
                {
                    UpdateTimeline(_home, statuses, "h");
                    UpdateTimeline(_unified, statuses, "h");
                });
            }            
        }

        public void MentionsTimeline()
        {
            var twitter = new Twitter();
            var statuses = twitter.MentionsTimeline();
            if (statuses.Length > 0)
            {
                DispatchInvoker(() =>
                {
                    UpdateTimeline(_mentions, statuses, "m");
                    UpdateTimeline(_unified, statuses, "m");
                });
            }
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