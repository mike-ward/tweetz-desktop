using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace tweetz5.Model
{
    public interface ITimelines : INotifyPropertyChanged
    {
        Task HomeTimeline();
        Task MentionsTimeline();
        Task DirectMessagesTimeline();
        Task FavoritesTimeline();
        void UpdateTimeStamps();
        void UpdateStatus(IEnumerable<Status> statuses, string tweetType);
        void SwitchTimeline(string name);
        void ClearAllTimelines();
        void AddFavorite(Tweet tweet);
        Task RemoveFavorite(Tweet tweet);
        Task Search(string query);
        Task DeleteTweet(Tweet tweet);
        Task Retweet(Tweet tweet);
        void SignalCancel();
        CancellationToken CancellationToken { get; }
    }
}