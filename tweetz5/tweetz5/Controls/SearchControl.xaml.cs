// Copyright (c) 2013 Blue Onion Software - All rights reserved

namespace tweetz5.Controls
{
    public partial class SearchControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        public void SetSearchText(string text)
        {
            if (text != null) SearchText.Text = text;
        }
    }
}