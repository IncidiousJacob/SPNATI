namespace Desktop.CommonControls
{
	partial class PropertyTableWithCheckboxes
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.chk1 = new Desktop.Skinning.SkinnedCheckBox();
            this.chk2 = new Desktop.Skinning.SkinnedCheckBox();
            this.lbl1 = new Desktop.Skinning.SkinnedLabel();
            this.SuspendLayout();
            // 
            // chk1
            // 
            this.chk1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chk1.AutoSize = true;
            this.chk1.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.chk1.Location = new System.Drawing.Point(622, 6);
            this.chk1.Name = "chk1";
            this.chk1.Size = new System.Drawing.Size(50, 17);
            this.chk1.TabIndex = 5;
            this.chk1.Text = "chk1";
            this.chk1.UseVisualStyleBackColor = true;
            this.chk1.CheckedChanged += new System.EventHandler(this.chk1_CheckedChanged);
            // 
            // chk2
            // 
            this.chk2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chk2.AutoSize = true;
            this.chk2.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.chk2.Location = new System.Drawing.Point(682, 6);
            this.chk2.Name = "chk2";
            this.chk2.Size = new System.Drawing.Size(50, 17);
            this.chk2.TabIndex = 6;
            this.chk2.Text = "chk2";
            this.chk2.UseVisualStyleBackColor = true;
            this.chk2.CheckedChanged += new System.EventHandler(this.chk2_CheckedChanged);
            // 
            // lbl1
            // 
            this.lbl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lbl1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl1.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
            this.lbl1.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
            this.lbl1.Location = new System.Drawing.Point(527, 6);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(23, 13);
            this.lbl1.TabIndex = 7;
            this.lbl1.Text = "lbl1";
            // 
            // PropertyTableWithCheckboxes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.chk1);
            this.Controls.Add(this.chk2);
            this.Name = "PropertyTableWithCheckboxes";
            this.Controls.SetChildIndex(this.chk2, 0);
            this.Controls.SetChildIndex(this.chk1, 0);
            this.Controls.SetChildIndex(this.lbl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private Skinning.SkinnedCheckBox chk1;
        private Skinning.SkinnedCheckBox chk2;
        private Skinning.SkinnedLabel lbl1;
    }
}
