using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tweetz5.Utilities;

namespace tweetz5UnitTests.Utilities
{
    [TestClass]
    public class ReadSafeListTests
    {
        [TestMethod]
        public void ReadSafeReturnsClone()
        {
            var list = new ReadSafeList<string> {"one", "two", "three"};
            var enumerator = list.GetEnumerator();
            list.Insert(0, "zero");
            list[0].Should().Be("zero");
            enumerator.MoveNext();
            enumerator.Current.Should().Be("one");
        }
    }
}