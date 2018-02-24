using System.Threading.Tasks;
using KsWare.MSBuildTargets.Nuget.RestApiV3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.MSBuildTargets.Tests.Nuget.RestApiV3 {

	[TestClass()]
	public class NuGetApiClientV3Tests {

		[TestMethod()]
		public async Task SearchTest() {
			var r = await new NuGetApiClientV3().Search("KsWare.MSBuildTargets", true);
			Assert.AreEqual(1, r.TotalHits);
			StringAssert.Contains(r.Data[0].Versions[0].Version, ".");
		}
	}
}