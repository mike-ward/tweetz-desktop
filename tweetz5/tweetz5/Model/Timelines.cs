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
        private ObservableCollection<Tweet> _timeline = new ObservableCollection<Tweet>();
        private readonly Collection<Tweet> _tweets = new Collection<Tweet>();
        private readonly Collection<Tweet> _search = new Collection<Tweet>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Visibility _searchVisibility = Visibility.Collapsed;

        public const string UnifiedName = "unified";
        public const string HomeName = "home";
        public const string MentionsName = "mentions";
        public const string MessagesName = "messages";
        public const string FavoritesName = "favorites";
        public const string SearchName = "search";

        public Action<Action> DispatchInvokerOverride { private get; set; }

        public ObservableCollection<Tweet> Timeline
        {
            get { return _timeline; }
            set { SetProperty(ref _timeline, value); }
        }

        private string TimelineName
        {
            get { return _timelineName; }
            set { SetProperty(ref _timelineName, value); }
        }

        public Visibility SearchVisibility
        {
            get { return _searchVisibility; }
            set { SetPropertyValue(ref _searchVisibility, value); }
        }

        public void ClearAllTimelines()
        {
            _timeline.Clear();
        }

        private bool UpdateTimelines(IEnumerable<Status> statuses, string tweetType)
        {
            var updated = false;
            foreach (var status in statuses)
            {
                var tweet = status.CreateTweet(tweetType);
                var index = _tweets.IndexOf(tweet);
                if (index == -1)
                {
                    _tweets.Add(tweet);
                }
                else
                {
                    var tweetTypes = tweet.TweetTypes;
                    tweet = _tweets[index];
                    tweet.AddTweetTypes(tweetTypes);
                }

                if (CurrentTimelineFilter(tweet) && _timeline.Contains(tweet) == false)
                {
                    _timeline.Add(tweet);
                    updated = true;
                }
            }
            if (updated) SortTweetCollection(_timeline);
            return updated;
        }

        private readonly Dictionary<string, Predicate<Tweet>> _timelineFilters = new Dictionary<string, Predicate<Tweet>>
        {
            {UnifiedName, t => t.TweetTypes.Contains(TweetClassification.Search) == false},
            {HomeName, t => t.TweetTypes.Contains(TweetClassification.Home)},
            {MentionsName, t => t.TweetTypes.Contains(TweetClassification.Mention)},
            {MessagesName, t => t.TweetTypes.Contains(TweetClassification.DirectMessage)},
            {FavoritesName, t => t.TweetTypes.Contains(TweetClassification.Favorite)},
            {SearchName, t => t.TweetTypes.Contains(TweetClassification.Search)},
        };

        private Predicate<Tweet> CurrentTimelineFilter
        {
            get
            {
                Predicate<Tweet> filter;
                if (_timelineFilters.TryGetValue(TimelineName, out filter)) return filter;
                return t => false;
            }
        }

        public void SwitchTimeline(string name)
        {
            if (TimelineName == name) return;
            Timeline.Clear();
            TimelineName = name;
            var tweets = (TimelineName == SearchName) ? _search : _tweets;
            foreach (var tweet in tweets.Where(t => CurrentTimelineFilter(t)).OrderByDescending(t => t.CreatedAt).Take(200)) Timeline.Add(tweet);
            SearchVisibility = (TimelineName == SearchName) ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void PlayNotification()
        {
            if (Application.Current != null) ChirpCommand.Command.Execute(string.Empty, Application.Current.MainWindow);
        }

        private void DispatchInvoker(Action callback)
        {
            var invoker = DispatchInvokerOverride ?? (action => Application.Current.Dispatcher.InvokeAsync(callback));
            invoker(callback);
        }

        private void RemoveStatus(Tweet tweet)
        {
            _tweets.Remove(tweet);
            Timeline.Remove(tweet);
        }

        private static ulong MaxSinceId(ulong currentSinceId, ICollection<Status> statuses)
        {
            return (statuses.Count > 0)
                ? Math.Max(currentSinceId, statuses.Max(s => ulong.Parse(s.Id)))
                : currentSinceId;
        }

        public void UpdateStatus(IEnumerable<Status> statuses, string tweetType)
        {
            DispatchInvoker(() => { if (UpdateTimelines(statuses, tweetType)) PlayNotification(); });
        }

        private ulong _homeSinceId = 1;

        public void HomeTimeline()
        {
            var statuses = Twitter.HomeTimeline(_homeSinceId);
            _homeSinceId = MaxSinceId(_homeSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Home);
        }

        private ulong _mentionsSinceId = 1;

        public void MentionsTimeline()
        {
            var statuses = Twitter.MentionsTimeline(_mentionsSinceId);
            _mentionsSinceId = MaxSinceId(_mentionsSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Mention);
        }

        private ulong _favoritesSinceId = 1;

        public void FavoritesTimeline()
        {
            var statuses = Twitter.FavoritesTimeline(_favoritesSinceId);
            _favoritesSinceId = MaxSinceId(_favoritesSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Favorite);
        }

        private ulong _directMessagesSinceId = 1;

        public void DirectMessagesTimeline()
        {
            var statuses = Twitter.DirectMessagesTimeline(_directMessagesSinceId)
                .Concat(Twitter.DirectMessagesSentTimeline(_directMessagesSinceId))
                .ToArray();
            _directMessagesSinceId = MaxSinceId(_favoritesSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.DirectMessage);
        }

        public void UpdateTimeStamps()
        {
            DispatchInvoker(() =>
            {
                if (Timeline == null) return;
                foreach (var tweet in Timeline) tweet.TimeAgo = tweet.CreatedAt.TimeAgo();
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
                tweet.AddTweetTypes(TweetClassification.Favorite);
                _tweets.Add(tweet);
            }
            else
            {
                _tweets[index].AddTweetTypes(TweetClassification.Favorite);
                _tweets[index].Favorited = true;
            }
        }

        public void RemoveFavorite(Tweet tweet)
        {
            if (tweet.Favorited == false) return;
            Twitter.DestroyFavorite(tweet.StatusId);
            var index = _tweets.IndexOf(tweet);
            var t = _tweets[index];
            t.Favorited = false;
            t.TweetTypes = t.TweetTypes.Replace(TweetClassification.Favorite, "");
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
            Task.Run(() =>
            {
                var json = Twitter.Search(query + "+exclude:retweets");
                var statuses = SearchStatuses.ParseJson(json);
                _search.Clear();
                foreach (var status in statuses) _search.Add(status.CreateTweet(TweetClassification.Search));
                DispatchInvoker(() =>
                {
                    _timeline.Clear();
                    foreach (var tweet in _search) _timeline.Add(tweet);
                });
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