using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace KsWare.MSBuildTargets.Configuration {

	public partial class ConfigurationFile {

		private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigurationFile));

		private static readonly string DefaultFileName = "KsWare.MSBuildTargets.config";

		public static ConfigurationFile Instance { get; set; }

		public static ConfigurationFile Load(string directory) {
			var path = Path.Combine(directory, DefaultFileName);
			if (!File.Exists(path)) return null;
			ConfigurationFile configuration;
			using (var r = File.OpenText(path)) configuration = (ConfigurationFile) Serializer.Deserialize(r);
			configuration.FileName = path;
			return configuration;
		}

		public static ConfigurationFile LoadRecursive(string directory, List<Property> globalProperties) {
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
			var cf= list.Last();
			cf.GlobalProperties = globalProperties;
			Instance = cf;
			return cf;
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


	}

}
