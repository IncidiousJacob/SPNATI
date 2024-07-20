using System.ComponentModel;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	public class ModeCondition
	{
		[DefaultValue("")]
		[XmlAttribute("expr")]
		public string Expression;

		[DefaultValue("")]
		[XmlAttribute("cmp")]
		public string Operator;

		[DefaultValue("")]
		[XmlAttribute("value")]
		public string Value;

		public ModeCondition() { }

		public ModeCondition(string expr, string cmp, string value)
		{
			Expression = expr;
			Operator = cmp;
			Value = value;
		}

		public override int GetHashCode()
		{
			int hash = (Expression ?? "").GetHashCode();
			hash *= (Operator ?? "").GetHashCode();
			hash *= (Value ?? "").GetHashCode();
			return hash;
		}

		public ModeCondition Copy()
		{
			return new ModeCondition(Expression, Operator, Value);
		}
	}
}
