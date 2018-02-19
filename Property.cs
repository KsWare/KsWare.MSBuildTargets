using System.ComponentModel;
using System.Xml.Serialization;

namespace KsWare.MSBuildTargets {

	public class Property {

		private string _value;

		public Property() { }

		public Property(string name, string value) {
			Name  = name;
			Value = value;
		}

		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("Value")]
		public string Value { get => _value != null && !_value.Contains("\"") ? _value : null; set => _value = value; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("Property.Value")]
		public string PropertyValue {
			get => _value != null && _value.Contains("\"") ? _value : null;
			set => _value = value;
		}

		public bool ShouldSerializePropertyValue() { return _value != null && _value.Contains("\""); }

		public override string ToString() {
			var v=Value == null ? "NULL" : $"\"{Value}\"";
			return $"{Name} = {v}";
		}
	}

}