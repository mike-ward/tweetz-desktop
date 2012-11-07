// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace tweetz5.Model
{
    public class Tweet : INotifyPropertyChanged
    {
        private string _timeAgo;

        public string StatusId { get; set; }
        public string Name { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Text { get; set; }
        public string MarkupText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TweetType { get; set; }

        public string TimeAgo
        {
            get { return _timeAgo; }
            set
            {
                if (TimeAgo != value)
                {
                    _timeAgo = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [DataContract]
    public class Status
    {
        [DataMember(Name = "id_str")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name="entities")]
        public Entities Entities { get; set; }

        public static Status[] ParseJson(string json)
        {
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(Status[]));
                return (Status[]) serializer.ReadObject(stream);
            }
        }
    }

    [DataContract]
    public class User
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrl { get; set; }
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
    }

    [DataContract]
    public class UrlEntity
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "diplay_url")]
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
}