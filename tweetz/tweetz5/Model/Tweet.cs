using System;
using tweetz5.Utilities;

namespace tweetz5.Model
{
    public enum TweetClassification
    {
        Home,
        Mention,
        DirectMessage,
        Favorite,
        Search
    }

    public sealed class Tweet : NotifyPropertyChanged, IEquatable<Tweet>
    {
        private string _timeAgo;
        private string _retweetedBy;
        private string _retweetedByScreenName;
        private bool _retweet;
        private bool _isHome;
        private bool _isMention;
        private bool _isDirectMessage;
        private bool _isFavorite;
        private bool _isSearch;

        public string StatusId { get; set; }
        public string RetweetStatusId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Text { get; set; }
        public MarkupNode[] MarkupNodes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] MediaLinks { get; set; }
        public string[] Urls { get; set; }
        public bool IsMyTweet { get; set; }

        public bool IsRetweet
        {
            get { return _retweet; }
            set { SetPropertyValue(ref _retweet, value); }
        }

        public string RetweetedBy
        {
            get { return _retweetedBy; }
            set { SetProperty(ref _retweetedBy, value); }
        }

        public string RetweetedByScreenName
        {
            get { return _retweetedByScreenName; }
            set { SetProperty(ref _retweetedByScreenName, value); }
        }

        public string TimeAgo
        {
            get { return _timeAgo; }
            set { SetProperty(ref _timeAgo, value); }
        }

        public bool IsHome
        {
            get { return _isHome; }
            set { SetPropertyValue(ref _isHome, value); }
        }

        public bool IsMention
        {
            get { return _isMention; }
            set { SetPropertyValue(ref _isMention, value); }
        }

        public bool IsDirectMessage
        {
            get { return _isDirectMessage; }
            set { SetPropertyValue(ref _isDirectMessage, value); }
        }

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { SetPropertyValue(ref _isFavorite, value); }
        }

        public bool IsSearch
        {
            get { return _isSearch; }
            set { SetPropertyValue(ref _isSearch, value); }
        }

        public bool Equals(Tweet other)
        {
            if (other == null) return false;
            if (other.StatusId == StatusId) return true;
            if (other.RetweetStatusId == StatusId) return true;
            if (string.IsNullOrWhiteSpace(other.RetweetStatusId) == false && other.RetweetStatusId == RetweetStatusId) return true;
            return false;
        }
    }
}