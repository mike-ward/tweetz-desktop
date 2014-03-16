using System;
using System.Linq;
using tweetz5.Utilities;

namespace tweetz5.Model
{
    public static class TweetClassification
    {
        public const string Home = "h";
        public const string Mention = "m";
        public const string DirectMessage = "d";
        public const string Search = "s";
        public const string Favorite = "f";
    }

    public sealed class Tweet : NotifyPropertyChanged, IEquatable<Tweet>
    {
        private bool _favorited;
        private string _timeAgo;
        private string _tweetTypes;
        private string _retweetedBy;
        private bool _retweet;

        public string StatusId { get; set; }
        public string RetweetStatusId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Text { get; set; }
        public MarkupNode[] MarkupNodes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] MediaLinks { get; set; }
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

        public string TweetTypes
        {
            get { return _tweetTypes; }
            set { SetProperty(ref _tweetTypes, value); }
        }

        public string TimeAgo
        {
            get { return _timeAgo; }
            set { SetProperty(ref _timeAgo, value); }
        }

        public bool Favorited
        {
            get { return _favorited; }
            set { SetPropertyValue(ref _favorited, value); }
        }

        public bool IsDirectMesssage
        {
            get { return TweetTypes.Contains(TweetClassification.DirectMessage); }
        }

        public void AddTweetTypes(string tweetTypes)
        {
            foreach (var tt in tweetTypes.Where(c => TweetTypes.IndexOf(c) == -1)) TweetTypes += tt;
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