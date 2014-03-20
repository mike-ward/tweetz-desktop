using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using tweetz5.Commands;

namespace tweetz5.Model
{
    public static class TwitterStream
    {
        public static void User(CancellationToken cancelationToken)
        {
            Task.Run(() =>
            {
                while (cancelationToken.IsCancellationRequested == false)
                {
                    var delay = Task.Delay(30*1000, cancelationToken);
                    delay.Wait(cancelationToken);
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
                        {
                            using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                            {
                                stream.BaseStream.ReadTimeout = 60*1000;
                                while (true)
                                {
                                    var json = stream.ReadLine();
                                    if (json == null)
                                    {
                                        Trace.TraceInformation("{ null }");
                                        break;
                                    }
                                    if (cancelationToken.IsCancellationRequested) break;
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

                    catch (AggregateException ae)
                    {
                        foreach (var ex in ae.InnerExceptions)
                        {
                            if (ex is WebException ||
                                ex is ArgumentNullException ||
                                ex is ArgumentException ||
                                ex is SocketException ||
                                ex is IOException ||
                                ex is InvalidOperationException)
                            {
                                Trace.TraceError(ex.ToString());
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }

                Trace.TraceInformation("{ Stream task ends }");
            }, cancelationToken);
        }
    }
}