using Microsoft.VisualStudio.TestTools.UnitTesting;
using KsWare.MSBuildTargets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace KsWare.MSBuildTargets.Tests {

	[TestClass()]
	public class ProgramTests {

		[TestMethod()]
		public void MainTest() {
			var repoPath = @"D:\Develop\Extern\GitHub.KsWare\KsWare.MSBuildTargets";
			var ProjectPath = $@"{repoPath}\src\KsWare.MSBuildTargets.DemoApp\KsWare.MSBuildTargets.DemoApp.csproj";
			var ConfigurationName = "Debug";
			var PlatformName = "Any CPU";
			var TargetPath = $@"{repoPath}\src\KsWare.MSBuildTargets.DemoApp\bin\Debug\KsWare.MSBuildTargets.DemoApp.exe";

			if (!File.Exists(ProjectPath) || !File.Exists(TargetPath)) 
				Assert.Inconclusive("Demo project path not found.");

			var rc=Program.Main(new[] {"-pp", ProjectPath, "-cn", ConfigurationName, "-pn", PlatformName, "-tp", TargetPath});

			Assert.That.Variable(rc).Should().Be(0);
		}
	}
}