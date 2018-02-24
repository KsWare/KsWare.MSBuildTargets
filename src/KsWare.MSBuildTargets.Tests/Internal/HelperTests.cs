using KsWare.MSBuildTargets.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.MSBuildTargets.Tests.Internal {

	[TestClass()]
	public class HelperTests {

		[TestMethod()]
		public void SplitSpaceSeperatedVerbatimStringTest() {
			// A "B B" "C ""C"" C" """D"""

			CollectionAssert.AreEqual(
				new[] {@"A", @"B B", @"C ""C"" C", @"""D""" },
				Helper.SplitSpaceSeparatedVerbatimString(@"A ""B B"" ""C """"C"""" C"" """"""D"""""""));
		}

		[TestMethod()]
		public void JoinSpaceSeparatedVerbatimStringTest() {
			var input = new[] {@"A", @"B B", @"C ""C"" C", @"""D"""};
			Assert.AreEqual(@"A ""B B"" ""C """"C"""" C"" """"""D""""""",
				Helper.JoinSpaceSeparatedVerbatimString(input));
		}
	}
}