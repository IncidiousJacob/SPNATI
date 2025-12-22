using Desktop;
using Desktop.CommonControls;
using Desktop.Skinning;
using SPNATI_Character_Editor.Controls;
using SPNATI_Character_Editor.DataStructures;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Character), 70, DelayRun = true, Caption = "Collectibles")]
	[Tutorial("https://www.youtube.com/watch?v=TQqijFL0Kko")]
	public partial class CollectibleEditor : Activity
	{
		private Character _character;
		private ListViewItem _selectedItem;

		public CollectibleEditor()
		{
			InitializeComponent();
		}

		public override string Caption
		{
			get
			{
				return "Collectibles";
			}
		}

		protected override void OnInitialize()
		{
			_character = Record as Character;
			lstCollectibles.LargeImageList = new ImageList();
			lstCollectibles.LargeImageList.ImageSize = new Size(128, 128);
			lstCollectibles.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			lstCollectibles.LargeImageList.Images.Add("???", Properties.Resources.Achievement);
			table.Context = new CollectibleContext(_character, CharacterContext.Collectible);

			txtHiddenExceptionEvent.TextChanged += HiddenException_TextChanged;
			txtHiddenExceptionCostumeSet.TextChanged += HiddenException_TextChanged;
			txtHiddenExceptionCostume.TextChanged += HiddenException_TextChanged;

			hiddenExceptionsGroupBox.Visible = false; // Hide this box by default for cleanliness sake
		}

		protected override void OnActivate()
		{
			PopulateCollectibles();
			UpdateAddButton();
		}

		private void UpdateAddButton()
		{
			CharacterHistory history = CharacterHistory.Get(_character, false);
			int limit = TestRequirements.Instance.GetAllowedCollectibles(history.Current.TotalLines);
			tsAdd.Enabled = _character.Collectibles.Count < limit;
			if (!tsAdd.Enabled)
			{
				int thresholds = _character.Collectibles.Count - 2 + 1;
				int requirement = thresholds * 600 + 1;
				tsAdd.ToolTipText = $"You need {requirement} lines to be able to add another collectible.";
			}
			else
			{
				tsAdd.ToolTipText = "Add Collectible";
			}
		}

		protected override void OnParametersUpdated(params object[] parameters)
		{
			if (parameters.Length > 0)
			{
				ValidationContext context = parameters[0] as ValidationContext;
				if (context != null)
				{
					for (int i = 0; i < lstCollectibles.Items.Count; i++)
					{
						Collectible c = lstCollectibles.Items[i].Tag as Collectible;
						if (c == context.Collectible)
						{
							lstCollectibles.Items[i].Selected = true;
							break;
						}
					}
				}
			}
		}

		private void PopulateCollectibles()
		{
			lstCollectibles.Items.Clear();
			foreach (Collectible c in _character.Collectibles.Collectibles)
			{
				AddCollectible(c, false);
			}
		}

		private void AddCollectible(Collectible c, bool select)
		{
			ListViewItem item = new ListViewItem(c.Title);
			item.Tag = c;
			item.ImageKey = ThumbnailImageKey(c);
			lstCollectibles.Items.Add(item);
			if (select)
			{
				item.Selected = true;
			}
			UpdateAddButton();
		}

		private string ThumbnailImageKey(Collectible c)
		{
			if (Config.SafeMode)
				return "";
			Bitmap thumbnail = GetImage(c.Thumbnail);
			if (thumbnail != null)
			{
				if (!lstCollectibles.LargeImageList.Images.ContainsKey(c.Thumbnail))
				{
					lstCollectibles.LargeImageList.Images.Add(c.Thumbnail, thumbnail);
				}
				return c.Thumbnail;
			}
			else
			{
				return "???";
			}
		}

		public static Bitmap GetImage(string src)
		{
			if (string.IsNullOrEmpty(src)) { return null; }
			Bitmap img = null;
			string path = Path.Combine(Config.SpnatiDirectory, src);
			if (!File.Exists(path))
			{
				return null;
			}
			try
			{
				using (Bitmap temp = new Bitmap(path))
				{
					img = new Bitmap(temp);
				}
			}
			catch { }
			return img;
		}

		public static Bitmap GetImage(CollectibleImage src)
		{
			return GetImage(src.Path);
		}


		private void tsAdd_Click(object sender, EventArgs e)
		{
			Collectible c = new Collectible()
			{
				Id = "new_collectible",
				Title = "New Collectible",
				Images = new List<CollectibleImage>(),
			};
			_character.Collectibles.Add(c);
			AddCollectible(c, true);
		}

		private void tsRemove_Click(object sender, EventArgs e)
		{
			if (lstCollectibles.SelectedItems.Count == 0) { return; }
			ListViewItem item = lstCollectibles.SelectedItems[0];
			Collectible collectible = item.Tag as Collectible;
			if (collectible != null && MessageBox.Show($"Are you sure you want to remove {collectible}? This cannot be undone.", "Remove Collectible", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				lstCollectibles.Items.Remove(item);
				_character.Collectibles.Remove(collectible);
				UpdateAddButton();
			}
		}

		private void ApplyHiddenExceptionsVisibility(Collectible collectible) // Show the GroupBox
		{
			bool show = collectible != null && collectible.Hidden;
			hiddenExceptionsGroupBox.Visible = show;
		}

		private bool _loadingHiddenExceptionsUI;

		private void HiddenException_TextChanged(object sender, EventArgs e)
		{
			if (_loadingHiddenExceptionsUI) return;

			var collectible = _selectedItem?.Tag as Collectible;
			if (collectible == null) return;

			SaveHiddenExceptionsFromUI(collectible);
		}

		private void SaveHiddenExceptionsFromUI(Collectible collectible)
		{
			string eve = txtHiddenExceptionEvent.Text?.Trim(); // Can't call it event, not calling it HEvent
			string set = txtHiddenExceptionCostumeSet.Text?.Trim(); // Trim to remove whitespace because people will mess this up. Hi Nmasp
			string costume = txtHiddenExceptionCostume.Text?.Trim();

			bool any = !string.IsNullOrEmpty(eve) || !string.IsNullOrEmpty(set) || !string.IsNullOrEmpty(costume);

			if (collectible.ExtraXml == null)
				collectible.ExtraXml = new List<XmlElement>();

			var elem = FindHiddenExceptionsElement(collectible);

			if (!any)
			{
				// No data, so remove the element entirely
				if (elem != null)
					collectible.ExtraXml.Remove(elem);
				return;
			}

			// Create the hidden exception element if it doesn't exist
			if (elem == null)
			{
				var doc = new XmlDocument();
				elem = doc.CreateElement("hidden-exceptions");
				collectible.ExtraXml.Add(elem);
			}

			// Update attributes
			SetOrClearAttribute(elem, "event", eve);
			SetOrClearAttribute(elem, "costume-set", set);
			SetOrClearAttribute(elem, "costume", costume);
		}
		private XmlElement FindHiddenExceptionsElement(Collectible collectible) // This grab the actual hidden-exceptions element
		{
			if (collectible?.ExtraXml == null) return null;
			return collectible.ExtraXml.FirstOrDefault(x => x != null && x.Name == "hidden-exceptions");
		}

		private void SetOrClearAttribute(XmlElement elem, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
				elem.RemoveAttribute(name);
			else
				elem.SetAttribute(name, value);
		}

		private void LoadHiddenExceptionsUI(Collectible collectible)
		{
			_loadingHiddenExceptionsUI = true;
			try
			{
				txtHiddenExceptionEvent.Text = "";
				txtHiddenExceptionCostumeSet.Text = "";
				txtHiddenExceptionCostume.Text = "";

				var elem = FindHiddenExceptionsElement(collectible);
				if (elem == null) return;

				txtHiddenExceptionEvent.Text = elem.GetAttribute("event");
				txtHiddenExceptionCostumeSet.Text = elem.GetAttribute("costume-set");
				txtHiddenExceptionCostume.Text = elem.GetAttribute("costume");
			}
			finally
			{
				_loadingHiddenExceptionsUI = false;
			}
		}

		private void lstCollectibles_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_selectedItem != null)
			{
				table.Save();
			}

			if (lstCollectibles.SelectedItems.Count == 0)
			{
				_selectedItem = null;
				table.Data = null;
				return;
			}

			ListViewItem item = lstCollectibles.SelectedItems[0];
			_selectedItem = item;
			Collectible collectible = item.Tag as Collectible;
			collectible.Character = _character;
			table.Data = collectible;

			ApplyHiddenExceptionsVisibility(collectible); // Open the GroupBox with the hidden exceptions text boxes
			LoadHiddenExceptionsUI(collectible); // Load the text boxes and proliferate

			lstImages.Items.Clear();

			if (collectible != null)
			{
				if (collectible.Images == null)
					collectible.Images = new List<CollectibleImage>();

				// Removing empty picture elements
				collectible.Images.RemoveAll(img => img == null || string.IsNullOrWhiteSpace(img.Path) || img.Path == "<no image set>");

				if (collectible.Images.Count > 0)
				{
					foreach (CollectibleImage img in collectible.Images)
					{
						lstImages.Items.Add(img);
					}
				}
				else
				{
					// Automatically add a picture entry if the collectible doesn't have any yet
					CollectibleImage newImg = new CollectibleImage();
					collectible.Images.Add(newImg);
					lstImages.Items.Add(newImg);
				}

				lstImages.SelectedIndex = 0;
			}
			else
			{
				lstImages.SelectedIndex = -1;
			}

			UpdatePreview();
			ToggleClothingVisibility();
			ToggleCostumeVisibility();
		}

		public override void Save()
		{
			table.Save();

            foreach (AlternateSkin alt in _character.Metadata.AlternateSkins)
            {
                foreach (SkinLink link in alt.Skins)
				{
					if (link.Collectible != null)
					{
						Collectible c = _character.Collectibles.Get(link.Collectible);
						if (c != null)
						{
							if (!string.IsNullOrEmpty(c.costumeFolder))
							{
								if (c.costumeFolder == link.Folder)
									continue;
							}
						}
						link.Collectible = null;
						link.IsDirty = true;
					}
				}
			}

			foreach (Collectible c in _character.Collectibles.Collectibles)
			{
				if (!string.IsNullOrEmpty(c.costumeFolder))
				{
					Costume skin = CharacterDatabase.GetSkin(c.costumeFolder);
					if (skin.Link.Collectible != c.Id)
					{
						skin.Link.Collectible = c.Id;
						skin.IsDirty = true;
					}
				}
			}
        }

		private void table_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_selectedItem == null)
			{
				return;
			}
			if (e.PropertyName == "Title")
			{
				Collectible collectible = _selectedItem.Tag as Collectible;
				_selectedItem.Text = collectible.Title;
			}
			else if (e.PropertyName == "Thumbnail")
			{
				Collectible collectible = _selectedItem.Tag as Collectible;
				_selectedItem.ImageKey = ThumbnailImageKey(collectible);
			}
			else if (e.PropertyName == "Image")
			{
				UpdatePreview();
			}
			else if (e.PropertyName == "Extra")
            {
				ToggleClothingVisibility();
				ToggleCostumeVisibility();
			}
			else if (e.PropertyName == "Hidden")
			{
				Collectible collectible = _selectedItem.Tag as Collectible;
				ApplyHiddenExceptionsVisibility(collectible);
			}
		}

		private void ToggleClothingVisibility()
		{
			Collectible collectible = _selectedItem.Tag as Collectible;

			Control[] rows = table.Controls.Find("PropertyTableRow", true);
			String[] names = { "ClothingName", "Classification", "Position", "Type", "Is Plural?", "ClothingImage" };

			foreach (PropertyTableRow c in rows)
			{
				if (names.Contains(c.Record.Key))
				{
					c.Visible = collectible.Wearable;
				}
			}
		}

        private void ToggleCostumeVisibility()
        {
            Collectible collectible = _selectedItem.Tag as Collectible;

            Control[] rows = table.Controls.Find("PropertyTableRow", true);
            String[] names = { "Costume" };

            foreach (PropertyTableRow c in rows)
            {
                if (names.Contains(c.Record.Key))
                {
                    c.Visible = collectible.costumeUnlock;
                }
            }
        }

 
        private void UpdatePreview()
		{
			if (!Config.SafeMode && lstImages.SelectedItems.Count > 0)
			{
				Bitmap bmp = GetImage(lstImages.SelectedItem as CollectibleImage);
				picPreview.Image = bmp;
			}
			else
			{
				picPreview.Image = null;
			}
		}

		private void tsUp_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			if (collectible == null)
			{
				return;
			}
			ObservableCollection<Collectible> collectibles = _character.Collectibles.Collectibles;

			int index = collectibles.IndexOf(collectible);
			if (index == 0)
			{
				return;
			}
			collectibles.Remove(collectible);
			collectibles.Insert(index - 1, collectible);

			lstCollectibles.ListViewItemSorter = new CollectibleSorter(collectibles);
			lstCollectibles.Sort();
		}

		private void tsDown_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			if (collectible == null)
			{
				return;
			}
			ObservableCollection<Collectible> collectibles = _character.Collectibles.Collectibles;

			int index = collectibles.IndexOf(collectible);
			if (index == collectibles.Count - 1)
			{
				return;
			}
			collectibles.Remove(collectible);
			collectibles.Insert(index + 1, collectible);

			lstCollectibles.ListViewItemSorter = new CollectibleSorter(collectibles);
			lstCollectibles.Sort();
		}

		private class CollectibleSorter : IComparer
		{
			private ObservableCollection<Collectible> _list;
			public CollectibleSorter(ObservableCollection<Collectible> list)
			{
				_list = list;
			}

			public int Compare(object x, object y)
			{
				ListViewItem i1 = x as ListViewItem;
				ListViewItem i2 = y as ListViewItem;
				Collectible c1 = i1.Tag as Collectible;
				Collectible c2 = i2.Tag as Collectible;
				return _list.IndexOf(c1).CompareTo(_list.IndexOf(c2));
			}
		}

		protected override void OnSkinChanged(Skin skin)
		{
			base.OnSkinChanged(skin);
			lstCollectibles.BackColor = skin.FieldBackColor;
			lstCollectibles.ForeColor = skin.Surface.ForeColor;
		}

		private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
		{
			// txtImagePath.Text = (lstImages.SelectedItem as CollectibleImage)?.Path;
			txtImagePath.Text = lstImages.SelectedItem.ToString();
			UpdatePreview();
		}

		private void cmdAddImage_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			if (collectible == null)
			{
				return;
			}

			CollectibleImage newImg = new CollectibleImage();
			collectible.Images.Add(newImg);
			lstImages.Items.Add(newImg);
			lstImages.SelectedIndex = lstImages.Items.Count - 1;

			// Auto-open the image selection dialog
			cmdImageBrowse_Click(cmdImageBrowse, e);
		}

		private void cmdRemoveImage_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			if (collectible == null)
			{
				return;
			}

			int idx = lstImages.SelectedIndex;
			if (idx < 0)
			{
				return;
			}

			collectible.Images.RemoveAt(idx);
			lstImages.Items.RemoveAt(idx);

			if (lstImages.Items.Count > 0)
			{
				lstImages.SelectedIndex = (idx > 0) ? (idx - 1) : 0;
			}
			else
			{
				lstImages.SelectedIndex = -1;
			}

			UpdatePreview();
		}

		private void txtImagePath_TextChanged(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			CollectibleImage selectedImage = lstImages.SelectedItem as CollectibleImage;
			if (selectedImage == null || collectible == null)
			{
				return;
			}

			string text = (txtImagePath.Text ?? "").Trim();
			if (text == "<no image set>" || text == "&lt;no image set&gt;")
			{
				text = "";
			}

			selectedImage.Path = text;
			UpdatePreview();
		}

		private void txtImagePath_Leave(object sender, EventArgs e)
		{
			CollectibleImage selectedImage = lstImages.SelectedItem as CollectibleImage;
			if (selectedImage == null || lstImages.SelectedIndex < 0)
			{
				return;
			}

			lstImages.Items[lstImages.SelectedIndex] = selectedImage;
		}

		private void cmdImageUp_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			CollectibleImage img = lstImages.SelectedItem as CollectibleImage;
			if (collectible == null || img == null)
			{
				return;
			}

			int idx = collectible.Images.IndexOf(img);
			if (idx == 0)
			{
				return;
			}

			collectible.Images.Remove(img);
			collectible.Images.Insert(idx - 1, img);

			lstImages.Items.Remove(img);
			lstImages.Items.Insert(idx - 1, img);
			lstImages.SelectedIndex = idx - 1;
		}

		private void cmdImageDown_Click(object sender, EventArgs e)
		{
			Collectible collectible = _selectedItem?.Tag as Collectible;
			CollectibleImage img = lstImages.SelectedItem as CollectibleImage;
			if (collectible == null || img == null)
			{
				return;
			}

			int idx = collectible.Images.IndexOf(img);
			if (idx == collectible.Images.Count - 1)
			{
				return;
			}

			collectible.Images.Remove(img);
			collectible.Images.Insert(idx + 1, img);

			lstImages.Items.Remove(img);
			lstImages.Items.Insert(idx + 1, img);
			lstImages.SelectedIndex = idx + 1;
		}

		private void cmdImageBrowse_Click(object sender, EventArgs e)
		{
			CollectibleImage img = lstImages.SelectedItem as CollectibleImage;
			if (img == null)
			{
				return;
			}

			string path = Path.Combine(Config.SpnatiDirectory, img.Path ?? "");
			if (path == Config.SpnatiDirectory)
			{
				openFileDialog1.FileName = "";
				path = _character.GetDirectory();
			}
			else
			{
				openFileDialog1.FileName = Path.GetFileName(path);
				path = Path.GetDirectoryName(path);
			}
			openFileDialog1.InitialDirectory = path;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string filename = openFileDialog1.FileName;
				string relPath = filename.Substring(Config.SpnatiDirectory.Length + 1).Replace('\\', '/');
				img.Path = relPath;

				lstImages.Items[lstImages.SelectedIndex] = img;
				txtImagePath.Text = relPath;
				UpdatePreview();
			}
		}
	}

	public class CollectibleContext : ICharacterContext
	{
		public ISkin Character { get; }
		public CharacterContext Context { get; }

		public CollectibleContext(ISkin character, CharacterContext context)
		{
			Character = character;
			Context = context;
		}
	}
}
