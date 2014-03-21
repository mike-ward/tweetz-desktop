namespace tweetz5.Model
{
    public enum MarkupNodeType
    {
        Text,
        Url,
        Mention,
        HashTag,
        Media
    }

    public class MarkupNode
    {
        public MarkupNodeType MarkupNodeType { get; private set; }
        public string Text { get; private set; }

        public MarkupNode(MarkupNodeType markupNodeType, string text)
        {
            MarkupNodeType = markupNodeType;
            Text = text;
        }
    }
}