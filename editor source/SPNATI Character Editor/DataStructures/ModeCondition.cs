using System.ComponentModel;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	public class ModeCondition
	{
		[DefaultValue("")]
		[XmlAttribute("length")]
		public string Length;

		public ModeCondition() { }

		public override int GetHashCode()
		{
			int hash = (Length ?? "").GetHashCode();
			return hash;
		}

		public ModeCondition Copy()
		{
			ModeCondition copy = new ModeCondition();
			copy.Length = Length;
			return copy;
		}
	}
}
