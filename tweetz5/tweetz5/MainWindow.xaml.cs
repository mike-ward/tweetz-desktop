// Copyright (c) 2012 Blue Onion Software. All rights reserved.

namespace tweetz5
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}