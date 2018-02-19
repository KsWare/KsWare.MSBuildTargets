using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NuGet.Versioning;

namespace KsWare.MSBuildTargets.Internal {

	public static class Helper {

		public static List<string> SplitSpaceSeperatedVerbatimString(string value) {
			var list = Regex.Matches(value, @"\""(\""\""|[^\""])+\""|[^ ]+", RegexOptions.ExplicitCapture).Cast<Match>()
				.Select(m => m.Value).ToList();
			list = list.Select(m => m.StartsWith("\"") ? m.Substring(1, m.Length - 2).Replace("\"\"", "\"") : m).ToList();
			return list;
		}

		public static string JoinSpaceSeperatedVerbatimString(IEnumerable<string> values) {
			if (values==null || !values.Any()) return null;
			var sb = new StringBuilder();
			foreach (var option in values) {
				var v        = option;
				var quote    = option.Contains(" ");
				if (quote) v = v.Replace("\"", "\"\"");
				if (quote) v = $"\"{v}\"";
				sb.Append(" " + v);
			}
			return sb.ToString(1, sb.Length - 1);
		}

		public static SemanticVersion[] GetExistingVersions(string target, string outputDirectory) {
			var regex=new Regex(@"(?imnx-s:^\d+\.\d+\.\d+(-[A-Z0-9\.]*)?(\+[A-Z0-9\.]*)?$)"); //TODO test regex
			var targetName = Path.GetFileNameWithoutExtension(target);
			var versions=Directory.GetFiles(outputDirectory, $"{targetName}*.nupkg")
				.Select(Path.GetFileNameWithoutExtension)
				.Select(s=>s.Substring(targetName.Length))
				.Where(s=>regex.IsMatch(s))
				.Select(SemanticVersion.Parse)
				.ToArray();
			return versions;
		}

		public static string AutoIncrementPatch(string target, string outputDirectory) {
			var targetName = Path.GetFileNameWithoutExtension(target);
			var versions = GetExistingVersions(target, outputDirectory);
			Array.Sort(versions, new VersionComparer());
			var             v = versions.Last();
			SemanticVersion newVersion;
			newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch + 1, "", v.Metadata);
			return newVersion.ToFullString();
		}

		public static string AutoIncrementSuffixCI(string source, string outputDirectory) {
			var projectName = Path.GetFileNameWithoutExtension(source);
			var packages    = Directory.GetFiles(outputDirectory, $"{projectName}*-CI*.nupkg");
			var versions    = packages.Select(ExtractVersion).ToArray();
			Array.Sort(versions, new VersionComparer());
			var             v = versions.LastOrDefault() ?? new SemanticVersion(1, 0, 0);
			SemanticVersion newVersion;
			if (v.Release == string.Empty) {
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch, "CI00001", v.Metadata);
			}
			else if (v.Release.StartsWith("CI")) {
				var ci = int.Parse(Regex.Match(v.Release, @"\d+$").Value);
				ci++;
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch, $"CI{ci:D5}", v.Metadata);
			}
			else {
				//TODO this will overwrite existing release
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch, "CI00001", v.Metadata);
			}

			return newVersion.Release;
		}

		public static SemanticVersion ExtractVersion(string nupgk) {
			var n     = Path.GetFileNameWithoutExtension(nupgk);
			var match = Regex.Match(n, @"\d+\.\d+\.\d+(-.+)?$");
			return SemanticVersion.Parse(match.Value);
		}

		/// <summary>
		/// Encodes an argument for passing into a program
		/// </summary>
		/// <param name="original">The value that should be received by the program</param>
		/// <returns>The value which needs to be passed to the program for the original value 
		/// to come through</returns>
		public static string EncodeParameterArgument(string original) {
			if (string.IsNullOrEmpty(original)) return original;
			string value = Regex.Replace(original, @"(\\*)" + "\"", @"$1\$0");
			value = Regex.Replace(value, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"");
			return value;
		}

		// This is an EDIT
		// Note that this version does the same but handles new lines in the arugments
		public static string EncodeParameterArgumentMultiLine(string original) {
			if (string.IsNullOrEmpty(original)) return original;
			string value = Regex.Replace(original, @"(\\*)" + "\"", @"$1\$0");
			value = Regex.Replace(value, @"^(.*\s.*?)(\\*)$", "\"$1$2$2\"", RegexOptions.Singleline);

			return value;
		}

		public static string JoinCommandLineArgs(string[] args) {
			return string.Join(" ", args.Select(EncodeParameterArgumentMultiLine));
		}

		public static string[] GetVariables(string s) {
			return Regex.Matches(s, @"(?<=\$)[_\p{L}][\w\.]*(?=\$)").Cast<Match>().Select(m=>m.Value).ToArray();
		}
	}

}
