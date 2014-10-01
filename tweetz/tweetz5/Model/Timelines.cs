using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using tweetz5.Commands;
using tweetz5.Utilities;

namespace tweetz5.Model
{
    public sealed class Timelines : NotifyPropertyChanged, ITimelines, IDisposable
    {
        private bool _disposed;
        private View _view;
        private RangeObservableCollection<Tweet> _timeline = new RangeObservableCollection<Tweet>();
        private readonly Collection<Tweet> _tweets = new Collection<Tweet>();
        private readonly Collection<Tweet> _search = new Collection<Tweet>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Visibility _searchVisibility = Visibility.Collapsed;
        private bool _isSearching;

        private bool UpdateTimelines(IEnumerable<Status> statuses, TweetClassification tweetType)
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
                    var t = _tweets[index];
                    t.IsHome |= tweet.IsHome;
                    t.IsMention |= tweet.IsMention;
                    t.IsDirectMessage |= tweet.IsDirectMessage;
                    t.IsFavorite |= tweet.IsFavorite;
                    tweet = t;
                }

                if (TimelineFilter(tweet) && _timeline.Contains(tweet) == false)
                {
                    Timeline.Add(tweet);
                    updated = true;
                }
            }
            if (updated) SortTweetCollection(Timeline);
            return updated;
        }

        public void UpdateStatus(IEnumerable<Status> statuses, TweetClassification tweetType)
        {
            DispatchInvoker(() => { if (UpdateTimelines(statuses, tweetType)) PlayNotification(); });
        }

        private readonly Dictionary<View, Predicate<Tweet>> _timelineFilters = new Dictionary<View, Predicate<Tweet>>
        {
            {View.Unified, t => true},
            {View.Home, t => t.IsHome},
            {View.Mentions, t => t.IsMention},
            {View.Messages, t => t.IsDirectMessage},
            {View.Favorites, t => t.IsFavorite},
            {View.Search, t => t.IsSearch},
        };

        private Predicate<Tweet> TimelineFilter
        {
            get
            {
                Predicate<Tweet> filter;
                if (_timelineFilters.TryGetValue(_view, out filter)) return filter;
                return t => false;
            }
        }

        public void SwitchView(View view)
        {
            if (_view == view) return;
            _view = view;
            Timeline.Clear();
            var tweets = (_view == View.Search) ? _search : _tweets;
            SearchVisibility = (_view == View.Search) ? Visibility.Visible : Visibility.Collapsed;
            Timeline.AddRange(tweets.Where(t => TimelineFilter(t)).OrderByDescending(t => t.CreatedAt).Take(200));
        }

        public RangeObservableCollection<Tweet> Timeline
        {
            get { return _timeline; }
            set { SetProperty(ref _timeline, value); }
        }

        public Visibility SearchVisibility
        {
            get { return _searchVisibility; }
            set { SetPropertyValue(ref _searchVisibility, value); }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsSearching
        {
            // ReSharper disable once UnusedMember.Global
            get { return _isSearching; }
            set {  SetPropertyValue(ref _isSearching, value); }
        }

        public void ClearAllTimelines()
        {
            Timeline.Clear();
            _tweets.Clear();
            _search.Clear();
            _homeSinceId = 1;
            _mentionsSinceId = 1;
            _messagesSinceId = 1;
            _favoritesSinceId = 1;
        }

        private static void PlayNotification()
        {
            if (Application.Current != null) ChirpCommand.Command.Execute(string.Empty, Application.Current.MainWindow);
        }

        public Action<Action> DispatchInvokerOverride { private get; set; }

        private void DispatchInvoker(Action callback)
        {
            var invoker = DispatchInvokerOverride ?? (action => Application.Current.Dispatcher.InvokeAsync(action));
            invoker(callback);
        }

        private static ulong MaxSinceId(ulong currentSinceId, ICollection<Status> statuses)
        {
            return (statuses.Count > 0)
                ? Math.Max(currentSinceId, statuses.Max(s => ulong.Parse(s.Id)))
                : currentSinceId;
        }

        private ulong _homeSinceId = 1;

        public async Task UpdateHome()
        {
            var statuses = await Twitter.HomeTimeline(_homeSinceId);
            _homeSinceId = MaxSinceId(_homeSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Home);
        }

        private ulong _mentionsSinceId = 1;

        public async Task UpdateMentions()
        {
            var statuses = await Twitter.MentionsTimeline(_mentionsSinceId);
            _mentionsSinceId = MaxSinceId(_mentionsSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Mention);
        }

        private ulong _favoritesSinceId = 1;

        public async Task UpdateFavorites()
        {
            var statuses = await Twitter.Favorites(_favoritesSinceId);
            _favoritesSinceId = MaxSinceId(_favoritesSinceId, statuses);
            UpdateStatus(statuses, TweetClassification.Favorite);
        }

        private ulong _messagesSinceId = 1;

        public async Task UpdateDirectMessages()
        {
            var recieved = await Twitter.DirectMessages(_messagesSinceId);
            var sent = await Twitter.DirectMessagesSent(_messagesSinceId);
            var statuses = recieved.Concat(sent).ToArray();
            _messagesSinceId = MaxSinceId(_favoritesSinceId, statuses);
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

        public async void AddFavorite(Tweet tweet)
        {
            if (tweet.IsFavorite) return;
            await Twitter.CreateFavorite(tweet.StatusId);
            tweet.IsFavorite = true;
        }

        public async Task RemoveFavorite(Tweet tweet)
        {
            if (tweet.IsFavorite == false) return;
            await Twitter.DestroyFavorite(tweet.StatusId);
            tweet.IsFavorite = false;
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

        public async Task Search(string query)
        {
            try
            {
                IsSearching = true;
                _search.Clear();
                Timeline.Clear();
                var json = await Twitter.Search(query);
                var statuses = SearchStatuses.ParseJson(json);
                foreach (var status in statuses.Where(s => s.RetweetedStatus == null)) _search.Add(status.CreateTweet(TweetClassification.Search));
                Timeline.AddRange(_search);
            }
            finally
            {
                IsSearching = false;
            }
        }

        public async Task DeleteTweet(Tweet tweet)
        {
            await Twitter.DestroyStatus(tweet.StatusId);
            _tweets.Remove(tweet);
            Timeline.Remove(tweet);
        }

        public async Task Retweet(Tweet tweet)
        {
            if (tweet.IsRetweet)
            {
                var id = string.IsNullOrWhiteSpace(tweet.RetweetStatusId) ? tweet.StatusId : tweet.RetweetStatusId;
                var json = Twitter.GetTweet(id);
                var status = Status.ParseJson("[" + json + "]")[0];
                var retweetStatusId = status.CurrentUserRetweet.Id;
                await Twitter.DestroyStatus(retweetStatusId);
                tweet.IsRetweet = false;
            }
            else
            {
                await Twitter.RetweetStatus(tweet.StatusId);
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