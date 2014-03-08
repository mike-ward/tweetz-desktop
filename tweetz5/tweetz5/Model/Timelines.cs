using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using tweetz5.Commands;
using tweetz5.Utilities;

// ReSharper disable InconsistentNaming

namespace tweetz5.Model
{
    public sealed class Timelines : NotifyPropertyChanged, ITimelines, IDisposable
    {
        private bool _disposed;
        private string _timelineName;
        private ObservableCollection<Tweet> _timeline;
        private readonly Dictionary<string, Timeline> _timelineMap;
        private readonly Collection<Tweet> _tweets = new Collection<Tweet>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Timeline _unified
        {
            get { return _timelineMap[UnifiedName]; }
        }

        private Timeline _home
        {
            get { return _timelineMap[HomeName]; }
        }

        private Timeline _mentions
        {
            get { return _timelineMap[MentionsName]; }
        }

        private Timeline _directMessages
        {
            get { return _timelineMap[MessagesName]; }
        }

        private Timeline _favorites
        {
            get { return _timelineMap[FavoritesName]; }
        }

        private Timeline _search
        {
            get { return _timelineMap[SearchName]; }
        }

        private Visibility _searchVisibility = Visibility.Collapsed;

        public const string UnifiedName = "unified";
        public const string HomeName = "home";
        public const string MentionsName = "mentions";
        public const string MessagesName = "messages";
        public const string FavoritesName = "favorites";
        public const string SearchName = "search";

        public Action<Action> DispatchInvokerOverride { private get; set; }

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
        }

        public ObservableCollection<Tweet> Timeline
        {
            get { return _timeline; }
            set { SetProperty(ref _timeline, value); }
        }

        private string TimelineName
        {
            set { SetProperty(ref _timelineName, value); }
        }

        public Visibility SearchVisibility
        {
            get { return _searchVisibility; }
            set { SetPropertyValue(ref _searchVisibility, value); }
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
            var screenName = new OAuth().ScreenName;
            foreach (var status in statuses)
            {
                var tweet = TweetUtilities.CreateTweet(tweetType, status);

                if (tweetType != "s") // serach results not added to tweet collection
                {
                    var index = _tweets.IndexOf(tweet);
                    if (index == -1)
                    {
                        _tweets.Add(tweet);
                        updated = true;
                    }
                    else
                    {
                        tweet = _tweets[index];
                    }

                    AddTweetType(tweetType, tweet);

                    if (status.Entities != null
                        && status.Entities.Mentions != null
                        && status.Entities.Mentions.Any(m => m.ScreenName == screenName))
                    {
                        // mentions can appear in the home timeline before the mentions timeline.
                        // Check so it can be highlighted sooner.
                        AddTweetType("m", tweet);
                    }
                }

                foreach (var timeline in timelines.Where(timeline => timeline.Tweets.IndexOf(tweet) == -1))
                {
                    timeline.Tweets.Add(tweet);
                }
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

        private static void AddTweetType(string tweetType, Tweet tweet)
        {
            if (tweet.TweetType.Contains(tweetType) == false) tweet.TweetType += tweetType;
        }

        private static void PlayNotification()
        {
            if (Application.Current != null)
            {
                ChirpCommand.Command.Execute(string.Empty, Application.Current.MainWindow);
            }
        }

        private void DispatchInvoker(Action callback)
        {
            var invoker = DispatchInvokerOverride ?? (action => Application.Current.Dispatcher.InvokeAsync(callback));
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

        private static ulong MaxSinceId(ulong currentSinceId, ICollection<Status> statuses)
        {
            return (statuses.Count > 0)
                ? Math.Max(currentSinceId, statuses.Max(s => ulong.Parse(s.Id)))
                : currentSinceId;
        }

        public void HomeTimeline()
        {
            var statuses = Twitter.HomeTimeline(_home.SinceId);
            _home.SinceId = MaxSinceId(_home.SinceId, statuses);
            UpdateStatus(new[] {HomeName, UnifiedName}, statuses, "h");
        }

        public void UpdateStatus(string[] timelineNames, IEnumerable<Status> statuses, string tweetType)
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
            var statuses = Twitter.MentionsTimeline(_mentions.SinceId);
            _mentions.SinceId = MaxSinceId(_mentions.SinceId, statuses);
            DispatchInvoker(() =>
            {
                if (UpdateTimelines(new[] {_mentions, _unified}, statuses, "m")) PlayNotification();
                foreach (var tweet in _unified.Tweets.Where(h => statuses.Any(s => s.Id == h.StatusId)))
                {
                    tweet.TweetType += "m";
                }
            });
        }

        public void FavoritesTimeline()
        {
            var statuses = Twitter.FavoritesTimeline(_favorites.SinceId);
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
            var statuses = Twitter.DirectMessagesTimeline(_directMessages.SinceId);
            _directMessages.SinceId = MaxSinceId(_favorites.SinceId, statuses);
            DispatchInvoker(() =>
            {
                if (UpdateTimelines(new[] {_directMessages, _unified}, statuses, "d")) PlayNotification();
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
                if (Timeline == null) return;
                foreach (var tweet in Timeline)
                {
                    tweet.TimeAgo = TweetUtilities.TimeAgo(tweet.CreatedAt);
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
                var json = Twitter.Search(query + "+exclude:retweets");
                var statuses = SearchStatuses.ParseJson(json);
                UpdateStatus(new[] {SearchName}, statuses, "s");
            }, CancellationToken);
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

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        public void SignalCancel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_cancellationTokenSource == null) return;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}