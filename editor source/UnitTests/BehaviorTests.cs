using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPNATI_Character_Editor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
	[TestClass]
	public class BehaviorTests : IDisposable
	{
		private Character _character;

		[TestInitialize]
		public void Init()
		{
			TriggerDatabase.FakeUsedInStage = UsedInStage;

			_character = new Character();
			for (int i = 0; i < 5; i++)
			{
				_character.Wardrobe.Add(new Clothing());
			}
		}

		private static bool UsedInStage(string tag, Character character, int stage)
		{
			return true;
		}

		private Case CreateWorkingCase(Behaviour behavior, string tag, int[] stages, params string[] lines)
		{
			Case c = new Case(tag);
			foreach (string line in lines)
			{
				c.Lines.Add(new DialogueLine("test", line));
			}
			c.Stages.AddRange(stages);
			behavior.AddWorkingCase(c);
			return c;
		}

		[TestMethod]
		public void ReplaceReplaces()
		{
			Behaviour behavior = _character.Behavior;
			behavior.PrepareForEdit(_character);
			CreateWorkingCase(behavior, "a", new int[] { 0, 1, 2 }, "a1");
			CreateWorkingCase(behavior, "b", new int[] { 0, 1, 2 }, "c");
			HashSet<string> dest = new HashSet<string>();
			dest.Add("b");
			behavior.BulkReplace("a", dest);
			Assert.AreEqual(2, behavior.GetWorkingCases().Count());
			Assert.AreEqual("a1", behavior.GetWorkingCases().ToList()[1].Lines[0].Text);
		}

		[TestMethod]
		public void ReplaceReplacesMultiple()
		{
			Behaviour behavior = _character.Behavior;
			behavior.PrepareForEdit(_character);
			CreateWorkingCase(behavior, "a", new int[] { 0, 1, 2 }, "a1");
			CreateWorkingCase(behavior, "b", new int[] { 0, 1, 2 }, "c");
			HashSet<string> dest = new HashSet<string>();
			dest.Add("b");
			dest.Add("c");
			dest.Add("d");
			behavior.BulkReplace("a", dest);
			Assert.AreEqual(4, behavior.GetWorkingCases().Count());
			Assert.AreEqual("a1", behavior.GetWorkingCases().ToList()[1].Lines[0].Text);
			Assert.AreEqual("a1", behavior.GetWorkingCases().ToList()[2].Lines[0].Text);
			Assert.AreEqual("a1", behavior.GetWorkingCases().ToList()[3].Lines[0].Text);
		}


		public void Dispose()
		{
			_character.Dispose();
		}
	}
}
