using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using KsWare.MSBuildTargets.Commands;
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
			Configuration = ConfigurationFile.LoadRecursive(directory);
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
				case "nuget":case "nuget.exe" : return CallNuGet(args);
			}
			
			//TODO find cmd 
			var p = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = cmd,
					Arguments = Helper.JoinCommandLineArgs(args)
				}
			};
			p.Start();
			p.WaitForExit();//TODO timeout
			return p.ExitCode;
		}

		private static int CallNuGet(string[] args) {
			int result;
			var expandedArgs = args.ToArray(); 

			switch (expandedArgs[0].ToLower()) {
				case "pack": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-pack
					var pack = new NuGetPack();
					ExpandVariables(ref expandedArgs,"NuGet.Pack");
					pack.Source = expandedArgs[1];
					for (int i = 2; i < expandedArgs.Length; i++) {
						switch (expandedArgs[i].ToLowerInvariant()) {
							case n.nuget.pack.OutputDirectory: pack.OutputDirectory = expandedArgs[++i]; break;
						}
					}
					ExpandSpecialVariables(pack, ref expandedArgs);
					result = global::NuGet.CommandLine.Program.Main(expandedArgs);
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(expandedArgs));
					if (result==0) Configuration.GlobalProperties.Set(N.NuGet.Push.PackagePath,GetPackagePath(Configuration));
					break;
				}
				case "push": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-push
					var push = new NuGetPush();
					ExpandVariables(ref expandedArgs,"NuGet.Push");
					ExpandSpecialVariables(push, ref expandedArgs);
					push.PackagePath = expandedArgs[1];
					Console.WriteLine("nuget "+Helper.JoinSpaceSeparatedVerbatimString(expandedArgs));
					result = global::NuGet.CommandLine.Program.Main(expandedArgs);
					break;
				}
				default: {
					ExpandVariables(ref expandedArgs);
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(expandedArgs));
					result = global::NuGet.CommandLine.Program.Main(expandedArgs);
					break;
				}
			}
			return result;
		}
		
		private static string GetPackagePath(ConfigurationFile configuration) {
			var outputDirectory = configuration.GetProperty(N.NuGet.Pack.OutputDirectory);
			var targetName= Path.GetFileNameWithoutExtension(configuration.GetProperty(N.IDE.TargetPath));
			var d = new DirectoryInfo(outputDirectory);
			var f=d.GetFiles($"{targetName}.*.nupkg").Aggregate((curMin, x) =>
				curMin == null || x.LastWriteTime < curMin.LastWriteTime ? x : curMin);
			return f.FullName;
		}

		private static void ExpandVariables(ref string[] args, string defaultNamespace=null) {
			for (int i = 0; i < args.Length; i++) {
				var variables = Helper.GetVariables(args[i]);
				foreach (var variable in variables) {
					var v = Configuration.GetProperty(variable);
					if (v == null && variable.Contains(".")) continue;
					if (v == null && !variable.Contains(".") && defaultNamespace == null) continue;
					if (v == null) v = Configuration.GetProperty($"{defaultNamespace}.{variable}");
					if (v == null) continue;
					args[i]=args[i].Replace($"${variable}$", v);
				}
			}
		}

		private static void ExpandSpecialVariables(NuGetPack pack, ref string[] args) {
			var outputDirectory = pack.OutputDirectory??Configuration.GetProperty(N.NuGet.Pack.OutputDirectory); //TODO check null
			var source = Configuration.GetProperty(N.IDE.TargetPath);
			for (int i = 0; i < args.Length; i++) {
				switch (args[i].ToLowerInvariant()) {
					case "$incrementci$": args[i] = Helper.IncrementSuffixCI(source,outputDirectory).ToFullString(); break;
					case "$incrementpatch$": args[i] = Helper.IncrementPatch(source, outputDirectory).ToFullString(); break;
				}
			}
		}

		private static void ExpandSpecialVariables(NuGetPush push, ref string[] args) {
			for (int i = 0; i < args.Length; i++) {
				switch (args[i].ToLowerInvariant()) {
					case "$packagepath$":
						args[i] = Configuration.GetProperty(N.NuGet.Push.PackagePath);
						break;
				}
			}
		}

		/// <summary>
		/// Provides all variable names.
		/// </summary>
		public static class N {

			public static class IDE {
				private const string FullName = nameof(IDE);

				/* https://msdn.microsoft.com/en-us/library/c02as0cs.aspx
				$(RemoteMachine)	Set to the value of the Remote Machine property on the Debug property page. See Changing Project Settings for a C/C++ Debug Configuration for more information.
				$(Configuration)	The name of the current project configuration, for example, "Debug".
				$(Platform)	The name of current project platform, for example, "Win32".
				$(ParentName)	(Deprecated.) Name of the item containing this project item. This will be the parent folder name, or project name.
				$(RootNameSpace)	The namespace, if any, containing the application.
				$(IntDir)	Path to the directory specified for intermediate files. If this is a relative path, intermediate files go to this path appended to the project directory. This path should have a trailing slash. This resolves to the value for the Intermediate Directory property. Do not use $(OutDir) to define this property.
				$(OutDir)	Path to the output file directory. If this is a relative path, output files go to this path appended to the project directory. This path should have a trailing slash. This resolves to the value for the Output Directory property. Do not use $(IntDir) to define this property.
				$(DevEnvDir)	The installation directory of Visual Studio (defined as drive + path); includes the trailing backslash '\'.
				$(InputDir)	(Deprecated; migrated.) The directory of the input file (defined as drive + path); includes the trailing backslash '\'. If the project is the input, then this macro is equivalent to $(ProjectDir).
				$(InputPath)	(Deprecated; migrated.) The absolute path name of the input file (defined as drive + path + base name + file extension). If the project is the input, then this macro is equivalent to $(ProjectPath).
				$(InputName)	(Deprecated; migrated.) The base name of the input file. If the project is the input, then this macro is equivalent to $(ProjectName).
				$(InputFileName)	(Deprecated; migrated.) The file name of the input file (defined as base name + file extension). If the project is the input, then this macro is equivalent to $(ProjectFileName).
				$(InputExt)	(Deprecated; migrated.) The file extension of the input file. It includes the '.' before the file extension. If the project is the input, then this macro is equivalent to $(ProjectExt).
				$(ProjectDir)	The directory of the project (defined as drive + path); includes the trailing backslash '\'.
				$(ProjectPath)	The absolute path name of the project (defined as drive + path + base name + file extension).
				$(ProjectName)	The base name of the project.
				$(ProjectFileName)	The file name of the project (defined as base name + file extension).
				$(ProjectExt)	The file extension of the project. It includes the '.' before the file extension.
				$(SolutionDir)	The directory of the solution (defined as drive + path); includes the trailing backslash '\'.
				$(SolutionPath)	The absolute path name of the solution (defined as drive + path + base name + file extension).
				$(SolutionName)	The base name of the solution.
				$(SolutionFileName)	The file name of the solution (defined as base name + file extension).
				$(SolutionExt)	The file extension of the solution. It includes the '.' before the file extension.
				$(TargetDir)	The directory of the primary output file for the build (defined as drive + path); includes the trailing backslash '\'.
				$(TargetPath)	The absolute path name of the primary output file for the build (defined as drive + path + base name + file extension).
				$(TargetName)	The base name of the primary output file for the build.
				$(TargetFileName)	The file name of the primary output file for the build (defined as base name + file extension).
				$(TargetExt)	The file extension of the primary output file for the build. It includes the '.' before the file extension.
				$(VSInstallDir)	The directory into which you installed Visual Studio.

				This property contains the version of the targeted Visual Studio, which might be different that the host Visual Studio. For example, when building with $(PlatformToolset) = v110, $(VSInstallDir) contains the path to the Visual Studio 2012 installation.
				$(VCInstallDir)	The directory into which you installed Visual C++.

				This property contains the version of the targeted Visual C++, which might be different that the host Visual Studio. For example, when building with $(PlatformToolset) = v140, $(VCInstallDir) contains the path to the Visual C++ 2015 installation.
				$(FrameworkDir)	The directory into which the .NET Framework was installed.
				$(FrameworkVersion)	The version of the .NET Framework used by Visual Studio. Combined with $(FrameworkDir), the full path to the version of the .NET Framework use by Visual Studio.
				$(FrameworkSDKDir)	The directory into which you installed the .NET Framework. The .NET Framework could have been installed as part of Visual Studio or separately.
				$(WebDeployPath)	The relative path from the web deployment root to where the project outputs belong. Returns the same value as RelativePath.
				$(WebDeployRoot)	The absolute path to the location of <localhost>. For example, c:\inetpub\wwwroot.
				$(SafeParentName)	(Deprecated.) The name of the immediate parent in valid name format. For example, a form is the parent of a .resx file.
				$(SafeInputName)	(Deprecated.) The name of the file as a valid class name, minus file extension.
				$(SafeRootNamespace)	(Deprecated.) The namespace name in which the project wizards will add code. This namespace name will only contain characters that would be permitted in a valid C++ identifier.
				$(FxCopDir)	The path to the fxcop.cmd file. The fxcop.cmd file is not installed with all Visual C++ editions.
				*/

				/// <summary>
				/// The absolute path name of the project (defined as drive + path + base name + file extension).
				/// </summary>
				public const string ProjectPath = FullName + "." + nameof(ProjectPath);

				/// <summary>
				/// The name of the current project configuration, for example, "Debug".
				/// </summary>
				public const string ConfigurationName = FullName + "." + nameof(ConfigurationName);

				/// <summary>
				/// The name of current project platform, for example, "Win32".
				/// </summary>
				public const string PlatformName = FullName + "." + nameof(PlatformName);

				/// <summary>
				/// The absolute path name of the primary output file for the build (defined as drive + path + base name + file extension).
				/// </summary>
				public const string TargetPath = FullName + "." + nameof(TargetPath);
			}

			public static class NuGet {
				private const string FullName = nameof(NuGet);

				public static class Pack {
					// ReSharper disable once MemberHidesStaticFromOuterClass
					private const string FullName = NuGet.FullName + "." + nameof(Pack);

					public const string OutputDirectory = FullName + "." + nameof(OutputDirectory);
				}

				public static class Push {
					// ReSharper disable once MemberHidesStaticFromOuterClass
					private const string FullName = NuGet.FullName + "." + nameof(Push);

					public const string PackagePath = FullName + "." + nameof(PackagePath);
				}
			}

			public const string Target = nameof(Target);
		}

		public static class n {
			public static class nuget {
				public static class pack {
					public const string OutputDirectory = "-outputdirectory";
				}
			}
		}
	}

}
