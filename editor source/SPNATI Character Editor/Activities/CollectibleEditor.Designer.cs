namespace SPNATI_Character_Editor.Activities
{
	partial class CollectibleEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectibleEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstCollectibles = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsAdd = new System.Windows.Forms.ToolStripButton();
            this.tsRemove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsUp = new System.Windows.Forms.ToolStripButton();
            this.tsDown = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.table = new Desktop.CommonControls.PropertyTable();
            this.hiddenExceptionsGroupBox = new Desktop.Skinning.SkinnedGroupBox();
            this.iconAltDrag = new Desktop.Skinning.SkinnedIcon();
            this.exceptionLabel4 = new Desktop.Skinning.SkinnedLabel();
            this.txtHiddenExceptionCostume = new Desktop.Skinning.SkinnedTextBox();
            this.txtHiddenExceptionCostumeSet = new Desktop.Skinning.SkinnedTextBox();
            this.exceptionLabel3 = new Desktop.Skinning.SkinnedLabel();
            this.txtHiddenExceptionEvent = new Desktop.Skinning.SkinnedTextBox();
            this.exceptionLabel2 = new Desktop.Skinning.SkinnedLabel();
            this.cmdImageBrowse = new Desktop.Skinning.SkinnedButton();
            this.cmdImageDown = new Desktop.Skinning.SkinnedIcon();
            this.cmdImageUp = new Desktop.Skinning.SkinnedIcon();
            this.cmdRemoveImage = new Desktop.Skinning.SkinnedIcon();
            this.cmdAddImage = new Desktop.Skinning.SkinnedIcon();
            this.txtImagePath = new Desktop.Skinning.SkinnedTextBox();
            this.lstImages = new Desktop.Skinning.SkinnedListBox();
            this.label1 = new Desktop.Skinning.SkinnedLabel();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.hiddenExceptionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstCollectibles);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(782, 620);
            this.splitContainer1.SplitterDistance = 197;
            this.splitContainer1.TabIndex = 0;
            // 
            // lstCollectibles
            // 
            this.lstCollectibles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCollectibles.HideSelection = false;
            this.lstCollectibles.Location = new System.Drawing.Point(0, 25);
            this.lstCollectibles.MultiSelect = false;
            this.lstCollectibles.Name = "lstCollectibles";
            this.lstCollectibles.Size = new System.Drawing.Size(197, 595);
            this.lstCollectibles.TabIndex = 1;
            this.lstCollectibles.UseCompatibleStateImageBehavior = false;
            this.lstCollectibles.SelectedIndexChanged += new System.EventHandler(this.lstCollectibles_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsAdd,
            this.tsRemove,
            this.toolStripSeparator1,
            this.tsUp,
            this.tsDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(197, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsAdd
            // 
            this.tsAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsAdd.Image = global::SPNATI_Character_Editor.Properties.Resources.Add;
            this.tsAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsAdd.Name = "tsAdd";
            this.tsAdd.Size = new System.Drawing.Size(23, 22);
            this.tsAdd.Text = "Add Collectible";
            this.tsAdd.Click += new System.EventHandler(this.tsAdd_Click);
            // 
            // tsRemove
            // 
            this.tsRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRemove.Image = global::SPNATI_Character_Editor.Properties.Resources.Remove;
            this.tsRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRemove.Name = "tsRemove";
            this.tsRemove.Size = new System.Drawing.Size(23, 22);
            this.tsRemove.Text = "Remove Collectible";
            this.tsRemove.Click += new System.EventHandler(this.tsRemove_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsUp
            // 
            this.tsUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUp.Image = global::SPNATI_Character_Editor.Properties.Resources.UpArrow;
            this.tsUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUp.Name = "tsUp";
            this.tsUp.Size = new System.Drawing.Size(23, 22);
            this.tsUp.Text = "Move Up";
            this.tsUp.Click += new System.EventHandler(this.tsUp_Click);
            // 
            // tsDown
            // 
            this.tsDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDown.Image = global::SPNATI_Character_Editor.Properties.Resources.DownArrow;
            this.tsDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDown.Name = "tsDown";
            this.tsDown.Size = new System.Drawing.Size(23, 22);
            this.tsDown.Text = "Move Down";
            this.tsDown.Click += new System.EventHandler(this.tsDown_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.table);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.hiddenExceptionsGroupBox);
            this.splitContainer2.Panel2.Controls.Add(this.cmdImageBrowse);
            this.splitContainer2.Panel2.Controls.Add(this.cmdImageDown);
            this.splitContainer2.Panel2.Controls.Add(this.cmdImageUp);
            this.splitContainer2.Panel2.Controls.Add(this.cmdRemoveImage);
            this.splitContainer2.Panel2.Controls.Add(this.cmdAddImage);
            this.splitContainer2.Panel2.Controls.Add(this.txtImagePath);
            this.splitContainer2.Panel2.Controls.Add(this.lstImages);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.picPreview);
            this.splitContainer2.Size = new System.Drawing.Size(581, 620);
            this.splitContainer2.SplitterDistance = 284;
            this.splitContainer2.TabIndex = 0;
            // 
            // table
            // 
            this.table.AllowDelete = false;
            this.table.AllowFavorites = false;
            this.table.AllowHelp = true;
            this.table.AllowMacros = false;
            this.table.BackColor = System.Drawing.Color.White;
            this.table.Data = null;
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.HeaderType = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.table.HideAddField = true;
            this.table.HideSpeedButtons = true;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.ModifyingProperty = null;
            this.table.Name = "table";
            this.table.PanelType = Desktop.Skinning.SkinnedBackgroundType.Background;
            this.table.PlaceholderText = null;
            this.table.PreserveControls = true;
            this.table.PreviewData = null;
            this.table.RemoveCaption = "Remove";
            this.table.RowHeaderWidth = 80F;
            this.table.RunInitialAddEvents = false;
            this.table.Size = new System.Drawing.Size(284, 620);
            this.table.Sorted = true;
            this.table.TabIndex = 0;
            this.table.UndoManager = null;
            this.table.UseAutoComplete = false;
            this.table.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.table_PropertyChanged);
            // 
            // hiddenExceptionsGroupBox
            // 
            this.hiddenExceptionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hiddenExceptionsGroupBox.BackColor = System.Drawing.Color.White;
            this.hiddenExceptionsGroupBox.Controls.Add(this.iconAltDrag);
            this.hiddenExceptionsGroupBox.Controls.Add(this.exceptionLabel4);
            this.hiddenExceptionsGroupBox.Controls.Add(this.txtHiddenExceptionCostume);
            this.hiddenExceptionsGroupBox.Controls.Add(this.txtHiddenExceptionCostumeSet);
            this.hiddenExceptionsGroupBox.Controls.Add(this.exceptionLabel3);
            this.hiddenExceptionsGroupBox.Controls.Add(this.txtHiddenExceptionEvent);
            this.hiddenExceptionsGroupBox.Controls.Add(this.exceptionLabel2);
            this.hiddenExceptionsGroupBox.Highlight = Desktop.Skinning.SkinnedHighlight.Heading;
            this.hiddenExceptionsGroupBox.Image = null;
            this.hiddenExceptionsGroupBox.Location = new System.Drawing.Point(2, 374);
            this.hiddenExceptionsGroupBox.MinimumSize = new System.Drawing.Size(261, 100);
            this.hiddenExceptionsGroupBox.Name = "hiddenExceptionsGroupBox";
            this.hiddenExceptionsGroupBox.PanelType = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.hiddenExceptionsGroupBox.ShowIndicatorBar = false;
            this.hiddenExceptionsGroupBox.Size = new System.Drawing.Size(261, 102);
            this.hiddenExceptionsGroupBox.TabIndex = 15;
            this.hiddenExceptionsGroupBox.TabStop = false;
            this.hiddenExceptionsGroupBox.Text = "Hidden Exceptions";
            // 
            // iconAltDrag
            // 
            this.iconAltDrag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconAltDrag.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.iconAltDrag.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.iconAltDrag.Flat = false;
            this.iconAltDrag.Image = global::SPNATI_Character_Editor.Properties.Resources.Help;
            this.iconAltDrag.Location = new System.Drawing.Point(234, 0);
            this.iconAltDrag.Name = "iconAltDrag";
            this.iconAltDrag.Size = new System.Drawing.Size(21, 23);
            this.iconAltDrag.TabIndex = 43;
            this.toolTip1.SetToolTip(this.iconAltDrag, resources.GetString("iconAltDrag.ToolTip"));
            this.iconAltDrag.UseVisualStyleBackColor = true;
            // 
            // exceptionLabel4
            // 
            this.exceptionLabel4.AutoSize = true;
            this.exceptionLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.exceptionLabel4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exceptionLabel4.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
            this.exceptionLabel4.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
            this.exceptionLabel4.Location = new System.Drawing.Point(5, 79);
            this.exceptionLabel4.Name = "exceptionLabel4";
            this.exceptionLabel4.Size = new System.Drawing.Size(94, 13);
            this.exceptionLabel4.TabIndex = 18;
            this.exceptionLabel4.Text = "Costume Available";
            // 
            // txtHiddenExceptionCostume
            // 
            this.txtHiddenExceptionCostume.BackColor = System.Drawing.Color.White;
            this.txtHiddenExceptionCostume.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtHiddenExceptionCostume.ForeColor = System.Drawing.Color.Black;
            this.txtHiddenExceptionCostume.Location = new System.Drawing.Point(111, 76);
            this.txtHiddenExceptionCostume.Name = "txtHiddenExceptionCostume";
            this.txtHiddenExceptionCostume.Size = new System.Drawing.Size(144, 20);
            this.txtHiddenExceptionCostume.TabIndex = 17;
            this.toolTip1.SetToolTip(this.txtHiddenExceptionCostume, "The xml/folder name of a costume, e.g. aqua_konosuba_summer");
            // 
            // txtHiddenExceptionCostumeSet
            // 
            this.txtHiddenExceptionCostumeSet.BackColor = System.Drawing.Color.White;
            this.txtHiddenExceptionCostumeSet.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtHiddenExceptionCostumeSet.ForeColor = System.Drawing.Color.Black;
            this.txtHiddenExceptionCostumeSet.Location = new System.Drawing.Point(111, 50);
            this.txtHiddenExceptionCostumeSet.Name = "txtHiddenExceptionCostumeSet";
            this.txtHiddenExceptionCostumeSet.Size = new System.Drawing.Size(144, 20);
            this.txtHiddenExceptionCostumeSet.TabIndex = 16;
            this.toolTip1.SetToolTip(this.txtHiddenExceptionCostumeSet, "The xml name of a costume set that is available; e.g. april_fools");
            // 
            // exceptionLabel3
            // 
            this.exceptionLabel3.AutoSize = true;
            this.exceptionLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.exceptionLabel3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exceptionLabel3.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
            this.exceptionLabel3.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
            this.exceptionLabel3.Location = new System.Drawing.Point(5, 53);
            this.exceptionLabel3.Name = "exceptionLabel3";
            this.exceptionLabel3.Size = new System.Drawing.Size(100, 13);
            this.exceptionLabel3.TabIndex = 15;
            this.exceptionLabel3.Text = "Costume Set Active";
            // 
            // txtHiddenExceptionEvent
            // 
            this.txtHiddenExceptionEvent.BackColor = System.Drawing.Color.White;
            this.txtHiddenExceptionEvent.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.txtHiddenExceptionEvent.ForeColor = System.Drawing.Color.Black;
            this.txtHiddenExceptionEvent.Location = new System.Drawing.Point(111, 24);
            this.txtHiddenExceptionEvent.Name = "txtHiddenExceptionEvent";
            this.txtHiddenExceptionEvent.Size = new System.Drawing.Size(144, 20);
            this.txtHiddenExceptionEvent.TabIndex = 14;
            this.toolTip1.SetToolTip(this.txtHiddenExceptionEvent, "The xml name of an event; valentines, april_fools, easter, summer, halloween, xma" +
        "s, sleepover");
            // 
            // exceptionLabel2
            // 
            this.exceptionLabel2.AutoSize = true;
            this.exceptionLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.exceptionLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exceptionLabel2.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
            this.exceptionLabel2.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
            this.exceptionLabel2.Location = new System.Drawing.Point(5, 27);
            this.exceptionLabel2.Name = "exceptionLabel2";
            this.exceptionLabel2.Size = new System.Drawing.Size(68, 13);
            this.exceptionLabel2.TabIndex = 13;
            this.exceptionLabel2.Text = "Event Active";
            // 
            // cmdImageBrowse
            // 
            this.cmdImageBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdImageBrowse.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.cmdImageBrowse.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.cmdImageBrowse.Flat = false;
            this.cmdImageBrowse.Location = new System.Drawing.Point(2, 594);
            this.cmdImageBrowse.Name = "cmdImageBrowse";
            this.cmdImageBrowse.Size = new System.Drawing.Size(287, 23);
            this.cmdImageBrowse.TabIndex = 8;
            this.cmdImageBrowse.Text = "Browse for Image ...";
            this.cmdImageBrowse.UseVisualStyleBackColor = true;
            this.cmdImageBrowse.Click += new System.EventHandler(this.cmdImageBrowse_Click);
            // 
            // cmdImageDown
            // 
            this.cmdImageDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdImageDown.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.cmdImageDown.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.cmdImageDown.Flat = false;
            this.cmdImageDown.Image = global::SPNATI_Character_Editor.Properties.Resources.DownArrow;
            this.cmdImageDown.Location = new System.Drawing.Point(269, 546);
            this.cmdImageDown.Name = "cmdImageDown";
            this.cmdImageDown.Size = new System.Drawing.Size(16, 16);
            this.cmdImageDown.TabIndex = 7;
            this.cmdImageDown.Text = "Move Image Down";
            this.cmdImageDown.UseVisualStyleBackColor = true;
            this.cmdImageDown.Click += new System.EventHandler(this.cmdImageDown_Click);
            // 
            // cmdImageUp
            // 
            this.cmdImageUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdImageUp.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.cmdImageUp.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.cmdImageUp.Flat = false;
            this.cmdImageUp.Image = global::SPNATI_Character_Editor.Properties.Resources.UpArrow;
            this.cmdImageUp.Location = new System.Drawing.Point(269, 524);
            this.cmdImageUp.Name = "cmdImageUp";
            this.cmdImageUp.Size = new System.Drawing.Size(16, 16);
            this.cmdImageUp.TabIndex = 6;
            this.cmdImageUp.Text = "Move Image Up";
            this.cmdImageUp.UseVisualStyleBackColor = true;
            this.cmdImageUp.Click += new System.EventHandler(this.cmdImageUp_Click);
            // 
            // cmdRemoveImage
            // 
            this.cmdRemoveImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveImage.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.cmdRemoveImage.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.cmdRemoveImage.Flat = false;
            this.cmdRemoveImage.Image = global::SPNATI_Character_Editor.Properties.Resources.Remove;
            this.cmdRemoveImage.Location = new System.Drawing.Point(269, 502);
            this.cmdRemoveImage.Name = "cmdRemoveImage";
            this.cmdRemoveImage.Size = new System.Drawing.Size(16, 16);
            this.cmdRemoveImage.TabIndex = 5;
            this.cmdRemoveImage.Text = "Remove Image";
            this.cmdRemoveImage.UseVisualStyleBackColor = true;
            this.cmdRemoveImage.Click += new System.EventHandler(this.cmdRemoveImage_Click);
            // 
            // cmdAddImage
            // 
            this.cmdAddImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddImage.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
            this.cmdAddImage.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
            this.cmdAddImage.Flat = false;
            this.cmdAddImage.Image = global::SPNATI_Character_Editor.Properties.Resources.Add;
            this.cmdAddImage.Location = new System.Drawing.Point(269, 480);
            this.cmdAddImage.Name = "cmdAddImage";
            this.cmdAddImage.Size = new System.Drawing.Size(16, 16);
            this.cmdAddImage.TabIndex = 4;
            this.cmdAddImage.Text = "Add Image";
            this.cmdAddImage.UseVisualStyleBackColor = true;
            this.cmdAddImage.Click += new System.EventHandler(this.cmdAddImage_Click);
            // 
            // txtImagePath
            // 
            this.txtImagePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImagePath.BackColor = System.Drawing.Color.White;
            this.txtImagePath.ForeColor = System.Drawing.Color.Black;
            this.txtImagePath.Location = new System.Drawing.Point(2, 568);
            this.txtImagePath.Name = "txtImagePath";
            this.txtImagePath.Size = new System.Drawing.Size(287, 20);
            this.txtImagePath.TabIndex = 3;
            this.txtImagePath.TextChanged += new System.EventHandler(this.txtImagePath_TextChanged);
            this.txtImagePath.Leave += new System.EventHandler(this.txtImagePath_Leave);
            // 
            // lstImages
            // 
            this.lstImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstImages.BackColor = System.Drawing.Color.White;
            this.lstImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lstImages.ForeColor = System.Drawing.Color.Black;
            this.lstImages.FormattingEnabled = true;
            this.lstImages.Location = new System.Drawing.Point(2, 480);
            this.lstImages.Name = "lstImages";
            this.lstImages.Size = new System.Drawing.Size(261, 82);
            this.lstImages.TabIndex = 2;
            this.lstImages.SelectedIndexChanged += new System.EventHandler(this.lstImages_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
            this.label1.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 38);
            this.label1.TabIndex = 1;
            this.label1.Text = "Note: To unlock a collectible, associate it with dialogue in the Dialogue tab usi" +
    "ng the trophy button.";
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.Location = new System.Drawing.Point(0, 42);
            this.picPreview.Margin = new System.Windows.Forms.Padding(0);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(293, 329);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 0;
            this.picPreview.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // CollectibleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CollectibleEditor";
            this.Size = new System.Drawing.Size(782, 620);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.hiddenExceptionsGroupBox.ResumeLayout(false);
            this.hiddenExceptionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView lstCollectibles;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton tsAdd;
		private System.Windows.Forms.ToolStripButton tsRemove;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private Desktop.CommonControls.PropertyTable table;
		private System.Windows.Forms.PictureBox picPreview;
		private Desktop.Skinning.SkinnedLabel label1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsUp;
		private System.Windows.Forms.ToolStripButton tsDown;
        private Desktop.Skinning.SkinnedButton cmdImageBrowse;
        private Desktop.Skinning.SkinnedIcon cmdImageDown;
        private Desktop.Skinning.SkinnedIcon cmdImageUp;
        private Desktop.Skinning.SkinnedIcon cmdRemoveImage;
        private Desktop.Skinning.SkinnedIcon cmdAddImage;
        private Desktop.Skinning.SkinnedTextBox txtImagePath;
        private Desktop.Skinning.SkinnedListBox lstImages;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private Desktop.Skinning.SkinnedGroupBox hiddenExceptionsGroupBox;
        private Desktop.Skinning.SkinnedLabel exceptionLabel4;
        private Desktop.Skinning.SkinnedTextBox txtHiddenExceptionCostume;
        private Desktop.Skinning.SkinnedTextBox txtHiddenExceptionCostumeSet;
        private Desktop.Skinning.SkinnedLabel exceptionLabel3;
        private Desktop.Skinning.SkinnedTextBox txtHiddenExceptionEvent;
        private Desktop.Skinning.SkinnedLabel exceptionLabel2;
        private Desktop.Skinning.SkinnedIcon iconAltDrag;
    }
}
