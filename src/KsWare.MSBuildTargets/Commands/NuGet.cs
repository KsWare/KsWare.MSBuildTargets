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
					result = global::NuGet.CommandLine.Program.Main(Helper.UnescapeSecretArguments(pack.Arguments));
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(Helper.HideSecretArguments(pack.Arguments)));
					if (result == 0)
						Configuration.GlobalProperties.Set(N.NuGet.Push.PackagePath, Helper.GetPackagePath(Configuration));
					break;
				}
				case "push": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-push
					var push = new NuGetPush(args, Configuration);
					var args0 = Helper.UnescapeSecretArguments(push.Arguments);
					Console.WriteLine("nuget " + Helper.JoinSpaceSeparatedVerbatimString(Helper.HideSecretArguments(push.Arguments)));
					result = global::NuGet.CommandLine.Program.Main(Helper.UnescapeSecretArguments(push.Arguments));
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
