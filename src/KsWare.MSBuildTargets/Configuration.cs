using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Commands;
using KsWare.MSBuildTargets.Internal;
using NuGet.Versioning;
using N =  KsWare.MSBuildTargets.Program.N;

namespace KsWare.MSBuildTargets {

	public class Configuration {

		private static readonly XmlSerializer Serializer=new XmlSerializer(typeof(Configuration));

		private static readonly string DefaultFileName = "KsWare.MSBuildTargets.config";

		[XmlIgnore]
		public Configuration Parent { get; set; }

		[XmlIgnore]
		public string FileName { get; set; }

		[XmlIgnore]
		public List<Property> GlobalProperties { get; set; }

		[XmlAttribute]
		public string FileVersion { get; set; } = "1.0";

		public string GetProperty(string propertyName) {
			var buildConfigurationName = GlobalProperties.GetValue(N.IDE.ConfigurationName);

			if(GlobalProperties!=null && GlobalProperties.HasProperty(propertyName)) return GlobalProperties.GetValue(propertyName);

			var bc=GetBuildConfiguration(buildConfigurationName, propertyName, true);
			if (bc == null) return null; // no matching build configuration found
			var v = bc.Properties.GetValue(propertyName);
			if (v != null) return v;
			switch (propertyName) {
				case N.NuGet.Pack.OutputDirectory: return Path.GetDirectoryName(GetProperty(N.IDE.TargetPath));
				default: return null;
			}
		}

		private BuildConfiguration GetBuildConfiguration(string buildConfigurationName, string propertyName, bool recursive=false) {
			if (buildConfigurationName == null) {
				var bc = BuildConfigurations.Get(null);
				if (bc !=null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;
				if (bc == null && (!recursive || Parent == null)) return null;
				return Parent?.GetBuildConfiguration(null, propertyName, true);
			}
			else {
				var bc = BuildConfigurations.Get(buildConfigurationName);
				if (bc != null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;
				bc = BuildConfigurations.Get(null);
				if (bc != null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;
				if (bc == null && (!recursive || Parent == null)) return null;
				return Parent?.GetBuildConfiguration(buildConfigurationName, propertyName, true);
			}
		}

		[XmlElement("BuildConfiguration")]
		public List<BuildConfiguration> BuildConfigurations { get; set; } = new List<BuildConfiguration>();

		

		public static Configuration Load(string directory) {
			var path = Path.Combine(directory, DefaultFileName);
			if (!File.Exists(path)) return null;
			Configuration configuration;
			using (var r = File.OpenText(path)) configuration = (Configuration) Serializer.Deserialize(r);
			configuration.FileName = path;
			return configuration;
		}

		public static Configuration LoadRecursive(string directory) {
			var list = new List<Configuration>();
			var d    = directory;
			while (true) {
				var cfg =Load(d);
				if(cfg !=null) list.Add(cfg);
				d = Path.GetDirectoryName(d);
				if(d ==null) break;
			}
			list.Reverse();
			for (int i = list.Count-1; i > 0; i--) {
				list[i].Parent = list[i - 1];
			}
			foreach (var conf in list) {
				foreach (var buildConfiguration in conf.BuildConfigurations) {
					buildConfiguration.Parent = conf;
				}
			}
			return list.LastOrDefault();
		}

		public static void SaveSample() {
			var conf=new Configuration {
				BuildConfigurations = {
					new BuildConfiguration {
						Properties = {
							new Property("OutputDirectory", @"D:\Develop\Packages"),
							new Property("ApiKey", @"01234567890ABCDEF01234567890ABCDEF01234567890ABCDEF"),
							new Property("Complex", "a \"complex\" value\r\nmultiline and \"   \" multiple spaces.")
						}
					},
					new BuildConfiguration {
						Configuration = "Debug",
						Commands = {
							"nuget pack $$ -suffix $IncrementCI$"
						}
					},
					new BuildConfiguration {
						Configuration = "Release",
						Commands = {
							"nuget pack $$ -version $IncrementPatch$",
							"nuget push $$ -apikey $ApiKey$"
						}
					}
				}
			};
			var path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			path = Path.Combine(path, DefaultFileName);
			using (var w=File.CreateText(path)) Serializer.Serialize(w,conf);
		}

		public List<string> GetCommands(string buildConfigurationName, bool recursive = false) {

			if (buildConfigurationName == null) {
				var bc = BuildConfigurations.Get(null);
				if (bc !=null && bc.Commands.Count>0) return bc.Commands;
				if (bc == null && (!recursive || Parent == null)) return null;
				return Parent.GetCommands(null, true);
			}
			else {
				var bc = BuildConfigurations.Get(buildConfigurationName);
				if (bc != null && bc.Commands.Count > 0) return bc.Commands;
				if (bc == null) bc = BuildConfigurations.Get(null);
				if (bc != null && bc.Commands.Count > 0) return bc.Commands;
				if (bc == null && (!recursive || Parent == null)) return null;
				return Parent.GetCommands(null, true);
			}
		}
	}

}