// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System.Windows;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Model;

namespace tweetz5UnitTests.Model
{
    [TestClass]
    public class StringValueVisibilityConverterTests
    {
        [TestMethod]
        public void ConverterShouldReturnVisibleWhenNotEmpty()
        {
            var converter = new StringValueVisibilityConverter();
            converter.Convert("string", null, null, null).Should().Be(Visibility.Visible);
        }

        [TestMethod]
        public void ConverterShouldReturnCollapsedWhenNullOrEmpty()
        {
            var converter = new StringValueVisibilityConverter();
            converter.Convert("", null, null, null).Should().Be(Visibility.Collapsed);            
            converter.Convert(null, null, null, null).Should().Be(Visibility.Collapsed);            
        }
    }
}