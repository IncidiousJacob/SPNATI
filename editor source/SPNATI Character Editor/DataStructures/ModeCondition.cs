using System.ComponentModel;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	public class ModeCondition
	{
		[DefaultValue("")]
		[XmlAttribute("expr")]
		public string Expression;

		public ModeCondition() { }

		public ModeCondition(string expr)
		{
			Expression = expr;
		}

		public override int GetHashCode()
		{
			int hash = (Expression ?? "").GetHashCode();
			return hash;
		}

		public ModeCondition Copy()
		{
			return new ModeCondition(Expression);
		}

		public bool Equals(ModeCondition cond)
		{
			return Expression == cond.Expression;
		}
	}
}
