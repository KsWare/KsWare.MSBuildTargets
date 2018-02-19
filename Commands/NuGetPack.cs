using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Internal;
using NuGet.Versioning;

namespace KsWare.MSBuildTargets.Commands {

	public class NuGetPack {

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
				switch (Version ?? string.Empty) {
					case "":
						return Version;
					case "IncrementPatch":
						return Helper.AutoIncrementPatch(Source,OutputDirectory);
					default:
						return Version;
				}
			}
		}

		[XmlAttribute]
		public string Suffix { get; set; }

		public string SuffixExpanded {
			get {
				switch (Suffix ?? string.Empty) {
					case "":
						return Suffix;
					case "IncrementCI":
						return Helper.AutoIncrementSuffixCI(Source, OutputDirectory);
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
			get => Helper.JoinSpaceSeperatedVerbatimString(Options);
			set => Options = string.IsNullOrWhiteSpace(value) ? new List<string>() : Helper.SplitSpaceSeperatedVerbatimString(value);
		}

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
	}

}