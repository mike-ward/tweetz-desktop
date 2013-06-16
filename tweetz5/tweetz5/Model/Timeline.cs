// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Collections.ObjectModel;

namespace tweetz5.Model
{
    public class Timeline
    {
        public Timeline()
        {
            Tweets = new ObservableCollection<Tweet>();
            SinceId = 1;
        }

        public ObservableCollection<Tweet> Tweets { get; private set; }
        public ulong SinceId { get; set; }

        public void Clear()
        {
            Tweets.Clear();
            SinceId = 1;
        }
    }
}