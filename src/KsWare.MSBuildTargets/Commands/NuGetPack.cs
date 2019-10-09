using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Configuration;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Commands {

	public class NuGetPack {

		private readonly ConfigurationFile _configuration;
		private List<string> _arguments;

		public NuGetPack(string[] args, ConfigurationFile configuration) {
			_configuration = configuration;
			_arguments = args.ToList();
			Helper.ExpandVariables(_arguments, "NuGet.Pack");
			Source = _arguments[1];
			for (int i = 2; i < _arguments.Count; i++) {
				var arg = _arguments[i];
				switch (true) {
					case bool _ when arg.Eq(N.NuGet.Pack.OutputDirectory): OutputDirectory = _arguments[++i]; break;
				}
			}
			ExpandSpecialVariables();
		}

		private NuGetPack() {
			
		}

		[XmlIgnore]
		public string Command { get; set; } = "pack";

		[XmlAttribute]
		public string Source { get; set; }

		[XmlAttribute]
		public string OutputDirectory { get; set; }

		[XmlAttribute]
		public string ApiKey { get; set; }

		[XmlAttribute]
		public string Version { get; set; }

		public string VersionExpanded {
			get {
				switch (Version ?? String.Empty) {
					case "":
						return Version;
					case "IncrementPatch":
						return Helper.IncrementPatch(Source,OutputDirectory).ToFullString();
					case "IncrementCI":
						return Helper.IncrementSuffixCI(Source, OutputDirectory).ToFullString();
					default:
						return Version;
				}
			}
		}

		[XmlAttribute]
		public string Suffix { get; set; }

		public string SuffixExpanded {
			get {
				switch (Suffix ?? String.Empty) {
					default:
						return Suffix;
				}
			}
		}

		public string[] AsNugetArgumentList {
			get {
				var list = new List<string>();

				list.Add(Command);

				if (Source != null) {
					list.Add(Source);
				}

				if (OutputDirectory != null) {
					list.Add("-OutputDirectory");
					list.Add(OutputDirectory);
				}
				if (Version != null) {
					list.Add("-Version");
					list.Add(VersionExpanded);
				}
				if (Suffix != null) {
					list.Add("-Suffix");
					list.Add(SuffixExpanded);
				}

				foreach (var option in Options)
					list.Add(option);

				return list.ToArray();
			}
		}


		[XmlIgnore]
		public List<string> Options { get; set; } = new List<string>();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("Options")]
		public string OptionsXml {
			get => Helper.JoinSpaceSeparatedVerbatimString(Options);
			set => Options = String.IsNullOrWhiteSpace(value) ? new List<string>() : Helper.SplitSpaceSeparatedVerbatimString(value);
		}

		public IEnumerable<string> Arguments => _arguments;

		public static NuGetPack Join(List<NuGetPack> configurations) {
			var joined = new NuGetPack();
			foreach (var conf in configurations) {
				if (!string.IsNullOrWhiteSpace(conf.Command)) joined.Command                 = conf.Command;
				if (!string.IsNullOrWhiteSpace(conf.Source)) joined.Source                   = conf.Source;
				if (!string.IsNullOrWhiteSpace(conf.OutputDirectory)) joined.OutputDirectory = conf.OutputDirectory;
				if (!string.IsNullOrWhiteSpace(conf.Version)) joined.Version                 = conf.Version;
				if (!string.IsNullOrWhiteSpace(conf.Suffix)) joined.Suffix                   = conf.Suffix;
				joined.Options.AddRange(conf.Options);
			}
			return joined;
		}

		
		private void ExpandSpecialVariables() {
			var outputDirectory = OutputDirectory ?? Helper.Configuration.GetProperty(N.NuGet.Pack.OutputDirectory); //TODO check null
			var source = Helper.Configuration.GetProperty(N.IDE.TargetPath);
			for (int i = 0; i < _arguments.Count; i++) {
				switch (_arguments[i].ToLowerInvariant()) {
					case "$incrementci$": _arguments[i] = Helper.IncrementSuffixCI(source, outputDirectory).ToFullString(); break;
					case "$incrementpatch$": _arguments[i] = Helper.IncrementPatch(source, outputDirectory).ToFullString(); break;
				}
			}
		}

	}

}