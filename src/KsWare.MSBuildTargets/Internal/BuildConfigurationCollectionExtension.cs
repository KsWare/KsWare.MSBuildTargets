using System;
using System.Collections.Generic;
using System.Linq;

namespace KsWare.MSBuildTargets.Internal {

	public static class BuildConfigurationCollectionExtension {

		public static BuildConfiguration Get(this IEnumerable<BuildConfiguration> col, string buildConfigurationName) {
			return col.LastOrDefault(p =>
				string.Compare(buildConfigurationName, p.Configuration, StringComparison.OrdinalIgnoreCase) == 0 &&
				p.Platform                                                                                  == null);
		}

		public static BuildConfiguration Get(this IEnumerable<BuildConfiguration> col, string buildConfigurationName, string platformName) {
			return col.LastOrDefault(p =>
				string.Compare(buildConfigurationName, p.Configuration, StringComparison.OrdinalIgnoreCase) == 0 &&
				string.Compare(platformName,           p.Platform,      StringComparison.OrdinalIgnoreCase) == 0);
		}

	}

}