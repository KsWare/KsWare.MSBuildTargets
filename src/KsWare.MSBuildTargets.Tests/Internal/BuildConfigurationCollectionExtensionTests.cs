using FluentAssertions;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.MSBuildTargets.Tests.Internal {

	[TestClass()]
	public class BuildConfigurationCollectionExtensionTests {

		[TestMethod()]
		public void GetTest() {
			var col = new[] {
				new ConfigurationFile.BuildConfiguration()                         {Id = 1}, 
				new ConfigurationFile.BuildConfiguration("BeforeBuild")            {Id = 2},
				new ConfigurationFile.BuildConfiguration("BeforeBuild", "Debug"  ) {Id = 3},
				new ConfigurationFile.BuildConfiguration("BeforeBuild", "Release") {Id = 4},
				new ConfigurationFile.BuildConfiguration("AfterBuild")             {Id = 5},
				new ConfigurationFile.BuildConfiguration("AfterBuild",  "Debug"  ) {Id = 6},
				new ConfigurationFile.BuildConfiguration("AfterBuild",  "Release") {Id = 7},
			};
			col.Get(null)?.Id.Should().Be(1);
			col.Get(null, null)?.Id.Should().Be(1);
			col.Get(null, null, null)?.Id.Should().Be(1);
			col.Get("AfterBuild")?.Id.Should().Be(5);
			col.Get("AfterBuild", null)?.Id.Should().Be(5);
			col.Get("AfterBuild", null, null)?.Id.Should().Be(5);
			col.Get("AfterBuild", "Debug")?.Id.Should().Be(6);
			col.Get("AfterBuild", "Debug", null)?.Id.Should().Be(6);
		}
	}
}