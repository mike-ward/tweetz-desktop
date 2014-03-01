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

                    Debug.WriteLine("{ Start Twitter User Stream }");
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
                            stream.BaseStream.ReadTimeout = 60*1000;
                            while (true)
                            {
                                var json = stream.ReadLine();
                                if (json == null) { Debug.WriteLine("{ null }"); break; }
                                if (cancelationToken.IsCancellationRequested) break;
                                Debug.WriteLine(string.IsNullOrWhiteSpace(json) ? "{ Blankline }" : json);

                                var serializer = new JavaScriptSerializer();
                                var reply = serializer.Deserialize<Dictionary<string, object>>(json);
                                if (reply != null && reply.ContainsKey("user"))
                                {
                                    Debug.WriteLine("{ tweet identified }");
                                    var statuses = Status.ParseJson("[" + json + "]");
                                    Application.Current.Dispatcher.InvokeAsync
                                        (() => UpdateStatusHomeTimelineCommand.Command.Execute(statuses, Application.Current.MainWindow));
                                }
                            }
                        }
                    }

                    catch (WebException ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    catch (ArgumentNullException ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    catch (ArgumentException ex)
                    {
                        Debug.WriteLine(ex);
                    }

                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                Debug.WriteLine("{ Stream task ends }");
            }, cancelationToken);
        }
    }
}