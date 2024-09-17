using SPNATI_Character_Editor.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	/// <summary>
	/// Data representation of a single dialogue stage
	/// </summary>
	public class Stage
	{
		public const int Default = 99;

		[XmlAttribute("id")]
		public int Id;

		[XmlSortMethod("SortCases")]
		[XmlElement("case")]
		public List<Case> Cases;

		public Stage()
		{
			Cases = new List<Case>();
		}

		public Stage(int id)
		{
			Id = id;
			Cases = new List<Case>();
		}
	}
}
