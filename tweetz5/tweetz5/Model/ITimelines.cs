using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace tweetz5.Model
{
    public interface ITimelines : INotifyPropertyChanged
    {
        void HomeTimeline();
        void MentionsTimeline();
        void DirectMessagesTimeline();
        void FavoritesTimeline();
        void UpdateTimeStamps();
        void UpdateStatus(IEnumerable<Status> statuses, string tweetType);
        void SwitchTimeline(string name);
        void ClearAllTimelines();
        void AddFavorite(Tweet tweet);
        void RemoveFavorite(Tweet tweet);
        void Search(string query);
        void DeleteTweet(Tweet tweet);
        void Retweet(Tweet tweet);
        void SignalCancel();
        CancellationToken CancellationToken { get; }
    }
}