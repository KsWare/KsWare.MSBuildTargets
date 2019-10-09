using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Commands {
// https://docs.microsoft.com/de-de/nuget/reference/cli-reference/cli-ref-push

	public sealed class NuGetPush {

		private readonly ConfigurationFile _configuration;
		private List<string> _arguments;

		public NuGetPush(string[] args, ConfigurationFile configuration) {
			_configuration = configuration;
			_arguments = args.ToList();
			AddMandatoryParameters();
			Helper.ExpandVariables(_arguments, "NuGet.Push");
			ExpandSpecialVariables();
			PackagePath = _arguments[1];
			ProcessSource();
		}

		public string PackagePath { get; set; }
		public IEnumerable<string> Arguments => _arguments;

		public void ProcessSource() {
//			var source = _arguments.GetArgument("-Source");
//			if(source.StartsWith("http")) return;
//			var localSource = source.StartsWith("file:") ? new Uri(source,UriKind.Absolute).LocalPath : source;
//			Directory.CreateDirectory(localSource);
		}

		public void AddMandatoryParameters() {
			if (!_arguments.Co("-Source")) _arguments.AddRange(new[] {"-Source", "$Source$"});
			if (!_arguments.Co("-NonInteractive")) _arguments.AddRange(new[] { "-NonInteractive"});
		}


		private void ExpandSpecialVariables() {
			for (int i = 0; i < _arguments.Count; i++) {
				switch (_arguments[i].ToLowerInvariant()) {
					case "$packagepath$": _arguments[i] = Helper.Configuration.GetProperty(N.NuGet.Push.PackagePath); break;
				}
			}
		}

	}

}
