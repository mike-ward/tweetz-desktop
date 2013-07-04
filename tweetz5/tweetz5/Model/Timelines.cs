// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        void ClearAllTimelines();
        void AddFavorite(Tweet tweet);
        void RemoveFavorite(Tweet tweet);
        void Search(string query);
        void DeleteTweet(Tweet tweet);
        void Retweet(Tweet tweet);
        string TimelineName { get; set; }
    }

    public class Timelines : ITimelines
    {
        private string _timelineName;
        private ObservableCollection<Tweet> _timeline;
        private readonly Dictionary<string, Timeline> _timelineMap;
        private readonly Collection<Tweet> _tweets = new Collection<Tweet>(); 
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

        private bool UpdateTimelines(Timeline[] timelines, IEnumerable<Status> statuses, string tweetType)
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
                    RetweetStatusId = (status.RetweetedtStatus != null) ? status.RetweetedtStatus.Id : string.Empty,
                    MediaLinks = status.Entities.Media != null ? status.Entities.Media.Select(m => m.MediaUrl).ToArray() : new string[0]
                };

                var index = _tweets.IndexOf(tweet);
                if (tweetType != "s")
                {
                    if (index == -1)
                    {
                        _tweets.Add(tweet);
                    }
                    else
                    {
                        tweet = _tweets[index];
                        if (tweet.TweetType.Contains(tweetType) == false) tweet.TweetType += tweetType;
                    }
                }

                foreach (var timeline in timelines.Where(timeline => timeline.Tweets.Any(t => t.StatusId == status.Id) == false))
                {
                    timeline.Tweets.Add(tweet);
                }
                updated = true;
            }
            if (updated)
            {
                foreach (var timeline in timelines)
                {
                    SortTweetCollection(timeline.Tweets);
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
                    Markup = string.Format("<p{0}>", media.Url),
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

        private void RemoveStatus(Tweet tweet)
        {
            foreach (var timeline in _timelineMap.Values)
            {
                timeline.Tweets.Remove(tweet);
            }
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

        public void AddFavorite(Tweet tweet)
        {
            if (tweet.Favorited) return;
            Twitter.CreateFavorite(tweet.StatusId);
            tweet.Favorited = true;
            var index = _tweets.IndexOf(tweet);
            if (index == -1)
            {
                tweet.TweetType += "f";
                _tweets.Add(tweet);
            }
            else
            {
                _tweets[index].TweetType += "f";
                _tweets[index].Favorited = true;
            }
            if (_favorites.Tweets.Contains(tweet) == false)
            {
                _favorites.Tweets.Add(tweet);
                SortTweetCollection(_favorites.Tweets);                
            }
        }

        public void RemoveFavorite(Tweet tweet)
        {
            if (tweet.Favorited == false) return;
            Twitter.DestroyFavorite(tweet.StatusId);
            var index = _tweets.IndexOf(tweet);
            var t = _tweets[index];
            t.Favorited = false;
            t.TweetType = t.TweetType.Replace("f", "");
            _favorites.Tweets.Remove(t);
        }

        private static void SortTweetCollection(ObservableCollection<Tweet> collection)
        {
            var i = 0;
            foreach (var item in collection.OrderByDescending(s => s.CreatedAt))
            {
                // Move will trigger a properychanged event even if the indexes are the same.
                var indexOfItem = collection.IndexOf(item);
                if (indexOfItem != i) collection.Move(indexOfItem, i);
                i += 1;
            }            
        }

        public void Search(string query)
        {
            _search.Clear();
            Task.Run(() =>
            {
                var json = Twitter.Search(query);
                var statuses = SearchStatuses.ParseJson(json);
                UpdateStatus(new[] { SearchName }, statuses, "s");
            });
        }

        public void DeleteTweet(Tweet tweet)
        {
            Twitter.DestroyStatus(tweet.StatusId);
            RemoveStatus(tweet);
        }

        public void Retweet(Tweet tweet)
        {
            if (tweet.IsRetweet)
            {
                var id = string.IsNullOrWhiteSpace(tweet.RetweetStatusId) ? tweet.StatusId : tweet.RetweetStatusId;
                var json = Twitter.GetTweet(id);
                var status = Status.ParseJson("[" + json + "]")[0];
                var retweetStatusId = status.CurrentUserRetweet.Id;
                Twitter.DestroyStatus(retweetStatusId);
                tweet.IsRetweet = false;
            }
            else
            {
                Twitter.RetweetStatus(tweet.StatusId);
                tweet.IsRetweet = true;
            }            
        }
    }
}