// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using tweetz5.Model;
using tweetz5.Utilities.System;

namespace tweetz5.Controls
{
    public class TimelineController : IDisposable
    {
        private readonly ITimelines _timelinesModel;
        private ITimer _checkTimelines;
        private ITimer _updateTimeStamps;

        public TimelineController(ITimelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
        }

        public void StartTimelines()
        {
            _checkTimelines = SysTimer.Factory();
            _checkTimelines.Interval = 100;
            var firstTime = true;
            _checkTimelines.Elapsed += (s, e) =>
                {
                    try
                    {
                        _checkTimelines.Interval = 60000;
                        _timelinesModel.HomeTimeline();
                        _timelinesModel.MentionsTimeline();
                        _timelinesModel.DirectMessagesTimeline();
                        _timelinesModel.FavoritesTimeline();

                        if (firstTime)
                        {
                            firstTime = false;
                            if (Application.Current != null)
                            {
                                Application.Current.Dispatcher.Invoke(
                                    () => Commands.SwitchTimelinesCommand.Execute(Timelines.UnifiedName, Application.Current.MainWindow));
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        // Offline, authorization error, exceeded rate limit, etc.
                        Console.WriteLine(ex);
                    }
                };
            _checkTimelines.Start();

            _updateTimeStamps = SysTimer.Factory();
            _updateTimeStamps.Interval = 30000;
            _updateTimeStamps.Elapsed += (s, e) => _timelinesModel.UpdateTimeStamps();
            _updateTimeStamps.Start();
            TwitterStream.User(_timelinesModel.CancellationToken);
        }

        public void StopTimelines()
        {
            if (_checkTimelines != null)
            {
                _checkTimelines.Dispose();
                _checkTimelines = null;
            }
            if (_updateTimeStamps != null)
            {
                _updateTimeStamps.Dispose();
                _updateTimeStamps = null;
            }
            _timelinesModel.SignalCancel();
            _timelinesModel.ClearAllTimelines();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                if (_checkTimelines != null)
                {
                    _checkTimelines.Dispose();
                    _checkTimelines = null;
                }
                if (_updateTimeStamps != null)
                {
                    _updateTimeStamps.Dispose();
                    _updateTimeStamps = null;
                }
            }
        }

        public void CopyTweetToClipboard(Tweet tweet)
        {
            Clipboard.SetText(tweet.Text);
        }

        public void CopyLinkToClipboard(Tweet tweet)
        {
            Clipboard.SetText(TweetLink(tweet));
        }

        public static string TweetLink(Tweet tweet)
        {
            return string.Format("https://twitter.com/{0}/status/{1}", tweet.ScreenName, tweet.StatusId);
        }

        public void UpdateStatus(string[] timelines, Status[] statuses, string tweetType)
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

        public IList<string> ScreenNames
        {
            get { return _timelinesModel.ScreenNames; }
        }
    }
}