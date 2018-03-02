using System;
using System.IO;
using System.Linq;
using KsWare.MSBuildTargets.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.MSBuildTargets.Tests.Internal {

	[TestClass()]
	public class HelperTests {

		private Action _testCleanup;

		[TestCleanup]
		public void TestCleanup() {
			_testCleanup?.Invoke();
		}

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

		[DataTestMethod]
		[DataRow("1.0.0"        , "1.0.1-CI00001")]
		[DataRow("1.0.1-CI00001", "1.0.1-CI00002")]
		[DataRow("1.0.1-xyz"    , "1.0.2-CI00001")]
		public void IncrementSuffixCITest(string test, string expected) {
			var outputDirectory = @"D:\Develop\Packages";
			if(!Directory.Exists(outputDirectory)) Assert.Inconclusive("OutputDirectory not configured.");
			var target = @"KsWare.MSBuildTargets.Tests";

			var f = Path.Combine(outputDirectory, target + "." + test + ".nupkg");
			_testCleanup = () => { File.Delete(f); };
			File.WriteAllText(f,null);

			var v0 = Helper.GetExistingVersions(target, outputDirectory).LastOrDefault();
			Assert.AreEqual(test, v0.ToFullString());

			var v1 = Helper.IncrementSuffixCI(target, outputDirectory);
			Assert.AreEqual(expected,v1.ToFullString());
		}

		[TestMethod()]
		public void FindFilesTest() {
			var directory = @"D:\Develop\Extern\GitHub.KsWare\KsWare.MSBuildTargets\src";
			var globPattern = @"**\AssemblyInfo.*";
			var files=Helper.FindFiles(directory, globPattern);
			Assert.AreEqual(4,files.Length);
		}

		[TestMethod()]
		public void GetExistingVersionsNugetOrgTest() {
			Assert.AreNotEqual(0, Helper.GetExistingVersions("KsWare.MSBuildTargets", null).Length);
			Assert.AreEqual(0, Helper.GetExistingVersions("KsWare.NonExistentPackage", null).Length);

			Assert.ThrowsException<DirectoryNotFoundException>(() => Helper.GetExistingVersions("KsWare.MSBuildTargets", @"X:\NonExistingFolder\Gfavc6567"));
		}

	}
}