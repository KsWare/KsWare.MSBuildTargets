using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.MSBuildTargets.Internal {

	public static class StringListExtension {

		/// <summary>
		/// Contains with StringComparison.OrdinalIgnoreCase
		/// </summary>
		public static bool Co(this IEnumerable<string> list, string s) {
			foreach (var arg in list) {
				if (arg.Eq(s)) return true;
			}

			return false;
		}

		public static string GetArgument(this IEnumerable<string> args, string name) {
			var found = false;
			foreach (var arg in args) {
				if (found) return arg;
				if (arg.Eq(name)) found = true;
			}

			return null;
		}

	}

	public static class StringExtensions {


		/// <summary>
		/// Equals with StringComparison.OrdinalIgnoreCase
		/// </summary>
		public static bool Eq(this string a, string b) {
			if (a == null || b == null) return false;
			return a.Equals(b, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Contains with StringComparison.OrdinalIgnoreCase
		/// </summary>
		public static bool Co(this string a, string b) {
			if (a == null || b == null) return false;
			return a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		/// <summary>
		/// StartsWith with StringComparison.OrdinalIgnoreCase
		/// </summary>
		public static bool Sw(this string a, string b) {
			if (a == null || b == null) return false;
			return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// EndsWith with StringComparison.OrdinalIgnoreCase
		/// </summary>
		public static bool Ew(this string a, string b) {
			if (a == null || b == null) return false;
			return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
		}

	}

}
