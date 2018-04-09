using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KsWare.MSBuildTargets.Internal {

	public class ConditionParser {

		public static bool IsTrue(string condition) {
			if (string.IsNullOrWhiteSpace(condition)) return true;
			var match=Regex.Match(condition, @"(?isnx-m:(?<a>(?<prefix>[$%]?)\w+(\k<prefix>))(?<op>\s+eq\s+|\s+ne\s+|\s*==\s*|\s*!=\s*)(?<b>(?<prefix>[$%]?)\w+(\k<prefix>)))",RegexOptions.Compiled);
			if (!match.Success) return false;
			var c = new Condition(match.Groups["a"].Value, match.Groups["op"].Value, match.Groups["b"].Value);
			return c.IsTrue;
		}

		private class Condition {

			public Condition() { }

			public Condition(string a, string op, string b) {
				A = a;
				Op = op;
				B = b;
			}
			public string A { get; set; }
			public string Op { get; set; }
			public string B { get; set; }

			public bool IsTrue {
				get {
					switch (Op) {
						case "==": case "eq": return Expand(A) == Expand(B);
						case "!=": case "ne": return Expand(A) != Expand(B);
						default: throw new InvalidOperationException();
					}
				}
			}

			private string Expand(string s) {
				var s1 = Environment.ExpandEnvironmentVariables(s);
				return s1;
			}
		}
	}
}
