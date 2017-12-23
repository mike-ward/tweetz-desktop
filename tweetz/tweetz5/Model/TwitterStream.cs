using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using tweetz5.Commands;

namespace tweetz5.Model
{
    public class TwitterStream : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Run()
        {
            var token = _cancellationTokenSource.Token;

            Task.Run(() =>
            {
                while (_cancellationTokenSource.IsCancellationRequested == false)
                {
                    var delay = Task.Delay(30 * 1000, token);
                    delay.Wait(token);
                    if (delay.IsCanceled || delay.IsFaulted) break;

                    Trace.TraceInformation("{ Start Twitter User Stream }");
                    const string url = "https://userstream.twitter.com/1.1/user.json";
                    var oauth = new OAuth();
                    var nonce = OAuth.Nonce();
                    var timestamp = OAuth.TimeStamp();
                    var signature = OAuth.Signature("GET", url, nonce, timestamp, oauth.AccessToken, oauth.AccessTokenSecret, null);
                    var authorizeHeader = OAuth.AuthorizationHeader(nonce, timestamp, oauth.AccessToken, signature);

                    var request = WebRequestWrapper.Create(new Uri(url));
                    request.Headers.Add("Authorization", authorizeHeader);

                    try
                    {
                        using (var response = request.GetResponse())
                        using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            stream.BaseStream.ReadTimeout = 60 * 1000;
                            while (true)
                            {
                                var json = stream.ReadLine();
                                if (json == null)
                                {
                                    Trace.TraceInformation("{ null }");
                                    break;
                                }

                                if (_cancellationTokenSource.IsCancellationRequested) break;
                                Trace.TraceInformation(string.IsNullOrWhiteSpace(json) ? "{ Blankline }" : json);

                                var serializer = new JavaScriptSerializer();
                                var reply = serializer.Deserialize<Dictionary<string, object>>(json);
                                if (reply != null && reply.ContainsKey("user"))
                                {
                                    Trace.TraceInformation("{ tweet identified }");
                                    var statuses = Status.ParseJson("[" + json + "]");
                                    Application.Current.Dispatcher.InvokeAsync
                                        (() => UpdateStatusHomeTimelineCommand.Command.Execute(statuses, Application.Current.MainWindow));
                                }
                            }
                        }
                    }

                    catch (WebException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }

                    catch (ArgumentNullException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }

                    catch (ArgumentException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }

                    catch (InvalidOperationException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }

                    catch (IOException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
                }

                Trace.TraceInformation("{ Stop Twitter User Stream }");
            }, token);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}