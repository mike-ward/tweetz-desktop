// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System.Windows;
using System.Windows.Media;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Model;

namespace tweetz5UnitTests.Model
{
    [TestClass]
    public class TweetTypeValueConverterTests
    {
        [TestMethod]
        public void ConverterShouldReturnSolidBrushWhenValueContainsM()
        {
            var converter = new TweetTypeValueConverter();
            converter.Convert("fm", null, null, null).Should().BeOfType<SolidColorBrush>();
        }

        [TestMethod]
        public void ConverterShouldReturnDependencyPropertyUnsetWhenValueDoesNotContainM()
        {
            var converter = new TweetTypeValueConverter();
            converter.Convert("f", null, null, null).Should().Be(DependencyProperty.UnsetValue);
        }
    }
}