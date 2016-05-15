using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace tweetz5.Utilities.System
{
    public static class DragAndDrop
    {
        public static void OnDragOver(object sender, DragEventArgs ea)
        {
            ea.Handled = true;
            ea.Effects = DragDropEffects.None;
            if (ea.Data.GetDataPresent("text/html"))
            {
                ea.Effects = DragDropEffects.Copy;
            }
            else if (ea.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = ea.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames != null && filenames.Length == 1 && IsValidImageExtension(filenames[0]))
                {
                    ea.Effects = DragDropEffects.Copy;
                }
            }
        }

        public static void OnDrop(object sender, DragEventArgs ea)
        {
            if (ea.Data.GetDataPresent("text/html"))
            {
                var html = String.Empty;
                var data = ea.Data.GetData("text/html");
                if (data is string)
                {
                    html = (string) data;
                }
                else if (data is MemoryStream)
                {
                    var stream = (MemoryStream) data;
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    html = (buffer[1] == (byte) 0)
                        ? Encoding.Unicode.GetString(buffer)
                        : Encoding.ASCII.GetString(buffer);
                }
                var match = new Regex(@"<img[^>]+src=""([^""]*)""").Match(html);
                if (match.Success)
                {
                    var uri = new Uri(match.Groups[1].Value);
                    var filename = Path.GetTempFileName();
                    var webClient = new WebClient();
                    try
                    {
                        webClient.DownloadFileCompleted += (o, args) =>
                        {
                            var mainWindow = (MainWindow) Application.Current.MainWindow;
                            mainWindow.Compose.Visibility = Visibility.Visible;
                            mainWindow.Compose.Image = filename;
                            webClient.Dispose();
                        };
                        webClient.DownloadFileAsync(uri, filename);
                        ea.Handled = true;
                    }
                    catch (WebException ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
            }
            else if (ea.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = ea.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames != null && filenames.Length == 1 && IsValidImageExtension(filenames[0]))
                {
                    var mainWindow = (MainWindow) Application.Current.MainWindow;
                    mainWindow.Compose.Visibility = Visibility.Visible;
                    mainWindow.Compose.Image = filenames[0];
                    ea.Handled = true;
                }
            }
        }

        private static bool IsValidImageExtension(string filename)
        {
            var extension = Path.GetExtension(filename) ?? String.Empty;
            var extensions = new[] {".png", ".jpg", ".jpeg", ".gif"};
            return extensions.Any(e => extension.Equals(e, StringComparison.OrdinalIgnoreCase));
        }
    }
}