namespace SPNATI_Character_Editor.Controls.EditControls.VariableControls
{
    partial class PlayerBirthdayControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radNotToday = new Desktop.Skinning.SkinnedRadioButton();
            this.radToday = new Desktop.Skinning.SkinnedRadioButton();
            this.SuspendLayout();
            // 
            // radNotToday
            // 
            this.radNotToday.AutoSize = true;
            this.radNotToday.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.radNotToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radNotToday.Location = new System.Drawing.Point(225, 1);
            this.radNotToday.Name = "radNotToday";
            this.radNotToday.Size = new System.Drawing.Size(75, 17);
            this.radNotToday.TabIndex = 14;
            this.radNotToday.TabStop = true;
            this.radNotToday.Text = "Not Today";
            this.radNotToday.UseVisualStyleBackColor = true;
            // 
            // radToday
            // 
            this.radToday.AutoSize = true;
            this.radToday.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.radToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.radToday.Location = new System.Drawing.Point(154, 1);
            this.radToday.Name = "radToday";
            this.radToday.Size = new System.Drawing.Size(55, 17);
            this.radToday.TabIndex = 13;
            this.radToday.TabStop = true;
            this.radToday.Text = "Today";
            this.radToday.UseVisualStyleBackColor = true;
            // 
            // PlayerBirthdayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.radNotToday);
            this.Controls.Add(this.radToday);
            this.Name = "PlayerBirthdayControl";
            this.Controls.SetChildIndex(this.radToday, 0);
            this.Controls.SetChildIndex(this.radNotToday, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Desktop.Skinning.SkinnedRadioButton radNotToday;
        private Desktop.Skinning.SkinnedRadioButton radToday;
    }
}
