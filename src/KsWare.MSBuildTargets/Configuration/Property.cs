using System.ComponentModel;
using System.Xml.Serialization;

namespace KsWare.MSBuildTargets.Configuration {

	public partial class ConfigurationFile {

		/// <summary>
		/// Provides a property (A name value pair).
		/// </summary>
		public class Property {

			private string _value;

			/// <summary>
			/// Initializes a new instance of the <see cref="Property"/> class.
			/// </summary>
			public Property() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="Property"/> class.
			/// </summary>
			/// <param name="name">The name.</param>
			/// <param name="value">The value.</param>
			public Property(string name, string value) {
				Name  = name;
				Value = value;
			}

			/// <summary>
			/// Gets or sets the name of the property.
			/// </summary>
			/// <value>The name.</value>
			[XmlAttribute("Name")]
			public string Name { get; set; }

			/// <summary>
			/// Gets or sets the value.
			/// </summary>
			/// <value>The value.</value>
			[XmlAttribute("Value")]
			public string Value { get => _value != null && !_value.Contains("\"") ? _value : null; set => _value = value; }

			/// <summary> Internal used by <seealso cref="XmlSerializer"/> </summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("Property.Value")]
			public string PropertyValue {
				get => _value != null && _value.Contains("\"") ? _value : null;
				set => _value = value;
			}

			/// <summary> Internal used by <seealso cref="XmlSerializer"/> </summary>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public bool ShouldSerializePropertyValue() { return _value != null && _value.Contains("\""); }

			/// <summary>
			/// Returns a <see cref="System.String" /> that represents this property.
			/// </summary>
			/// <returns>A <see cref="System.String" /> that represents this property.</returns>
			public override string ToString() {
				var v = Value == null ? "NULL" : $"\"{Value}\"";
				return $"{Name} = {v}";
			}
		}

	}

}