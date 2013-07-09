// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace tweetz5.Utilities
{
    internal static class Run
    {
        public static void Later(int delay, Action action)
        {
            Task.Run(() =>
            {
                Thread.Sleep(delay);
                Application.Current.Dispatcher.Invoke(action);
            });
        }
    }
}