// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.IO;
using System.Text;
using System.Windows;

namespace tweetz5.Utilities
{
    public static class CopyToClipboard
    {
        public static void AsText(string text)
        {
            Clipboard.SetText(text);
        }

        public static void AsTextAndHtml(string text, string htmlFragment)
        {
            var enc = Encoding.UTF8;

            const string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}"
                                 + "\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\n";

            var htmlBegin = "<html>\r\n<head>\r\n"
                            + "<meta http-equiv=\"Content-Type\""
                            + " content=\"text/html; charset=" + enc.WebName + "\">\r\n"
                            + "<title>HTML clipboard</title>\r\n</head>\r\n<body>\r\n"
                            + "<!--StartFragment-->";

            const string htmlEnd = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

            var beginSample = String.Format(begin, 0, 0, 0, 0);

            var countBegin = enc.GetByteCount(beginSample);
            var countHtmlBegin = enc.GetByteCount(htmlBegin);
            var countHtml = enc.GetByteCount(htmlFragment);
            var countHtmlEnd = enc.GetByteCount(htmlEnd);

            var htmlTotal = String.Format(
                begin
                , countBegin
                , countBegin + countHtmlBegin + countHtml + countHtmlEnd
                , countBegin + countHtmlBegin
                , countBegin + countHtmlBegin + countHtml) 
                + htmlBegin + htmlFragment + htmlEnd;

            var obj = new DataObject();
            var textStream = new MemoryStream(enc.GetBytes(text));
            obj.SetData(DataFormats.Text, textStream);
            var htmlStream = new MemoryStream(enc.GetBytes(htmlTotal));
            obj.SetData(DataFormats.Html, htmlStream);
            Clipboard.SetDataObject(obj, true);
            textStream.Dispose();
            htmlStream.Dispose();
        }
    }
}