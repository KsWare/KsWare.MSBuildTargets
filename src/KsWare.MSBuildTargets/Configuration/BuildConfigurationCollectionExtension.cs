using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KsWare.MSBuildTargets.Configuration;

namespace KsWare.MSBuildTargets.Internal {

	internal static class BuildConfigurationCollectionExtension {

		/// <summary>
		/// Gets a matching configuration. (configuration and platform must be empty)
		/// </summary>
		/// <param name="col">The configuration.</param>
		/// <param name="target">The target name.</param>
		/// <returns>BuildConfiguration.</returns>
		[CanBeNull]
		public static ConfigurationFile.BuildConfiguration Get(this IEnumerable<ConfigurationFile.BuildConfiguration> col, string target) {
			return col.LastOrDefault(p =>
				IsMatch(target, p.Targets) && 
				p.Configuration == null &&
				p.Platform      == null &&
				p.IsConditionIsTrue);
		}

		/// <summary>
		/// Gets a matching configuration. (platform must be empty)
		/// </summary>
		/// <param name="col">The configuration.</param>
		/// <param name="target">The target name.</param>
		/// <param name="buildConfigurationName">The build configuration name.</param>
		/// <returns>BuildConfiguration.</returns>
		[CanBeNull]
		public static ConfigurationFile.BuildConfiguration Get(this IEnumerable<ConfigurationFile.BuildConfiguration> col, string target, string buildConfigurationName) {
			return col.LastOrDefault(p =>
				IsMatch(target,                 p.Targets       ) &&
				IsMatch(buildConfigurationName, p.Configurations) &&
				p.Platform                                == null &&
				p.IsConditionIsTrue);
		}

		/// <summary>
		/// Gets a matching configuration.
		/// </summary>
		/// <param name="col">The configuration.</param>
		/// <param name="target">The target name.</param>
		/// <param name="buildConfigurationName">The build configuration name.</param>
		/// <param name="platformName">The platform name.</param>
		/// <returns>BuildConfiguration.</returns>
		[CanBeNull]
		public static ConfigurationFile.BuildConfiguration Get(this IEnumerable<ConfigurationFile.BuildConfiguration> col, string target,
			string buildConfigurationName,
			string platformName) {
			return col.LastOrDefault(p => 
				IsMatch(target,                 p.Targets       ) &&
			    IsMatch(buildConfigurationName, p.Configurations) && 
				IsMatch(platformName,           p.Platforms     ) && 
				p.IsConditionIsTrue);
		}

		/// <summary>
		/// Determines whether the specified string matches the second string. null matches null.
		/// </summary>
		/// <param name="a">The string to search.</param>
		/// <param name="b">The string to compare with.</param>
		/// <returns><c>true</c> if the specified a is match; otherwise, <c>false</c>.</returns>
		private static bool IsMatch(string a, string b) {
			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
		}


		/// <summary>
		/// Determines whether the specified string is in collection null matches empty collection
		/// </summary>
		/// <param name="a">The string to search.</param>
		/// <param name="col">The collection to compare with.</param>
		/// <returns><c>true</c> if the specified a is match; otherwise, <c>false</c>.</returns>
		private static bool IsMatch([CanBeNull] string a, [NotNull] IEnumerable<string> col) {
			if (a == null) return !col.Any();
			return col.Contains(a, StringComparer.OrdinalIgnoreCase);
		}

	}

}