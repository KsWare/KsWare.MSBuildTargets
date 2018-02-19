using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Commands;

namespace KsWare.MSBuildTargets {

	public class BuildConfiguration {

		public BuildConfiguration() { }

		public BuildConfiguration(string configuration) { Configuration = configuration; }

		public BuildConfiguration(string configuration, string platform) {
			Configuration = configuration;
			Platform = platform;
		}

		[XmlIgnore]
		public Configuration Parent { get; set; }

		[XmlAttribute]
		public string Configuration { get; set; }

		[XmlAttribute]
		public string Platform { get; set; }

		[XmlElement("Property")]
		public List<Property> Properties { get; set; } = new List<Property>();

		[XmlElement("Command")]
		public List<string> Commands { get; set; } = new List<string>();

	}

}