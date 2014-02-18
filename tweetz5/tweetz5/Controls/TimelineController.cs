// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using tweetz5.Model;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Controls
{
    public sealed class TimelineController : IDisposable
    {
        private Timers _timers = new Timers();
        private readonly ITimelines _timelinesModel;

        public TimelineController(ITimelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
        }

        public void StartTimelines()
        {

            _timers.Add(Settings.Default.UseStreamingApi ? 180 : 70, (s, e) =>
            {
                try
                {
                    _timelinesModel.HomeTimeline();
                    _timelinesModel.MentionsTimeline();
                    _timelinesModel.DirectMessagesTimeline();
                    _timelinesModel.FavoritesTimeline();
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex);
                }
            });

            _timers.Add(30, (s, e) => _timelinesModel.UpdateTimeStamps());

            if (Settings.Default.UseStreamingApi)
            {
                TwitterStream.User(_timelinesModel.CancellationToken);
            }
        }

        public void StopTimelines()
        {
            _timers.Dispose();
            _timers = new Timers();
            _timelinesModel.SignalCancel();
            _timelinesModel.ClearAllTimelines();
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timers.Dispose();
        }

        public static void CopyTweetToClipboard(Tweet tweet)
        {
            Clipboard.SetText(tweet.Text);
        }

        public static void CopyLinkToClipboard(Tweet tweet)
        {
            Clipboard.SetText(TweetLink(tweet));
        }

        public static string TweetLink(Tweet tweet)
        {
            return string.Format("https://twitter.com/{0}/status/{1}", tweet.ScreenName, tweet.StatusId);
        }

        public void UpdateStatus(string[] timelines, IEnumerable<Status> statuses, string tweetType)
        {
            _timelinesModel.UpdateStatus(timelines, statuses, tweetType);
        }

        public void SwitchTimeline(string timelineName)
        {
            _timelinesModel.SwitchTimeline(timelineName);
        }

        public void DeleteTweet(Tweet tweet)
        {
            _timelinesModel.DeleteTweet(tweet);
        }

        public void AddFavorite(Tweet tweet)
        {
            _timelinesModel.AddFavorite(tweet);
        }

        public void RemoveFavorite(Tweet tweet)
        {
            _timelinesModel.RemoveFavorite(tweet);
        }

        public void Search(string query)
        {
            _timelinesModel.Search(query);
        }

        public void Retweet(Tweet tweet)
        {
            _timelinesModel.Retweet(tweet);
        }
    }
}