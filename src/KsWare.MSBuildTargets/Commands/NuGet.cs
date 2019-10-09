using System;
using System.Linq;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Commands {

	public static class NuGet {

		private static ConfigurationFile Configuration => Helper.Configuration;

		public static int Call(string[] args) {
			int result;
			var expandedArgs = args.ToList();

			switch (expandedArgs[0].ToLower()) {
				case "pack": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-pack
					var pack = new NuGetPack(args, Configuration);

					result = global::NuGet.CommandLine.Program.Main(pack.Arguments.ToArray());
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(expandedArgs));
					if (result == 0)
						Configuration.GlobalProperties.Set(N.NuGet.Push.PackagePath, Helper.GetPackagePath(Configuration));
					break;
				}
				case "push": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-push
					var push = new NuGetPush(args, Configuration);
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(push.Arguments));
					result = global::NuGet.CommandLine.Program.Main(push.Arguments.ToArray());
					break;
				}
				default: {
					Helper.ExpandVariables(expandedArgs);
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(expandedArgs));
					result = global::NuGet.CommandLine.Program.Main(expandedArgs.ToArray());
					break;
				}
			}

			return result;
		}

	}

}
