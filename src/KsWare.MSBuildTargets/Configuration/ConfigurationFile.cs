using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using JetBrains.Annotations;
using KsWare.MSBuildTargets.Internal;
using N =  KsWare.MSBuildTargets.Program.N;

namespace KsWare.MSBuildTargets.Configuration {

	public partial class ConfigurationFile {

		[XmlIgnore]
		public List<Property> GlobalProperties { get; set; }

		public string GetProperty(string propertyName) {
			var buildConfigurationName = GlobalProperties.GetValue(N.IDE.ConfigurationName);
			var target                 = GlobalProperties.GetValue(N.Target);

			if(GlobalProperties!=null && GlobalProperties.HasProperty(propertyName)) return GlobalProperties.GetValue(propertyName);

			var bc=GetBuildConfiguration(target, buildConfigurationName, propertyName, true);
			if (bc == null) return null; // no matching build configuration found
			var v = bc.Properties.GetValue(propertyName);
			if (v != null) return v;
			switch (propertyName) {
				case N.NuGet.Pack.OutputDirectory: return Path.GetDirectoryName(GetProperty(N.IDE.TargetPath));
				case N.NuGet.Push.Source         : return "https://api.nuget.org/v3/index.json";
				default: return null;
			}
		}
	}


	[XmlRoot("Configuration")]
	public partial class ConfigurationFile {

		private static readonly XmlSerializer Serializer=new XmlSerializer(typeof(ConfigurationFile));

		private static readonly string DefaultFileName = "KsWare.MSBuildTargets.config";

		[XmlIgnore]
		public ConfigurationFile Parent { get; set; }

		[XmlIgnore]
		public string FileName { get; set; }

		[XmlAttribute]
		public string SchemaVersion { get; set; } = "1.1";

		private BuildConfiguration GetBuildConfiguration(string target, string buildConfigurationName, string propertyName, bool recursive=false) {
			if(target==null && buildConfigurationName!=null) throw new ArgumentNullException(nameof(target));
			BuildConfiguration bc;
			if (target != null && buildConfigurationName != null) goto A;
			if (target != null) goto B;
			goto C;

			A:
				bc = BuildConfigurations.Get(target, buildConfigurationName);
				if (bc != null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;
			B:
				bc = BuildConfigurations.Get(target, null);
				if (bc != null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;
			C:
				bc = BuildConfigurations.Get(null, null);
				if (bc != null && propertyName == null) return bc;
				if (bc != null && bc.Properties.HasProperty(propertyName)) return bc;

				if (bc == null && !recursive || Parent == null) return null;
				return Parent?.GetBuildConfiguration(target, buildConfigurationName, propertyName, true);
		}

		[XmlElement("BuildConfiguration")]
		public List<BuildConfiguration> BuildConfigurations { get; set; } = new List<BuildConfiguration>();

		public static ConfigurationFile Load(string directory) {
			var path = Path.Combine(directory, DefaultFileName);
			if (!File.Exists(path)) return null;
			ConfigurationFile configuration;
			using (var r = File.OpenText(path)) configuration = (ConfigurationFile) Serializer.Deserialize(r);
			configuration.FileName = path;
			return configuration;
		}

		public static ConfigurationFile LoadRecursive(string directory) {
			var list = new List<ConfigurationFile>();
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
			var conf=new ConfigurationFile {
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

		[NotNull]
		[SuppressMessage("ReSharper", "FlagArgument", Justification = "ignore")]
		[SuppressMessage("ReSharper", "MethodTooLong", Justification = "ignore")]
		public List<Command> GetCommands([CanBeNull] string target, [CanBeNull] string buildConfigurationName, bool recursive = false) {
			if(target==null && buildConfigurationName != null) throw new ArgumentNullException(nameof(target));

			BuildConfiguration bc = null;

			// target + buildConfigurationName
			// target
			// -

			if (target != null && buildConfigurationName != null) goto A;
			if (target != null) goto B;
			goto C;

			A:
			if (bc == null) bc = BuildConfigurations.Get(target, buildConfigurationName);
			if (bc != null && bc.Commands.Count > 0) return bc.Commands;
			B:
			if (bc == null) bc = BuildConfigurations.Get(target, null);
			if (bc != null && bc.Commands.Count > 0) return bc.Commands;
			C:
			if (bc == null) bc = BuildConfigurations.Get(null, null);
			if (bc != null && bc.Commands.Count > 0) return bc.Commands;

			if (bc == null && !recursive || Parent == null) return new List<Command>();
			return Parent.GetCommands(target, buildConfigurationName, true);
		}
	}

}