using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Toolkit.Common.Enums;

namespace Toolkit.UnitTests
{
    [TestClass]
    public class EnumHelperTests
    {
        private enum TestEnum
        {
            First,
            Second,
            Third
        }

        [TestMethod]
        public void TestEnumFromString()
        {
            var expected = TestEnum.First;
            var validValue = expected.ToString();
            var actual = EnumHelper.EnumFromString<TestEnum>(validValue);
            Assert.AreEqual<TestEnum>(expected, actual);
        }

        [TestMethod]
        public void TestEnumFromStringCustomDefaultValue()
        {
        }

        [TestMethod]
        public void TestEnumFromStringInvalidValue()
        {
        }
    }
}