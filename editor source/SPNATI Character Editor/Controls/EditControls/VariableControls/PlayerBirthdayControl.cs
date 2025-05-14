
namespace SPNATI_Character_Editor.Controls.EditControls.VariableControls
{
	[SubVariable("hasBirthdayToday")]
	public partial class PlayerBirthdayControl : SPNATI_Character_Editor.PlayerControlBase
	{
		public PlayerBirthdayControl()
		{
			InitializeComponent();
		}

		protected override void OnBoundData()
		{
			base.OnBoundData();
			radNotToday.Checked = Expression.Value == "false";
			radToday.Checked = Expression.Value != "false";
			OnAddedToRow();
		}

		public override void OnAddedToRow()
		{
			OnChangeLabel("Birthday");
		}

		protected override void AddHandlers()
		{
			base.AddHandlers();
			radToday.CheckedChanged += RadToday_CheckedChanged;
			radNotToday.CheckedChanged += RadToday_CheckedChanged;
		}

		protected override void RemoveHandlers()
		{
			base.RemoveHandlers();
			radToday.CheckedChanged -= RadToday_CheckedChanged;
			radNotToday.CheckedChanged -= RadToday_CheckedChanged;
		}

		private void RadToday_CheckedChanged(object sender, System.EventArgs e)
		{
			Save();
		}

		protected override string GetVariable()
		{
			return "hasBirthdayToday";
		}

		protected override void OnSave()
		{
			base.OnSave();
			Expression.Operator = "==";
			if (radToday.Checked)
			{
				Expression.Value = "true";
			}
			else
			{
				Expression.Value = "false";
			}
		}
	}
}
