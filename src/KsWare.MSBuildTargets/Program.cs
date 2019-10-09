using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets {

	public class Program {
		// TODO: load configuration file

		public static ConfigurationFile Configuration = new ConfigurationFile();
		public static bool TestMode = false;

		//TODO remove debug code in Main
		public static int Main(string[] args) {
			// -pp $(ProjectPath) -cn $(ConfigurationName) -pn $(PlatformName) -tp $(TargetPath)
			if (args.Length > 0 && args[0].StartsWith("debug")) {
				var projectRoot = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..");
				switch (args[0]) {
					case "debug1": args=new []{
							"-bt","AfterBuild",
							"-pp", $@"{projectRoot}\KsWare.MSBuildTargets.DemoApp\KsWare.MSBuildTargets.DemoApp.csproj",
							"-cn", "Release",
							"-pn","Any CPU",
							"-tp", $@"{projectRoot}\KsWare.MSBuildTargets.DemoApp\bin\Debug\KsWare.MSBuildTargets.DemoApp.exe"
						};
						break;
					case "debug2":
						args = new[]{
							"-bt","AfterBuild",
							"-pp",$@"{projectRoot}\KsWare.MSBuildTargets\KsWare.MSBuildTargets.csproj",
							"-cn", "Debug",
							"-pn","Any CPU",
							"-tp", $@"{projectRoot}\KsWare.MSBuildTargets\bin\debug\KsWare.MSBuildTargets.exe"
						};
						break;
					case "debug3":
						args = new[]{
							"-bt", "BeforeBuild",
							"-pp", $@"{projectRoot}\KsWare.MSBuildTargets.DemoApp\KsWare.MSBuildTargets.DemoApp.csproj",
							"-cn", "Debug",
							"-pn", "Any CPU",
							"-tp", $@"{projectRoot}\KsWare.MSBuildTargets.DemoApp\bin\Debug\KsWare.MSBuildTargets.DemoApp.exe"
						};
						break;
				}
			}

			var properties = new List<ConfigurationFile.Property>();

			for (int i = 0; i < args.Length; i++) {
				var param = args[i];
				var paramL = args[i].ToLowerInvariant();

				switch (paramL) {
					case "-test": TestMode = true; break;
					case "-bt": properties.Set(N.Target, args[++i]); break;
					case "-pp": properties.Set(N.IDE.ProjectPath, args[++i]); break;
					case "-cn": properties.Set(N.IDE.ConfigurationName, args[++i]); break;
					case "-pn": properties.Set(N.IDE.PlatformName, args[++i]); break;
					case "-tp": properties.Set(N.IDE.TargetPath, args[++i]); break;
//					case "-version": Configuration.Version = args[++i]; break;
//					case "-suffix": Configuration.Suffix = args[++i]; break;
//					case "-outputdirectory": Configuration.OutputDirectory = args[++i]; break;
//					default: Configuration.Options.Add(param);break;	
				}
			}

			try {
				//TODO message .. Expected properties.GetValue(N.IDE.ProjectPath) not to be <null> or empty because Project path not specified!, but found <null>.
				properties.GetValue(N.Target)?.ToLowerInvariant().Should().BeOneOf("Target is invalid!", "beforebuild", "afterbuild");
				properties.GetValue(N.IDE.ProjectPath).Should().NotBeNullOrEmpty("Project path not specified!");
				File.Exists(properties.GetValue(N.IDE.ProjectPath)).Should().BeTrue("Project path not found!");
				properties.GetValue(N.IDE.TargetPath).Should().NotBeNullOrEmpty("Target path not specified!");
				properties.GetValue(N.IDE.ConfigurationName).Should().NotBeNullOrEmpty("Configuration name not specified!");
				properties.GetValue(N.IDE.PlatformName).Should().NotBeNullOrEmpty("Platform name not specified!");
			}
			catch (Exception ex) {
				Console.WriteLine("Usage: KsWare.MSBuildTargets.exe -bt BeforeBuild -pp $(ProjectPath) -cn $(ConfigurationName) -pn $(PlatformName) -tp $(TargetPath)");
				Console.Error.WriteLine(ex.Message);
				if(Assembly.GetEntryAssembly()==Assembly.GetExecutingAssembly()) Environment.Exit(1);
				return 1;
			}

			var directory = Path.GetDirectoryName(properties.GetValue(N.IDE.ProjectPath));
			Configuration = Helper.Configuration=ConfigurationFile.LoadRecursive(directory);
			Configuration.GlobalProperties = properties;

			var commands = Configuration.GetCommands(properties.GetValue(N.Target), properties.GetValue(N.IDE.ConfigurationName), true);
			foreach (var command in commands) {
				if (command.Flags.Contains("ignore", StringComparer.OrdinalIgnoreCase)) continue;
				if(string.IsNullOrWhiteSpace(command.CommandLine)) continue;
				var result = CallCommand(command.CommandLine);
				if (result != 0) return result;
			}
			return 0;

		}

		private static int CallCommand(string commandLine) {
			var cmdline = commandLine;

			var args = Helper.SplitSpaceSeparatedVerbatimString(cmdline).ToArray();
			var cmd = args[0];
			args = args.Skip(1).ToArray();

			switch (cmd.ToLowerInvariant()) {
				case "nuget":case "nuget.exe" : return Commands.NuGet.Call(args);
				default: return Commands.Generic.Call(cmd, args);
			}
		}


	}
}