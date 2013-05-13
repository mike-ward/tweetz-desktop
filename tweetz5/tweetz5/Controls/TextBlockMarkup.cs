// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public class TextBlockMarkup : TextBlock
    {
        public static DependencyProperty MarkupProperty = DependencyProperty.Register("Markup", typeof(string),
            typeof(TextBlockMarkup), new UIPropertyMetadata("Markup", OnMarkupChanged));

        public string Markup
        {
            get { return (string)GetValue(MarkupProperty); }
            set { SetValue(MarkupProperty, value); }
        }

        private static void OnMarkupChanged(DependencyObject sender, DependencyPropertyChangedEventArgs ea)
        {
            var textBlock = (TextBlockMarkup)sender;
            textBlock.Inlines.Clear();
            var text = (string)ea.NewValue;
            var start = 0;
            do
            {
                var index = text.IndexOf("<", start, StringComparison.Ordinal);
                if (index == -1)
                {
                    textBlock.Inlines.Add(Run(text.Substring(start)));
                    break;
                }
                if (index > start)
                {
                    var run = Run(text.Substring(start, index - start));
                    textBlock.Inlines.Add(run);
                }
                var tag = text[++index];
                start = text.IndexOf('>', ++index);
                var tagText = text.Substring(index, start - index);
                switch (tag)
                {
                    case 'a':
                        textBlock.Inlines.Add(Hyperlink("[link]", tagText, OpenExternalLink));
                        break;

                    case 'm':
                        textBlock.Inlines.Add(Mention(tagText));
                        break;

                    case 'h':
                        textBlock.Inlines.Add(Hashtag(tagText));
                        break;
                }
                start += 1;
            } while (start < text.Length);
        }

        private static Run Run(string text)
        {
            return new Run(RemoveLineFeeds(ConvertXmlEntities(text)));
        }

        private static string ConvertXmlEntities(string text)
        {
            return text
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'")
                .Replace("&amp;", "&");
        }

        private static string RemoveLineFeeds(string text)
        {
            return text.Replace("\n", "");
        }

        private static Hyperlink Hyperlink(string text, string link, RequestNavigateEventHandler clickHandler)
        {
            var hyperlink = new Hyperlink(new Run(text));
            if (string.IsNullOrWhiteSpace(link) == false)
            {
                hyperlink.NavigateUri = new Uri(link);
            }
            hyperlink.ToolTip = link;
            ToolTipService.SetInitialShowDelay(hyperlink, 0);
            ToolTipService.SetShowDuration(hyperlink, 30000);
            hyperlink.ToolTipOpening += (s, e) => hyperlink.ToolTip = LongUrl.Lookup(link);
            hyperlink.RequestNavigate += clickHandler;
            return hyperlink;
        }

        private static Span Mention(string text)
        {
            var span = new Span(new Run(text))
            {
                Style = (Style)Application.Current.FindResource("TweetMention")
            };
            return span;
        }

        private static Span Hashtag(string text)
        {
            var span = new Span(new Run(text))
            {
                Style = (Style)Application.Current.FindResource("TweetHashtag")
            };
            return span;            
        }

        private static void OpenExternalLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }
    }
}