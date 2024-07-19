namespace Desktop.CommonControls
{
	public partial class PropertyTableWithCheckboxes : PropertyTable
	{
		public void LabelCheckboxes(string lbl1text, string chk1text, string chk2text)
		{
			lbl1.Text = lbl1text;
			chk1.Text = chk1text;
			chk2.Text = chk2text;
		}

		public void SetCheckboxes(bool c1, bool c2)
		{
			chk1.Checked = c1;
			chk2.Checked = c2;
		}

		public bool GetCheckbox1()
		{
			return chk1.Checked;
		}
		public bool GetCheckbox2()
		{
			return chk2.Checked;
		}

		public PropertyTableWithCheckboxes() : base()
		{
			//RemoveCaption = "Remove";
			InitializeComponent();

			//recAdd.RecordFilter = FilterControlsToData;
			//recAdd.RecordType = typeof(PropertyRecord);

			//OnUpdateSkin(SkinManager.Instance.CurrentSkin);
		}

		private void chk1_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!chk1.Checked)
			{
				chk2.Checked = true;
			}
		}

		private void chk2_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!chk2.Checked)
			{
				chk1.Checked = true;
			}
		}
	}

}
