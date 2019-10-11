using System;
using System.Collections.Generic;
using System.Linq;

namespace KsWare.MSBuildTargets.Configuration {

	public static class PropertyCollectionExtension {

		public static void Set(this IList<ConfigurationFile.Property> col, string propertyName, string propertyValue) {
			var p = col.Get(propertyName);
			if(p==null) col.Add(new ConfigurationFile.Property(propertyName, propertyValue));
			else p.Value=propertyValue;
		}

		public static ConfigurationFile.Property Get(this IEnumerable<ConfigurationFile.Property> col, string propertyName) {
			return col.LastOrDefault(p =>
				string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static string GetValue(this IEnumerable<ConfigurationFile.Property> col, string propertyName) {
			return col.LastOrDefault(p => string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0)?.GetValue();
		}

		public static bool HasProperty(this IEnumerable<ConfigurationFile.Property> col, string propertyName) {
			return col.Any(p => string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);
		}


	}

}