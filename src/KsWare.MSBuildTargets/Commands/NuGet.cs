using System;
using System.Linq;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;
using static KsWare.MSBuildTargets.Internal.Helper;

namespace KsWare.MSBuildTargets.Commands {

	public static class NuGet {

		private static ConfigurationFile Configuration => Helper.Configuration;

		public static int Call(string[] args) {
			int result;
			var expandedArgs = args.ToList();

			switch (expandedArgs[0].ToLower()) {
				case "pack": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-pack
					var pack = new NuGetPack(args, Configuration);
					result = global::NuGet.CommandLine.Program.Main(UnescapeSecretArguments(pack.Arguments));
					Console.WriteLine("nuget " + JoinSpaceSeparatedVerbatimString(HideSecretArguments(pack.Arguments)));
					if (result == 0)
						Configuration.GlobalProperties.Set(N.NuGet.Push.PackagePath, GetPackagePath(Configuration));
					break;
				}
				case "push": { // https://docs.microsoft.com/en-us/nuget/tools/cli-ref-push
					var push = new NuGetPush(args, Configuration);
					Console.WriteLine("nuget " + JoinSpaceSeparatedVerbatimString(HideSecretArguments(push.Arguments)));
					result = global::NuGet.CommandLine.Program.Main(UnescapeSecretArguments(push.Arguments));
					break;
				}
				default: {
					ExpandVariables(expandedArgs);
					Console.WriteLine("nuget " + JoinSpaceSeparatedVerbatimString(HideSecretArguments(expandedArgs)));
					result = global::NuGet.CommandLine.Program.Main(UnescapeSecretArguments(expandedArgs));
					break;
				}
			}

			return result;
		}

	}

}
