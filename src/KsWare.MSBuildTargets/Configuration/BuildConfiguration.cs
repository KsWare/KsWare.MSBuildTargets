using System.Collections.Generic;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Configuration {

	public partial class ConfigurationFile {

		/// <summary>
		/// Providies a build specific configuration. (<see cref="Target"/>, <see cref="Configuration"/>, <see cref="Platform"/>
		/// </summary>
		public class BuildConfiguration {

			/// <summary>
			/// Initializes a new instance of the <see cref="BuildConfiguration"/> class.
			/// </summary>
			public BuildConfiguration() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="BuildConfiguration"/> class.
			/// </summary>
			/// <param name="target">The target.</param>
			public BuildConfiguration(string target) { Target = target; }

			/// <summary>
			/// Initializes a new instance of the <see cref="BuildConfiguration"/> class.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="configuration">The configuration.</param>
			public BuildConfiguration(string target, string configuration) {
				Target        = target;
				Configuration = configuration;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BuildConfiguration"/> class.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="configuration">The configuration.</param>
			/// <param name="platform">The platform.</param>
			public BuildConfiguration(string target, string configuration, string platform) {
				Target        = target;
				Configuration = configuration;
				Platform      = platform;
			}

			/// <summary>
			/// Gets or sets the parent node <see cref="ConfigurationFile"/>.
			/// </summary>
			/// <value>The parent node.</value>
			/// <remarks>The value is set after deserialization.</remarks>
			[XmlIgnore]
			public ConfigurationFile Parent { get; set; }

			/// <summary>
			/// Gets or sets the identifier. [Internaly used]
			/// </summary>
			/// <value>The identifier.</value>
			[XmlIgnore]
			public int Id { get; set; }

			/// <summary>
			/// Gets or sets the build targets for which this configuration is valid.
			/// </summary>
			/// <value>The build targets. One or more of "BeforeBuild;AfterBuild".</value>
			[XmlAttribute]
			public string Target { get; set; }

			/// <summary>
			/// Gets or sets the build targets for which this configuration is valid.
			/// </summary>
			/// <value>The build targets. [BeforeBuild, AfterBuild]</value>
			[XmlIgnore]
			public string[] Targets => Helper.SplitSemicolon(Target);

			/// <summary>
			/// Gets or sets the build configurations for which this configuration is valid.
			/// </summary>
			/// <value>The build configuration. One or more of e.g. "Debug;Release" or empty to match all.</value>
			[XmlAttribute]
			public string Configuration { get; set; }

			/// <summary>
			/// Gets the configurations for which this configuration is valid.
			/// </summary>
			/// <value>The build configuration. One or more of e.g. "Debug" or "Release" or empty to match all.</value>
			[XmlIgnore]
			public string[] Configurations => Helper.SplitSemicolon(Configuration);

			/// <summary>
			/// Gets or sets the platforms for which this configuration is valid.
			/// </summary>
			/// <value>The platforms. One or more of e.g. "Any CPU;x86;x64" or empty to match all.</value>
			[XmlAttribute]
			public string Platform { get; set; }

			/// <summary>
			/// Gets the platforms for which this configuration is valid.
			/// </summary>
			/// <value>The platforms. One or more of e.g. "Any CPU", "x86", "x64" or empty to match all.</value>
			[XmlIgnore]
			public string[] Platforms => Helper.SplitSemicolon(Platform);

			
			[XmlAttribute]
			public string Condition { get; set; }

			
			[XmlIgnore]
			public string[] Conditions => Helper.SplitSemicolon(Configuration);

			[XmlIgnore]
			public bool IsConditionIsTrue => ConditionParser.IsTrue(Condition);

			/// <summary>
			/// Gets or sets the <see cref="Property"/>s.
			/// </summary>
			/// <value>The properties.</value>
			[XmlElement("Property")]
			public List<Property> Properties { get; set; } = new List<Property>();

			/// <summary>
			/// Gets or sets the <see cref="Command"/>s.
			/// </summary>
			/// <value>The commands.</value>
			[XmlElement("Command")]
			public List<Command> Commands { get; set; } = new List<Command>();

			/// <summary>
			/// Returns a <see cref="System.String" /> that represents this instance.
			/// </summary>
			/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
			public override string ToString() {
				return $"{Target} {Configuration} {Platform} Properties:{Properties} Commands:{Commands.Count}";
			}
		}

	}

}