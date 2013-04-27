// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;

namespace tweetz5.Controls
{
    public partial class ComposeTweet
    {
        public ComposeTweet()
        {
            InitializeComponent();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _textBox.Focus();
        }
    }
}