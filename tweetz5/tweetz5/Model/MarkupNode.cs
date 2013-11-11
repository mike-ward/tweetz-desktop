namespace tweetz5.Model
{
    public class MarkupNode
    {
        public string NodeType { get; private set; }
        public string Text { get; private set; }

        public MarkupNode(string nodeType, string text)
        {
            NodeType = nodeType;
            Text = text;
        }
    }
}