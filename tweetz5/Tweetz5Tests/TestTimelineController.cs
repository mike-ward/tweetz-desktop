// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Timers;
using Moq;
using NUnit.Framework;
using tweetz5;
using tweetz5.Model;
using tweetz5.Utilities.System;

namespace Tweetz5Tests
{
    [TestFixture]
    public class TestTimelineController
    {
        [Test]
        public void ConstructingControllerStartsTimelines()
        {
            try
            {
                var checkTimelines = new Mock<ITimer>();
                var updateTimelines = new Mock<ITimer>();
                var queue = new Queue<ITimer>();
                queue.Enqueue(checkTimelines.Object);
                queue.Enqueue(updateTimelines.Object);
                MyTimer.Override = queue.Dequeue;

                var timelines = new Mock<ITimelines>();
                var controller = new TimelineController(timelines.Object);
                checkTimelines.Raise(c => c.Elapsed += null, ElapsedEventArgsEmpty());
                updateTimelines.Raise(u => u.Elapsed += null, ElapsedEventArgsEmpty());

                checkTimelines.VerifySet(c => c.Interval = 100);
                checkTimelines.VerifySet(c => c.Interval = 60000);
                checkTimelines.Verify(c => c.Start());
                timelines.Verify(t => t.HomeTimeline());
                timelines.Verify(t => t.MentionsTimeline());

                updateTimelines.VerifySet(u => u.Interval = 30000);
                updateTimelines.Verify(u => u.Start());
                timelines.Verify(t => t.UpdateTimeStamps());
                Assert.That(controller, Is.Not.Null);
            }
            finally
            {
                MyTimer.Override = null;
            }
        }

        private static ElapsedEventArgs ElapsedEventArgsEmpty()
        {
            var types = new Type[2];
            types[0] = typeof (int);
            types[1] = typeof (int);
            var args = new object[2];
            args[0] = 0;
            args[1] = 1;
            var constructor = typeof (ElapsedEventArgs).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
            return (ElapsedEventArgs)constructor.Invoke(BindingFlags.NonPublic, null, args, CultureInfo.CurrentCulture);
        }
    }
}