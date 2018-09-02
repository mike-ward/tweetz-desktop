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
        private DateTime _createdAt;
        private bool _isDirectMessage;
        private bool _isFavorite;
        private bool _isHome;
        private bool _isMention;
        private bool _isSearch;
        private bool _retweet;
        private string _retweetedBy;
        private string _retweetedByScreenName;
        private string _timeAgo;

        public string StatusId { get; set; }
        public string RetweetStatusId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Text { get; set; }
        public MarkupNode[] MarkupNodes { get; set; }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public Media[] MediaLinks { get; set; }
        public Media[] ExtendedMediaLinks { get; set; }
        public string[] Urls { get; set; }
        public bool IsMyTweet { get; set; }

        public bool IsRetweet
        {
            get => _retweet;
            set => SetProperty(ref _retweet, value);
        }

        public string RetweetedBy
        {
            get => _retweetedBy;
            set => SetProperty(ref _retweetedBy, value);
        }

        public string RetweetedByScreenName
        {
            get => _retweetedByScreenName;
            set => SetProperty(ref _retweetedByScreenName, value);
        }

        public string TimeAgo
        {
            get => _timeAgo;
            set => SetProperty(ref _timeAgo, value);
        }

        public bool IsHome
        {
            get => _isHome;
            set => SetProperty(ref _isHome, value);
        }

        public bool IsMention
        {
            get => _isMention;
            set => SetProperty(ref _isMention, value);
        }

        public bool IsDirectMessage
        {
            get => _isDirectMessage;
            set => SetProperty(ref _isDirectMessage, value);
        }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public bool IsSearch
        {
            get => _isSearch;
            set => SetProperty(ref _isSearch, value);
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