using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public class MarkupService
    {
        public static readonly DependencyProperty MarkupNodesProperty = DependencyProperty.RegisterAttached(
            "MarkupNodes",
            typeof (IList<MarkupNode>),
            typeof (MarkupService),
            new PropertyMetadata(null, OnTextChanged)
            );

        public static IList<MarkupNode> GetMarkupNodes(DependencyObject d)
        {
            return d.GetValue(MarkupNodesProperty) as IList<MarkupNode>;
        }

        public static void SetMarkupNodes(DependencyObject d, IList<MarkupNode> value)
        {
            d.SetValue(MarkupNodesProperty, value);
        }

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs ea)
        {
            MarkupToContent(sender as TextBlock, ea.NewValue as IList<MarkupNode>);
        }

        public static void MarkupToContent(TextBlock textBlock, IList<MarkupNode> nodes)
        {
            var inlines = new List<object>();
            foreach (var node in nodes)
            {
                switch (node.NodeType)
                {
                    case "text":
                        inlines.Add(Run(node.Text));
                        break;
                    case "url":
                        inlines.Add(Hyperlink("[link]", node.Text));
                        break;
                    case "mention":
                        inlines.Add(Mention(node.Text));
                        break;
                    case "hashtag":
                        inlines.Add(Hashtag(node.Text));
                        break;
                    case "media":
                        inlines.Add(Hyperlink("[link]", node.Text));
                        break;
                }
            }
            textBlock.Inlines.Clear();
            textBlock.Inlines.AddRange(inlines);
            Utilities.System.NativeMethods.keybd_event(0x28, 0, 0x0002, IntPtr.Zero);
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
                Command = MyCommands.OpenLinkCommand,
                CommandParameter = link,
                ToolTip = link
            };
            ToolTipService.SetInitialShowDelay(hyperlink, 0);
            ToolTipService.SetShowDuration(hyperlink, 30000);
            hyperlink.ToolTipOpening += async (s, e) => hyperlink.ToolTip = await LongUrl.Lookup(link);
            return hyperlink;
        }

        private static Hyperlink Mention(string text)
        {
            return new Hyperlink(new Run("@" + text))
            {
                Command = MyCommands.ShowUserInformationCommand,
                CommandParameter = text
            };
        }

        private static Hyperlink Hashtag(string text)
        {
            return new Hyperlink(new Run("#" + text))
            {
                Command = MyCommands.SearchCommand,
                CommandParameter = text
            };
        }
    }
}