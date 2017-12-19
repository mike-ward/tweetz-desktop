using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace tweetz5.Model
{
    public interface ITimelines : INotifyPropertyChanged
    {
        Task UpdateHome();
        Task UpdateMentions();
        Task UpdateDirectMessages();
        Task UpdateFavorites();
        void UpdateTimeStamps();
        void UpdateStatus(IEnumerable<Status> statuses, TweetClassification tweetType);
        void SwitchView(View view);
        void ClearAllTimelines();
        void AddFavorite(Tweet tweet);
        Task RemoveFavorite(Tweet tweet);
        Task Search(string query);
        Task DeleteTweet(Tweet tweet);
        Task Retweet(Tweet tweet);
        void SignalCancel();
        CancellationToken CancellationToken { get; }
        bool ApiIsRunning { get; set; }
    }
}