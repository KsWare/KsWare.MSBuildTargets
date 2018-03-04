using System.ComponentModel;
using System.Xml.Serialization;
using KsWare.MSBuildTargets.Internal;

namespace KsWare.MSBuildTargets.Configuration {

	/// <summary>
	/// Class ConfigurationFile.
	/// </summary>
	public partial class ConfigurationFile {

		/// <summary>
		/// Provides a executable command.
		/// </summary>
		public class Command {

			/// <summary>
			/// Initializes a new instance of the <see cref="Command" /> class.
			/// </summary>
			public Command() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="Command" /> class.
			/// </summary>
			/// <param name="commandLine">The command line.</param>
			public Command(string commandLine) { CommandLine = commandLine; }

			/// <summary>
			/// Initializes a new instance of the <see cref="Command" /> class.
			/// </summary>
			/// <param name="commandLine">The command line.</param>
			/// <param name="flags">The flags.</param>
			public Command(string commandLine, string flags) {
				FlagsXml = flags;
				CommandLine = commandLine;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Command" /> class.
			/// </summary>
			/// <param name="commandLine">The command line.</param>
			/// <param name="flags">The flags.</param>
			public Command(string commandLine, string[] flags) {
				Flags = flags;
				CommandLine = commandLine;
			}


			/// <summary>
			/// Gets or sets the flags XML.
			/// </summary>
			/// <value>The flags XML.</value>
			[XmlAttribute("Flags")]
			[EditorBrowsable(EditorBrowsableState.Never)]
			public string FlagsXml { get => Helper.JoinSemicolon(Flags); set => Flags = Helper.SplitSemicolon(value); }

			/// <summary>
			/// Gets or sets the flags.
			/// </summary>
			/// <value>The flags.</value>
			[XmlIgnore]
			public string[] Flags { get; set; } = new string[0];

			/// <summary>
			/// Gets or sets the command line. Includes the command and all parameters.
			/// </summary>
			/// <value>The command line.</value>
			[XmlText]
			public string CommandLine { get; set; }

			/// <summary>
			/// Returns a <see cref="System.String" /> that represents this instance.
			/// </summary>
			/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
			public override string ToString() {
				var flags = string.IsNullOrWhiteSpace(FlagsXml) ? "" : "[" + FlagsXml + "] ";
				return $"{flags}{CommandLine}";
			}

			/// <summary>
			/// Performs an implicit conversion from <see cref="System.String" /> to <see cref="Command" />.
			/// </summary>
			/// <param name="commandLine">The command line.</param>
			/// <returns>The result of the conversion.</returns>
			public static implicit operator Command(string commandLine) => new Command(commandLine);
		}
	}

}