// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using tweetz5.Controls;
using tweetz5.Model;
using tweetz5.Utilities.System;

namespace tweetz5UnitTests
{
    [TestClass]
    public class TimelineControllerTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            SysTimer.ImplementationOverride = null;
        }

        [TestMethod]
        public void ConstructingControllerStartsTimelines()
        {
            Settings.ApplicationSettings.UseStreamingApi = false;
            var checkTimelines = new Mock<ITimer>();
            var updateTimelines = new Mock<ITimer>();
            var friendsBlockedTimelines = new Mock<ITimer>();
            var queue = new Queue<ITimer>();
            queue.Enqueue(checkTimelines.Object);
            queue.Enqueue(updateTimelines.Object);
            SysTimer.ImplementationOverride = queue.Dequeue;

            var timelines = new Mock<ITimelines>();
            var controller = new TimelineController(timelines.Object);
            controller.StartTimelines();
            checkTimelines.Raise(c => c.Elapsed += null, EventArgs.Empty);
            updateTimelines.Raise(u => u.Elapsed += null, EventArgs.Empty);
            friendsBlockedTimelines.Raise(u => u.Elapsed += null, EventArgs.Empty);

            checkTimelines.VerifySet(c => c.Interval = 100);
            checkTimelines.VerifySet(c => c.Interval = 70000);
            checkTimelines.Verify(c => c.Start());
            timelines.Verify(t => t.UpdateHome());
            timelines.Verify(t => t.UpdateMentions());

            updateTimelines.VerifySet(u => u.Interval = 100);
            updateTimelines.VerifySet(u => u.Interval = 30000);
            updateTimelines.Verify(u => u.Start());
            timelines.Verify(t => t.UpdateTimeStamps());

            controller.Should().NotBeNull();
        }
    }
}