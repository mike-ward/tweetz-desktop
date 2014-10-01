// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
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
        [TearDown]
        public void TearDown()
        {
            SysTimer.Override = null;
        }

        [Test]
        public void ConstructingControllerStartsTimelines()
        {
            var checkTimelines = new Mock<ITimer>();
            var updateTimelines = new Mock<ITimer>();
            var queue = new Queue<ITimer>();
            queue.Enqueue(checkTimelines.Object);
            queue.Enqueue(updateTimelines.Object);
            SysTimer.Override = queue.Dequeue;

            var timelines = new Mock<ITimelines>();
            var controller = new TimelineController(timelines.Object);
            controller.StartTimelines();
            checkTimelines.Raise(c => c.Elapsed += null, EventArgs.Empty);
            updateTimelines.Raise(u => u.Elapsed += null, EventArgs.Empty);

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
    }
}