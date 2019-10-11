using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Configuration {

	public partial class ConfigurationFile {

		[XmlIgnore]
		public List<Property> GlobalProperties { get; set; }

		public string GetProperty(string propertyName, string defaultNamespace=null) {
			var v = GetPropertyInternal(propertyName);
			if (v == null && propertyName.Contains(".")) goto Default;
			if (v == null && !propertyName.Contains(".") && defaultNamespace == null) goto Default;
			if (v == null) v = GetPropertyInternal($"{defaultNamespace}.{propertyName}");
			if (v == null) goto Default;
			return v;

			Default:
			var n = propertyName.Contains(".") ? propertyName : $"{defaultNamespace}.{propertyName}";
			switch (true) {
				case bool _ when n.Eq(N.NuGet.Pack.OutputDirectory): return Path.GetDirectoryName(GetProperty(N.IDE.TargetPath));
				case bool _ when n.Eq(N.NuGet.Push.Source): return "https://api.nuget.org/v3/index.json";
				default: return null;
			}
		}

		private string GetPropertyInternal(string propertyName) {
			var buildConfigurationName = GlobalProperties.GetValue(N.IDE.ConfigurationName);
			var target                 = GlobalProperties.GetValue(N.Target);

			if(GlobalProperties!=null && GlobalProperties.HasProperty(propertyName)) return GlobalProperties.GetValue(propertyName);

			var bc=GetBuildConfiguration(target, buildConfigurationName, propertyName, true);
			if (bc == null) return null; // TODO no matching build configuration found
			var v = bc.Properties.GetValue(propertyName);
			if (v != null) return v;
			//TODO unreachable
			switch (propertyName) {
				case N.NuGet.Pack.OutputDirectory: return Path.GetDirectoryName(GetProperty(N.IDE.TargetPath));
				case N.NuGet.Push.Source         : return "https://api.nuget.org/v3/index.json";
				default: return null;
			}
		}


	}


	[XmlRoot("Configuration")]
	public partial class ConfigurationFile {

		[XmlIgnore]
		public ConfigurationFile Parent { get; set; }

		[XmlIgnore]
		public string FileName { get; set; }

		[XmlAttribute]
		public string SchemaVersion { get; set; } = "1.1";

		[SuppressMessage("ReSharper", "FlagArgument", Justification = "ignore")]
		private BuildConfiguration GetBuildConfiguration([CanBeNull] string target, [CanBeNull] string buildConfigurationName, [CanBeNull] string propertyName, bool recursive=false) {
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