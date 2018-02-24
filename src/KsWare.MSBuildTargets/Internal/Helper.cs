using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KsWare.MSBuildTargets.Nuget.RestApiV3;
using NuGet.Versioning;

namespace KsWare.MSBuildTargets.Internal {

	public static class Helper {

		/// <summary>
		/// Splits and unescapes space separated verbatim string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>List of separated string</returns>
		/// <remarks><para>This is the opposite function from <see cref="JoinSpaceSeparatedVerbatimString"/></para></remarks>
		public static List<string> SplitSpaceSeparatedVerbatimString(string value) {
			var list = Regex.Matches(value, @"\""(\""\""|[^\""])+\""|[^ ]+", RegexOptions.ExplicitCapture).Cast<Match>()
				.Select(m => m.Value).ToList();
			list = list.Select(m => m.StartsWith("\"") ? m.Substring(1, m.Length - 2).Replace("\"\"", "\"") : m).ToList();
			return list;
		}

		/// <summary>
		/// Joins multiple strings to space separated verbatim string.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>System.String.</returns>
		/// <remarks>
		/// <para>With this function you can safely join an array of strings which includes double quotes to an single string .</para>
		/// <para><c>A</c> + <c>B B</c> + <c>C "C" C</c> converts to <c>A "B B" "C ""C"" C"</c>.</para>
		/// <para>This is the opposite function from <see cref="SplitSpaceSeparatedVerbatimString"/></para>
		/// </remarks>
		public static string JoinSpaceSeparatedVerbatimString(IEnumerable<string> values) {
			if (values==null || !values.Any()) return null;
			var sb = new StringBuilder();
			foreach (var option in values) {
				var v        = option;
				var quote    = option.Contains(' ') || option.Contains('"');
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
				.OrderBy(v=>v)
				.ToArray();
			return versions;
		}

		public static SemanticVersion[] GetExistingVersionsNugetOrg(string target) {
			var result=new NuGetApiClientV3().Search(target, true).Result;
			if (result.TotalHits == 0) throw new ArgumentException("Package name not found.");
			if (result.TotalHits > 1) throw new ArgumentException("Package name is not unique.");

			return result.Data[0].Versions.Select(v => SemanticVersion.Parse(v.Version)).ToArray();
		}

		public static string IncrementMajor(string target, string outputDirectory) {
			var v   = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major + 1, 0, 0, "", v.Metadata);
			return newVersion.ToFullString();
		}

		public static string IncrementMinor(string target, string outputDirectory) {
			var v = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major, v.Minor + 1, 0, "", v.Metadata);
			return newVersion.ToFullString();
		}

		public static string IncrementPatch(string target, string outputDirectory) {
			var v = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch + 1, "", v.Metadata);
			return newVersion.ToFullString();
		}

		public static string IncrementSuffixCI(string target, string outputDirectory) {
			// SemVer1
			var v = GetExistingVersions(target, outputDirectory).Last();
			SemanticVersion newVersion;
			if (v.Release == string.Empty) {
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch+1, "CI00001", v.Metadata);
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
