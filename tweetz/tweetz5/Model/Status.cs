using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace tweetz5.Model
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "id_str")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "recipient")]
        public User Recipient { get; set; }

        [DataMember(Name = "sender")]
        public User Sender { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; set; }

        [DataMember(Name = "retweeted_status")]
        public Status RetweetedStatus { get; set; }

        [DataMember(Name = "favorited")]
        public bool Favorited { get; set; }

        [DataMember(Name = "retweeted")]
        public bool Retweeted { get; set; }

        [DataMember(Name = "current_user_retweet")]
        public CurrentUserRetweet CurrentUserRetweet { get; set; }

        public static Status[] ParseJson(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof (Status[]));
                return (Status[])serializer.ReadObject(stream);
            }
        }
    }

    [DataContract]
    public class SearchStatuses
    {
        [DataMember(Name = "statuses")]
        public Status[] Statuses { get; set; }

        public static IEnumerable<Status> ParseJson(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof (SearchStatuses));
                var searchStatuses = (SearchStatuses)serializer.ReadObject(stream);
                return searchStatuses.Statuses;
            }
        }
    }

    [DataContract]
    public sealed class User : INotifyPropertyChanged
    {
        private bool _following;
        private bool _followedBy;

        [DataMember(Name = "id")]
        public ulong Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "description")]
        [DefaultValue("(not specified)")]
        public string Description { get; set; }

        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        [DataMember(Name = "location")]
        [DefaultValue("(not specified)")]
        public string Location { get; set; }

        [DataMember(Name = "url")]
        [DefaultValue("(not specified)")]
        public string Url { get; set; }

        [DataMember(Name = "statuses_count")]
        public int Tweets { get; set; }

        [DataMember(Name = "friends_count")]
        public int Friends { get; set; }

        [DataMember(Name = "followers_count")]
        public int Followers { get; set; }

        [DataMember(Name = "entities")]
        public UserObjectEntities Entities { get; set; }

        public bool Following
        {
            get { return _following; }
            set
            {
                if (_following == value) return;
                _following = value;
                OnPropertyChanged();
            }
        }

        public bool FollowedBy
        {
            // ReSharper disable once UnusedMember.Global
            get { return _followedBy; }
            set
            {
                if (_followedBy == value) return;
                _followedBy = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [DataContract]
    public class UserObjectEntities
    {
        [DataMember(Name = "url")]
        public UserObjectUrls Url { get; set; }
    }

    [DataContract]
    public class UserObjectUrls
    {
        [DataMember(Name = "urls")]
        public UrlEntity[] Urls { get; set; }
    }

    [DataContract]
    public class Entities
    {
        [DataMember(Name = "urls")]
        public UrlEntity[] Urls { get; set; }

        [DataMember(Name = "user_mentions")]
        public MentionEntity[] Mentions { get; set; }

        [DataMember(Name = "hashtags")]
        public HashTagEntity[] HashTags { get; set; }

        [DataMember(Name = "media")]
        public Media[] Media { get; set; }
    }

    [DataContract]
    public class UrlEntity
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "display_url")]
        public string DisplayUrl { get; set; }

        [DataMember(Name = "expanded_url")]
        public string ExpandedUrl { get; set; }

        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }
    }

    [DataContract]
    public class MentionEntity
    {
        [DataMember(Name = "id_str")]
        public string Id { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }
    }

    [DataContract]
    public class HashTagEntity
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }
    }

    [DataContract]
    public class Media
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "display_url")]
        public string DisplayUrl { get; set; }

        [DataMember(Name = "expanded_url")]
        public string ExpandedUrl { get; set; }

        [DataMember(Name = "media_url")]
        public string MediaUrl { get; set; }

        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }
    }

    [DataContract]
    public class CurrentUserRetweet
    {
        [DataMember(Name = "id_str")] public string Id;
    }

    public class Friendship
    {
        public bool Following { get; set; }
        public bool FollowedBy { get; set; }
    }
}