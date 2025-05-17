using Desktop.CommonControls.PropertyControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Linq;


namespace SPNATI_Character_Editor
{
	public class PoseSet : ICloneable, IComparable<PoseSet>
	{
		[Text(DisplayName = "ID", GroupOrder = 0)]
		[XmlAttribute("id")]
		public string Id;

		[XmlElement("pose")]
		public List<PoseSetEntry> Entries = new List<PoseSetEntry>();

		public PoseSet()
		{
			Id = "new set";
		}

		public object Clone()
		{
			PoseSet poseSet = MemberwiseClone() as PoseSet;
			poseSet.Entries = new List<PoseSetEntry>();
			foreach (PoseSetEntry entry in Entries)
			{
				PoseSetEntry clonedEntry = entry.Clone() as PoseSetEntry;
				poseSet.Entries.Add(clonedEntry);
			}
			return poseSet;
		}

		public int CompareTo(PoseSet other)
		{
			return Id.CompareTo(other.Id);
		}

		public override string ToString()
		{
			return Id;
		}

		public bool HasCoverageForStages(List<int> caseStages)
		{
			var coveredStages = Entries.SelectMany(entry => ParseStageRange(entry.Stage)).ToHashSet();
			return caseStages.All(stage => coveredStages.Contains(stage));
		}


		public PoseSetEntry GetPoseForStages(List<int> caseStages)
		{
			HashSet<int> coveredStages = new HashSet<int>();
			foreach (var entry in Entries)
			{
				List<int> entryStages = ParseStages(entry.Stage);
				foreach (int s in entryStages)
					coveredStages.Add(s);
			}

			if (!caseStages.All(stage => coveredStages.Contains(stage)))
				return null;

			foreach (var entry in Entries)
			{
				List<int> entryStages = ParseStages(entry.Stage);
				if (caseStages.Any(stage => entryStages.Contains(stage)))
					return entry;
			}

			return null;
		}

		private IEnumerable<int> ParseStageRange(string stageRange)
		{
			if (string.IsNullOrEmpty(stageRange)) yield break;

			foreach (var part in stageRange.Split(','))
			{
				if (part.Contains('-'))
				{
					var bounds = part.Split('-');
					if (int.TryParse(bounds[0], out int start) && int.TryParse(bounds[1], out int end))
					{
						for (int i = start; i <= end; i++)
							yield return i;
					}
				}
				else if (int.TryParse(part, out int singleStage))
				{
					yield return singleStage;
				}
			}
		}

		private List<int> ParseStages(string stageRange)
		{
			List<int> stages = new List<int>();

			if (string.IsNullOrEmpty(stageRange))
				return stages;

			foreach (var part in stageRange.Split(','))
			{
				if (part.Contains("-"))
				{
					var bounds = part.Split('-');
					if (int.TryParse(bounds[0], out int start) && int.TryParse(bounds[1], out int end))
					{
						for (int i = start; i <= end; i++)
						{
							stages.Add(i);
						}
					}
				}
				else if (int.TryParse(part, out int singleStage))
				{
					stages.Add(singleStage);
				}
			}

			return stages;
		}


	}

	public class PoseSetEntry : ICloneable 
	{
		[XmlAttribute("img")]
		public string Img;

		[XmlIgnore]
		public string Character;

		private string _stage;
		[StageSelect(DisplayName = "Stage", GroupName = "Conditions", GroupOrder = 2, Description = "Stage (Required)", BoundProperties = new string[] { "Character" }, FilterStagesToTarget = false, SkinVariable = "~self.costume~")]
		[XmlAttribute("stage")]
		public string Stage
		{
			get { return _stage; }
			set
			{
				_stage = value ?? "0";
			}
		}

		[DefaultValue("")]
		[XmlAttribute("location")]
		public string Location;

		[DefaultValue("")]
		[XmlAttribute("direction")]
		public string Direction;

		[DefaultValue("")]
		[XmlAttribute("dialogue-layer")]
		public string DialogueLayer;

		[DefaultValue(0)]
		[XmlAttribute("priority")]
		public int Priority;

		[DefaultValue(0F)]
		[XmlAttribute("weight")]
		public float Weight;

		[DefaultValue("")]
		[XmlAttribute("z-index")]
		public string ZIndexPoseSet;

		[Expression(DisplayName = "Variable Test (+)", GroupName = "Conditions", GroupOrder = 1, Description = "Tests the value of a variable. Multiple can be added")]
		[XmlArray("tests")]
		[XmlArrayItem("test")]
		public List<ExpressionTest> Tests = new List<ExpressionTest>();

		public PoseSetEntry() 
		{
		}

		public object Clone()
		{
			PoseSetEntry entry = MemberwiseClone() as PoseSetEntry;
			entry.Tests = new List<ExpressionTest>();
			foreach (ExpressionTest test in Tests)
			{
				ExpressionTest copiedTest = test.Copy();
				entry.Tests.Add(copiedTest);
			}
			return entry;
		}

	}
}