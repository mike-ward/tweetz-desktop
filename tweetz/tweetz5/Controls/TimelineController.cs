using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using tweetz5.Model;
using tweetz5.Utilities.ExceptionHandling;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Controls
{
    public sealed class TimelineController : IDisposable
    {
        private readonly ITimelines _timelinesModel;

        private bool _disposed;
        private Timers _timers = new Timers();

        public TimelineController(ITimelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timers.Dispose();
            _timers = null;
        }

        public void StartTimelines()
        {
            _timers.Add(Settings.Default.UseStreamingApi ? 180 : 70, (s, e) =>
            {
                try
                {
                    _timelinesModel.UpdateHome();
                    _timelinesModel.UpdateMentions();
                    _timelinesModel.UpdateDirectMessages();
                    _timelinesModel.UpdateFavorites();
                }
                catch (WebException ex)
                {
                    Trace.TraceError(ex.Message);
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

        public static void CopyTweetToClipboard(Tweet tweet)
        {
            Clipboard.SetText(tweet.Text);
        }

        public static void CopyLinkToClipboard(Tweet tweet)
        {
            Clipboard.SetText(TweetLink(tweet));
        }

        public static string TweetLink(Tweet tweet) => $"https://twitter.com/{tweet.ScreenName}/status/{tweet.StatusId}";

        public void UpdateStatus(IEnumerable<Status> statuses, TweetClassification tweetType)
        {
            _timelinesModel.UpdateStatus(statuses, tweetType);
        }

        public void SwitchTimeline(View view)
        {
            _timelinesModel.SwitchView(view);
        }

        public void DeleteTweet(Tweet tweet)
        {
            _timelinesModel.DeleteTweet(tweet).LogAggregateExceptions();
        }

        public void AddFavorite(Tweet tweet)
        {
            _timelinesModel.AddFavorite(tweet);
        }

        public void RemoveFavorite(Tweet tweet)
        {
            _timelinesModel.RemoveFavorite(tweet).LogAggregateExceptions();
        }

        public void Search(string query)
        {
            _timelinesModel.Search(query).LogAggregateExceptions();
        }

        public void Retweet(Tweet tweet)
        {
            _timelinesModel.Retweet(tweet).LogAggregateExceptions();
        }
    }
}