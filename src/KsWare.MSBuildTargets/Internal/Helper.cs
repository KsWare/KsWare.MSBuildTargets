using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotNet.Globbing;
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
			var regex=new Regex(@"(?imnx-s:\d+\.\d+\.\d+(-[A-Z0-9\.]*)?(\+[A-Z0-9\.]*)?$)"); //TODO test regex
			var targetName = GetTargetName(target);
			 
			var localVersions= outputDirectory==null ? new SemanticVersion[0]
				: Directory.GetFiles(outputDirectory, $"{targetName}.*.nupkg")	// filter possible files
				.Select(Path.GetFileNameWithoutExtension)						// trim extension
				.Select(s=>s.Substring(targetName.Length +1))					// remove target name and .
				.Where(s=>regex.IsMatch(s))										// filter valid versions "1.2.3-xyz+abc"
				.Select(SemanticVersion.Parse);									// convert to SemanticVersion

//			var a = Directory.GetFiles(outputDirectory, $"{targetName}.*.nupkg");
//			var b = a.Select(Path.GetFileNameWithoutExtension);
//			var c = b.Select(s => s.Substring(targetName.Length+1));
//			var d = c.Where(s => regex.IsMatch(s));
//			var e = d.Select(SemanticVersion.Parse);
//			var f = e.OrderBy(v => v).ToArray();

			var onlineVersions= GetExistingVersionsNugetOrg(target);

			var versions = localVersions.Concat(onlineVersions).OrderBy(v => v).ToArray();

			return versions;
		}

		// Get PackageID
		private static string GetTargetName(string target) {
			string targetName;
			if (target.Contains("\\")) {
				targetName = Path.GetFileNameWithoutExtension(target);
			}
			else {
				var ext = Path.GetExtension(target)?.ToLowerInvariant();
				if (ext == ".exe" || ext == ".dll")
					targetName = Path.GetFileNameWithoutExtension(target);
				else
					targetName = target;
			}
			return targetName;
		}

		public static SemanticVersion[] GetExistingVersionsNugetOrg(string target) {
			var result=new NuGetApiClientV3().Search(target, true).Result;
			if (result.TotalHits == 0) return new SemanticVersion[0];
			if (result.TotalHits > 1) throw new ArgumentException("Package name is not unique.");

			return result.Data[0].Versions.Select(v => SemanticVersion.Parse(v.Version)).ToArray();
		}

		public static SemanticVersion IncrementMajor(string target, string outputDirectory) {
			var v   = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major + 1, 0, 0, "", v.Metadata);
			return newVersion;
		}

		public static SemanticVersion IncrementMinor(string target, string outputDirectory) {
			var v = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major, v.Minor + 1, 0, "", v.Metadata);
			return newVersion;
		}

		public static SemanticVersion IncrementPatch(string target, string outputDirectory) {
			var v = GetExistingVersions(target, outputDirectory).Last();
			var newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch + 1, "", v.Metadata);
			return newVersion;
		}

		public static SemanticVersion IncrementSuffixCI(string target, string outputDirectory) {
			// SemVer1
			var v = GetExistingVersions(target, outputDirectory).LastOrDefault();
			if (v == null) // TODO extraxt assembly version
				throw new ArgumentException($"No existing package for target found.") {
					Data = {{"target", target}, {"outputDirectory", outputDirectory}}
				};
			SemanticVersion newVersion;
			if (v.Release == string.Empty) {
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch+1, "CI00001","");
			}
			else if (v.Release.StartsWith("CI")) {
				var ci = int.Parse(Regex.Match(v.Release, @"\d+$").Value);
				ci++;
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch, $"CI{ci:D5}","");
			}
			else {
				//TODO this will overwrite existing release
				newVersion = new SemanticVersion(v.Major, v.Minor, v.Patch+1, "CI00001", "");
			}

			return newVersion;
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

		public static string[] FindFiles(string directory, string globPattern) {
			var glob    = Glob.Parse(globPattern);
			var options = SearchOption.AllDirectories;
			var startDirectory = directory;
			// TODO optimize search path

			var files=new List<string>();
			var l1 = directory.EndsWith("\\") ? directory.Length : directory.Length + 1; // root with backslash
			foreach (var file in Directory.EnumerateFiles(startDirectory,"*", options)) {
				var relativePath = file.Substring(l1);
				Debug.WriteLine(relativePath);
				if (glob.IsMatch(relativePath)) files.Add(file);
			}
			return files.ToArray();
		}

		public static void PatchAssemblyVersion(string file,
			string assemblyVersion,
			string assemblyFileVersion,
			string assemblyInformationalVersion) {
			/*  [assembly: AssemblyVersion("0.1.78")]
				[assembly: AssemblyFileVersion("0.1.78")]
				[assembly: AssemblyInformationalVersion("0.1.78")] */

			var files=new List<string>();
			if (file.Contains("*") || file.Contains("?")) {
				// **\AssemblyInfo.*

			}
			else {
				files.Add(file);
			}

			var text = File.ReadAllText(file);
			if (!string.IsNullOrWhiteSpace(assemblyVersion))
				Regex.Replace(text, @"(?msnx-i:(?<=^\s*)\[assembly:\s*AssemblyVersion\s*\(\s*""[^""]*?""\)\s*\])",
					$"[assembly: AssemblyVersion(\"{assemblyVersion}\")]");
			if (!string.IsNullOrWhiteSpace(assemblyFileVersion))
				Regex.Replace(text, @"(?msnx-i:(?<=^\s*)\[assembly:\s*AssemblyFileVersion\s*\(\s*""[^""]*?""\)\s*\])",
					$"[assembly: AssemblyFileVersion(\"{assemblyFileVersion}\")]");
			if (!string.IsNullOrWhiteSpace(assemblyInformationalVersion))
				Regex.Replace(text, @"(?msnx-i:(?<=^\s*)\[assembly:\s*AssemblyVersion\s*\(\s*""[^""]*?""\)\s*\])",
					$"[assembly: AssemblyInformationalVersion(\"{assemblyInformationalVersion}\")]");
		}

		/// <summary>
		/// Splits a semicolon separated string.
		/// </summary>
		/// <param name="value">The value to split.</param>
		/// <returns>The separated string.</returns>
		/// <seealso cref="JoinSemicolon"/>
		public static string[] SplitSemicolon(string value) {
			if(string.IsNullOrWhiteSpace(value)) return new string[0];
			return value.Split(new [] {";"}, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => s != string.Empty)
				.ToArray();
		}

		/// <summary>
		/// Joins the values to a semicolon separated string.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>A semicolon separated string.</returns>
		/// <seealso cref="JoinSemicolon"/>
		public static string JoinSemicolon(string[] values) {
			var cleanValues = values?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
			if (cleanValues == null || cleanValues.Length == 0) return null;
			return string.Join(";", cleanValues);
		}
	}

}
