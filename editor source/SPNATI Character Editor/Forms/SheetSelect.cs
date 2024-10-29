using Desktop.Skinning;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Forms
{
	public partial class SheetSelectForm : SkinnedForm
	{
		public int Sheet { get; private set; }

		public SheetSelectForm(List<string> sheetNames)
		{
			InitializeComponent();
			foreach (string sheetName in sheetNames)
			{
				lstSheets.Items.Add(sheetName);
			}
			lstSheets.SelectedIndex = -1;
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			if (lstSheets.SelectedIndex == -1)
			{
				DialogResult = DialogResult.Cancel;
			}
			else
			{
				DialogResult = DialogResult.OK;
				Sheet = lstSheets.SelectedIndex;
			}
			Close();
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
