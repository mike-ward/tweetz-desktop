// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public class TextBlockMarkup : TextBlock
    {
        public static DependencyProperty MarkupProperty = DependencyProperty.Register("Markup", typeof (string),
            typeof (TextBlockMarkup), new UIPropertyMetadata("Markup", OnMarkupChanged));

        public string Markup
        {
            get { return (string)GetValue(MarkupProperty); }
            set { SetValue(MarkupProperty, value); }
        }

        private static void OnMarkupChanged(DependencyObject sender, DependencyPropertyChangedEventArgs ea)
        {
            var text = (string)ea.NewValue;
            var inlines = new List<object>();
            var textBlock = (TextBlockMarkup)sender;
            var start = 0;
            do
            {
                var index = text.IndexOf("<", start, StringComparison.Ordinal);
                if (index == -1)
                {
                    inlines.Add(Run(text.Substring(start)));
                    break;
                }
                if (index > start)
                {
                    var run = Run(text.Substring(start, index - start));
                    inlines.Add(run);
                }
                var tag = text[++index];
                start = text.IndexOf('>', ++index);
                var tagText = text.Substring(index, start - index);
                switch (tag)
                {
                    case 'a':
                        inlines.Add(Hyperlink("[link]", tagText));
                        break;

                    case 'm':
                        inlines.Add(Mention(tagText));
                        break;

                    case 'h':
                        inlines.Add(Hashtag(tagText));
                        break;
                }
                start += 1;
            } while (start < text.Length);
            textBlock.Inlines.Clear();
            textBlock.Inlines.AddRange(inlines);
            textBlock.UpdateLayout();
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

        private static Hyperlink Hyperlink(string text, string link)
        {
            var hyperlink = new Hyperlink(new Run(text))
            {
                Command = MainWindow.OpenLinkCommand,
                CommandParameter = link,
                ToolTip = link
            };
            ToolTipService.SetInitialShowDelay(hyperlink, 0);
            ToolTipService.SetShowDuration(hyperlink, 30000);
            hyperlink.ToolTipOpening += (s, e) => hyperlink.ToolTip = LongUrl.Lookup(link);
            return hyperlink;
        }

        private static Hyperlink Mention(string text)
        {
            return new Hyperlink(new Run(text))
            {
                Command = MainWindow.ShowUserInformationCommand,
                CommandParameter = text.Replace("@", "")
            };
        }

        private static Hyperlink Hashtag(string text)
        {
            return new Hyperlink(new Run(text))
            {
                Command = MainWindow.SearchCommand,
                CommandParameter = text
            };
        }
    }
}