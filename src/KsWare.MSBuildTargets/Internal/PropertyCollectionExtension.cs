using System;
using System.Collections.Generic;
using System.Linq;

namespace KsWare.MSBuildTargets.Internal {

	public static class PropertyCollectionExtension {

		public static void Set(this IList<Property> col, string propertyName, string propertyValue) {
			var p = col.Get(propertyName);
			if(p==null) col.Add(new Property(propertyName, propertyValue));
			else p.Value=propertyValue;
		}

		public static Property Get(this IEnumerable<Property> col, string propertyName) {
			return col.LastOrDefault(p =>
				string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static string GetValue(this IEnumerable<Property> col, string propertyName) {
			return col.LastOrDefault(p => string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0)?.Value;
		}

		public static bool HasProperty(this IEnumerable<Property> col, string propertyName) {
			return col.Any(p => string.Compare(propertyName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);
		}


	}

}