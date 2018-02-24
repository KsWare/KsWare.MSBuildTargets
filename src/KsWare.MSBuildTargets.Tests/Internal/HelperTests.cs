using System.IO;
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

		[TestMethod()] //TODO improve test
		public void IncrementSuffixCITest() {
			var outputDirectory = @"D:\Develop\Packages";
			var target = @"KsWare.MSBuildTargets";
			if(!Directory.Exists(outputDirectory)) Assert.Inconclusive("OutputDirectory not configured.");
			var v = Helper.IncrementSuffixCI(target, outputDirectory);
			StringAssert.StartsWith(v.ToFullString(),"CI");
		}
	}
}