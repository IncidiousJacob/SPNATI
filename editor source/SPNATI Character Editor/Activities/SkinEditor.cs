using Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Costume), 0)]
	[Tutorial("https://www.youtube.com/watch?v=35jIow_jMHE")]
	public partial class SkinEditor : Activity
	{
		private bool _linkDataChanged = false;
		private Costume _costume;
		private bool _populatingImages;
		private bool _exportOnQuit;
		private int _fullyClothedStage;

		public SkinEditor()
		{
			InitializeComponent();

			cboStatus.Items.Add("");
			cboStatus.Items.Add("online");
			cboStatus.Items.Add("offline");
			cboStatus.Items.Add("unlisted");
			cboGender.Items.AddRange(new string[] { "female", "male" });
			cboEvent.Items.AddRange(new string[] { "", "none", "valentines", "april_fools", "easter", "summer", "halloween", "xmas", "sleepover" });

			cboSize.Items.AddRange(new string[] { "small", "medium", "large" });
			cboFutanariPenisSize.Items.AddRange(new string[] { "", "small", "medium", "large" });

			cboGender.SelectedIndexChanged += cboGender_SelectedIndexChanged;
			cmdExpandFutanariSize.Click += cmdExpandFutanariSize_Click;
		}

		public override string Caption
		{
			get { return "General"; }
		}

		protected override void OnInitialize()
		{
			_costume = Record as Costume;
			SubscribeWorkspace<bool>(WorkspaceMessages.Save, OnSaveWorkspace);
			SubscribeWorkspace<int>(WorkspaceMessages.SkipLayersChanged, OnSkipLayersChanged);
		}

		private void OnSaveWorkspace(bool auto)
		{
			if (!auto)
			{
				Save();
				if (Serialization.ExportSkin(_costume))
				{
					Shell.Instance.SetStatus(string.Format("{0} exported successfully at {1}.", _costume, DateTime.Now.ToShortTimeString()));
				}
				else
				{
					Shell.Instance.SetStatus(string.Format("{0} failed to export.", _costume));
				}
			}
		}

		private void OnSkipLayersChanged(int layers)
		{
			valLayers.Value = Math.Max(valLayers.Minimum, Math.Min(layers, valLayers.Maximum));
			_costume.LayersNonSkip = layers;
			_costume.Link.LayersNonSkip = (int)valLayers.Value == _costume.Layers ? 0 : (int)valLayers.Value;
			int stage = 0;
			while (_costume.Character.LayerToStageName(stage, _costume).ToString() != "Fully Clothed")
			{
				stage++;
			}
			_fullyClothedStage = stage;
		}

		private void cboGender_SelectedIndexChanged(object sender, EventArgs e)
		{
			string gender = cboGender.SelectedItem?.ToString();

			bool isMale = gender == "male";

			lblSize.Text = isMale ? "Penis:" : "Breasts:";

			// Only allow futa options on female
			cmdExpandFutanariSize.Visible = !isMale;

			if (isMale)
			{
				// hide and clear futa controls
				lblFutanariPenisSize.Visible = false;
				cboFutanariPenisSize.Visible = false;
				cboFutanariPenisSize.SelectedIndex = 0; // ""
			}
			else
			{
				// keep futa hidden until user expands
				lblFutanariPenisSize.Visible = false;
				cboFutanariPenisSize.Visible = false;
			}
		}

		private string GetBaseMalePenisSize()
		{
			// Mirrors MetadataEditor logic
			return string.IsNullOrEmpty(_costume.Character.LegacySize)
				? _costume.Character.Penis
				: _costume.Character.LegacySize;
		}

		private string GetBaseFemaleBreastSize()
		{
			// Mirrors MetadataEditor logic
			return string.IsNullOrEmpty(_costume.Character.LegacySize)
				? _costume.Character.Breasts
				: _costume.Character.LegacySize;
		}

		private string GetBaseFutaPenisSize()
		{
			// Base futa size is just the character penis size (if any)
			return _costume.Character.Penis;
		}


		private void cmdExpandFutanariSize_Click(object sender, EventArgs e)
		{
			// only meaningful for female
			if (cboGender.SelectedItem?.ToString() == "male")
				return;

			lblFutanariPenisSize.Visible = true;
			cboFutanariPenisSize.Visible = true;
		}


		private void LinkCharacter()
		{
			Character character = RecordLookup.DoLookup(typeof(Character), "", false, _costume) as Character;
			if (character != null)
			{
				_costume.LinkCharacter(character);
			}
		}

		protected override void OnFirstActivate()
		{
			if (_costume.Character == null)
			{
				LinkCharacter();
			}

			SkinLink link = _costume.Link;
			if (link != null)
			{
				txtName.Text = link.CostumeName;
				cboStatus.Text = link.Status;
				cboEvent.Text = link.Set;
				txtDescription.Text = link.CostumeDescription;
				string gender = link.Gender ?? _costume.Character.Gender;
				cboGender.SelectedItem = gender;
				cboGender_SelectedIndexChanged(null, EventArgs.Empty);

				if (gender == "male")
				{
					string basePenis = GetBaseMalePenisSize();
					string effectivePenis = string.IsNullOrEmpty(_costume.Penis) ? basePenis : _costume.Penis;

					cboSize.SelectedItem = effectivePenis;
					cboFutanariPenisSize.SelectedIndex = 0; // ""
				}
				else
				{
					string baseBreasts = GetBaseFemaleBreastSize();
					string effectiveBreasts = string.IsNullOrEmpty(_costume.Breasts) ? baseBreasts : _costume.Breasts;

					cboSize.SelectedItem = effectiveBreasts;

					// Futa: show it if either the costume overrides it OR the base character has it
					string baseFuta = GetBaseFutaPenisSize();
					string effectiveFuta = !string.IsNullOrEmpty(_costume.Penis) ? _costume.Penis : baseFuta;

					if (!string.IsNullOrEmpty(effectiveFuta))
					{
						lblFutanariPenisSize.Visible = true;
						cboFutanariPenisSize.Visible = true;
						cboFutanariPenisSize.SelectedItem = effectiveFuta;
					}
					else
					{
						cboFutanariPenisSize.SelectedIndex = 0; // ""
					}
				}

				valLayers.Value = link.LayersNonSkip != 0 ? Math.Max(valLayers.Minimum, Math.Min(link.LayersNonSkip, valLayers.Maximum)) : Math.Max(valLayers.Minimum, Math.Min(_costume.Character.Metadata.Layers, valLayers.Maximum));
			}

			int stage = 0;
			while(_costume.Character.LayerToStageName(stage, _costume).ToString() != "Fully Clothed")
			{
				stage++;
			}
			_fullyClothedStage = stage;

			cboBaseStage.Items.Add("- None -");
			for (int i = 0; i < _costume.Layers + Clothing.ExtraStages; i++)
			{
				cboBaseStage.Items.Add(_costume.Character.LayerToStageName(i, _costume));
			}

			//if anyone tries to get fancy by linking to multiple folders instead of just the reskin and the base, sorry, but we're not handling it for now
			string baseFolder = $"opponents/{_costume.Character.FolderName}/";
			StageSpecificValue baseStage = _costume.Folders.Find(f => f.Value == baseFolder);
			if (baseStage != null)
			{
				cboBaseStage.SelectedIndex = baseStage.Stage + 1;
			}
			else
			{
				cboBaseStage.SelectedIndex = -1;
			}

			gridLabels.Set(_costume.Labels);

			if (_costume.Description != null)
			{
				txtDescription.Text = _costume.Description.Replace("<br>", Environment.NewLine);
			}

			PopulatePortraitDropdown();
			if (_costume.Link?.PreviewImage != null)
			{
				string portrait = _costume.Link.PreviewImage;
				PoseMapping pose = _costume.PoseLibrary.GetPose(portrait);
				cboDefaultPic.SelectedItem = pose;
			}

			var othernotes = _costume.CostumeOtherNotes;
			txtCostumeOtherNotes.Text = (othernotes ?? "").Replace("<br>", Environment.NewLine);

		}

		/// <summary>
		/// Populates the default portrait dropdown menu
		/// </summary>
		private void PopulatePortraitDropdown()
		{
			_populatingImages = true;
			List<PoseMapping> poses = _costume.PoseLibrary.GetPortraitPoses(_fullyClothedStage);
			cboDefaultPic.DisplayMember = "DisplayName";
			cboDefaultPic.DataSource = poses;
			_populatingImages = false;
		}

		private bool PromptToSave()
		{
			if (_costume == null || !_costume.IsDirty)
				return true;
			DialogResult result = MessageBox.Show(string.Format("Do you wish to save {0} first?", _costume.Link.CostumeName), "Save changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			if (result == DialogResult.Yes)
			{
				_exportOnQuit = true;
				return true;
			}
			else if (result == DialogResult.No)
			{
				return true;
			}
			return false;
		}

		public override bool CanQuit(CloseArgs args)
		{
			return PromptToSave();
		}

		public override void Quit()
		{
			if (_exportOnQuit)
			{
				OnSaveWorkspace(false);
			}
		}

		public override void Save()
		{
			_costume.Labels = gridLabels.Values;
			int countUnskipped = _costume.Wardrobe.Where(x => x?.Type != "skip").Count();
			_costume.LayersNonSkip = countUnskipped;

			string notes = txtCostumeOtherNotes.Text;

			if (string.IsNullOrWhiteSpace(notes))
			{
				notes = null;
			}
			else
			{
				notes = notes.Replace(Environment.NewLine, "<br>");
			}

			if (notes != _costume.CostumeOtherNotes)
			{
				_costume.CostumeOtherNotes = notes;
				_costume.IsDirty = true;
			}

			if (_costume.Link != null)
			{
				string status = cboStatus.Text;
				if (string.IsNullOrEmpty(status))
				{
					status = null;
				}

				string set = cboEvent.Text;
				if (string.IsNullOrEmpty(set) || set == "none")
				{
					set = null;
				}

				string description = txtDescription.Text;
				if (string.IsNullOrEmpty(description) || description == "")
				{
					description = null;
				}

				string gender = cboGender.SelectedItem?.ToString() ?? _costume.Character.Gender;

				string selectedBreasts = cboSize.SelectedItem?.ToString() ?? "";
				string selectedFutaPenis = cboFutanariPenisSize.SelectedItem?.ToString() ?? "";

				if (gender == "male")
				{
					string selected = cboSize.SelectedItem?.ToString() ?? "";
					string basePenis = GetBaseMalePenisSize();

					_costume.Penis = (string.IsNullOrEmpty(selected) || selected == basePenis) ? null : selected;
					_costume.Breasts = null;
				}
				else
				{
					// Breasts (female/futa)
					string baseBreasts = GetBaseFemaleBreastSize();
					_costume.Breasts = (string.IsNullOrEmpty(selectedBreasts) || selectedBreasts == baseBreasts) ? null : selectedBreasts;

					// Penis (futa only)
					string baseFutaPenis = GetBaseFutaPenisSize(); // probably _costume.Character.Penis
					bool futaEffective = !string.IsNullOrEmpty(selectedFutaPenis) || !string.IsNullOrEmpty(baseFutaPenis);

					if (futaEffective)
					{
						// Only write an override if different from base
						_costume.Penis = (string.IsNullOrEmpty(selectedFutaPenis) || selectedFutaPenis == baseFutaPenis) ? null : selectedFutaPenis;
					}
					else
					{
						// Not futa: ensure no penis override is written
						_costume.Penis = null;
					}
				}

				string label = _costume.Labels.Count > 0 ? _costume.Labels[0].Value : null;
				int layers = (int)valLayers.Value;

				if (txtName.Text != _costume.Link.CostumeName || status != _costume.Link.Status || set != _costume.Link.Set || _costume.Link.IsDirty
					|| gender != _costume.Link.Gender || label != _costume.Link.Label 
					|| _costume.Link.LayersNonSkip == 0 && layers != _costume.Layers
					|| layers != _costume.Link.LayersNonSkip
					|| description != _costume.Description )
				{
					_linkDataChanged = true;
				}
				if (_linkDataChanged)
				{
					_linkDataChanged = false;
					_costume.Link.IsDirty = false;
					_costume.Link.CostumeName = txtName.Text;
					_costume.Link.Status = status;
					_costume.Link.Set = set;
					_costume.Link.Label = label;
					_costume.Link.CostumeDescription = description;
					_costume.Link.LayersNonSkip = layers == _costume.Layers ? 0 : layers;

					if (gender != _costume.Character.Gender)
					{
						_costume.Link.Gender = gender;
					}
					else
					{
						_costume.Link.Gender = null;
					}

					Serialization.ExportCharacter(_costume.Character);
				}
			}

			//Here's where any unexpected folders are thrown out
			string folder = _costume.Folder;
			_costume.Folders.Clear();
			_costume.Folders.Add(new StageSpecificValue(0, folder));
			int baseIndex = cboBaseStage.SelectedIndex - 1;
			if (baseIndex >= 0)
			{
				_costume.Folders.Add(new StageSpecificValue(baseIndex, $"opponents/{_costume.Character.FolderName}/"));
			}
		}

		private void cboDefaultPic_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_populatingImages)
				return;

			PoseMapping image = cboDefaultPic.SelectedItem as PoseMapping;
			if (image == null)
				return;
			string newKey = image.Key.Replace("#-", _fullyClothedStage + "-");
			if (_costume.Link.PreviewImage != newKey)
			{
				_costume.Link.PreviewImage = newKey;
				_costume.Link.IsDirty = true;
			}
			Workspace.SendMessage(WorkspaceMessages.UpdatePreviewImage, new UpdateImageArgs(_costume, image, _fullyClothedStage));
		}

		private void cmdExpandPortrait_Click(object sender, EventArgs e)
		{
			cmdExpandPortrait.Visible = false;
			lblLayers.Visible = true;
			valLayers.Visible = true;
		}
	}
}
