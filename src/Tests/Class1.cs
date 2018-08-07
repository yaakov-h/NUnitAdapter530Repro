using System;
using NUnit.Framework;

namespace Tests
{
    public class Fixture
    {
		[Test]
		public void PassingTest() => Assert.Pass();

		[TestCase(null)]
		public void PassingTestCase(object o) => Assert.Pass();

		[Ignore("reasons")]
		public void IgnoredTest() => Assert.Fail();

		[TestCase(null, Ignore = "because")]
		public void IgnoredTestCase(object ignored) => Assert.Fail("This test should not be run.");

		[Explicit("reasons")]
		public void ExplicitTest() => Assert.Fail();

		[TestCase(null, Explicit = true, Reason = "because")]
		public void ExplicitTestCase(object ignored) => Assert.Fail("This test should not be run.");
	}
}
