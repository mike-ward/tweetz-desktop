// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // It was the only way
            Task.Run(() =>
            {
                Thread.Sleep(100);
                Dispatcher.Invoke(() => SearchText.Focus());
            });
        }
    }
}   