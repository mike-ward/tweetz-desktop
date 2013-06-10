// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows.Input;

namespace tweetz5.Controls
{
    public partial class StatusAlert
    {
        public StatusAlert()
        {
            InitializeComponent();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}