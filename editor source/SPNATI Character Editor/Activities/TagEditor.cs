using Desktop;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Character), 2)]
	[Activity(typeof(Costume), 2)]
	[Tutorial("https://www.youtube.com/watch?v=48ak4Ao6p5I")]
	public partial class TagEditor : Activity
	{
		private ISkin _character;
		private BindableTagList _bindings;
		private bool _pendingWardrobeChange;
		private bool _initialized;

		public TagEditor()
		{
			InitializeComponent();
		}

		public override string Caption
		{
			get { return "Tags"; }
		}

		protected override void OnInitialize()
		{
			_character = Record as ISkin;
			SubscribeWorkspace(WorkspaceMessages.WardrobeUpdated, OnWardrobeChanged);
			SubscribeWorkspace<IWardrobe>(WorkspaceMessages.SkinChanged, SkinChanged);
		}

		protected override void OnFirstActivate()
		{
			LoadTags();
			_initialized = true;
			_pendingWardrobeChange = false;
		}

		protected override void OnActivate()
		{
			if (_initialized)
			{
				string g = _character.Gender;
				bool isFuta = IsFutaSkin(_character, g);

				if (_lastGender != g || _lastIsFuta != isFuta)
				{
					RebuildGroupList();
					PopulateData();
				}
			}
			else if (_pendingWardrobeChange)
			{
				PopulateData();
			}
		}

		private void OnWardrobeChanged()
		{
			_pendingWardrobeChange = true;
		}

		private void SkinChanged(IWardrobe costume)
		{
			tagGrid.Refresh();
		}

		/// <summary>
		/// Populates the Tags grid with the character's tags
		/// </summary>
		private void LoadTags()
		{
			TagDictionary dictionary = TagDatabase.Dictionary;
			_bindings = new BindableTagList(_character);

			foreach (Tag tag in dictionary.Tags)
			{
				_bindings.Add(tag.Value);
			}

			RebuildGroupList();
			PopulateData();
			if (toc.Items.Count > 0)
			{
				toc.SelectedIndex = 0;
			}
		}

		// Rebuild the tag groups if the last gender changes

		private string _lastGender;
		private bool _lastIsFuta;

		private void RebuildGroupList()
		{
			TagDictionary dictionary = TagDatabase.Dictionary;

			string gender = _character.Gender;
			bool isFuta = IsFutaSkin(_character, gender);
			_lastGender = gender;
			_lastIsFuta = isFuta;

			// Remember current selection
			string selectedLabel = (toc.SelectedItem as TagGroup)?.Label;

			toc.Items.Clear();

			foreach (TagGroup group in dictionary.Groups)
			{
				if (group.Hidden) continue;

				if (string.IsNullOrEmpty(group.Gender))
				{
					toc.Items.Add(group);
					continue;
				}

				if (group.Gender == "female" && gender != "male")
				{
					toc.Items.Add(group);
					continue;
				}

				if (group.Gender == "male" && (gender == "male" || isFuta))
				{
					toc.Items.Add(group);
					continue;
				}
			}

			// Restore selection if possible
			if (!string.IsNullOrEmpty(selectedLabel))
			{
				for (int i = 0; i < toc.Items.Count; i++)
				{
					if ((toc.Items[i] as TagGroup)?.Label == selectedLabel)
					{
						toc.SelectedIndex = i;
						break;
					}
				}
			}
		}
		private bool IsFutaSkin(ISkin skin, string gender)
		{
			if (gender == "male") return false;

			// Character futa = has a penis size defined
			if (skin is Character ch)
			{
				return !string.IsNullOrWhiteSpace(ch.Penis) && !string.IsNullOrWhiteSpace(ch.Breasts);
			}

			// Costume futa = use costume override if present, else inherit from character
			if (skin is Costume co)
			{
				string penis = !string.IsNullOrWhiteSpace(co.Penis) ? co.Penis : co.Character.Penis;
				string breasts = !string.IsNullOrWhiteSpace(co.Breasts) ? co.Breasts : co.Character.Breasts;

				return !string.IsNullOrWhiteSpace(penis) && !string.IsNullOrWhiteSpace(breasts);
			}

			return false;
		}

		private void PopulateData()
		{
			tagList.SetData(_bindings, _character);

			tagGrid.SetCharacter(_character, _bindings);
			tagGrid.Visible = _initialized;
		}

		public override void Save()
		{
			SaveTags();
		}

		/// <summary>
		/// Saves the Tags grid into the current character
		/// </summary>
		private void SaveTags()
		{
			_bindings.SaveIntoCharacter();
			_character.IsDirty = true;
		}

		private void toc_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TagGroup group = toc.SelectedItem as TagGroup;
			tagGrid.SetGroup(group);
			tagGrid.Visible = true;
		}
	}
}
