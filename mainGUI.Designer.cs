namespace SoftwareEng
{
    partial class mainGUI
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Add New Album", 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainGUI));
            this.programMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addPhotosToExistingAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.albumListView = new System.Windows.Forms.ListView();
            this.defaultImageList = new System.Windows.Forms.ImageList(this.components);
            this.openAlbumContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainFormBackbutton = new System.Windows.Forms.Button();
            this.photoListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.photoImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusLabel = new System.Windows.Forms.Label();
            this.photoOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.photoContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAlbumContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programMenuStrip.SuspendLayout();
            this.openAlbumContextMenuStrip.SuspendLayout();
            this.photoContextMenuStrip.SuspendLayout();
            this.addAlbumContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // programMenuStrip
            // 
            this.programMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.programMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.programMenuStrip.Name = "programMenuStrip";
            this.programMenuStrip.Size = new System.Drawing.Size(589, 29);
            this.programMenuStrip.TabIndex = 0;
            this.programMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewAlbumToolStripMenuItem,
            this.toolStripSeparator1,
            this.addPhotosToExistingAlbumToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(47, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createNewAlbumToolStripMenuItem
            // 
            this.createNewAlbumToolStripMenuItem.Enabled = false;
            this.createNewAlbumToolStripMenuItem.Name = "createNewAlbumToolStripMenuItem";
            this.createNewAlbumToolStripMenuItem.Size = new System.Drawing.Size(266, 26);
            this.createNewAlbumToolStripMenuItem.Text = "Create New Album";
            this.createNewAlbumToolStripMenuItem.Click += new System.EventHandler(this.createNewAlbumToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(263, 6);
            // 
            // addPhotosToExistingAlbumToolStripMenuItem
            // 
            this.addPhotosToExistingAlbumToolStripMenuItem.Enabled = false;
            this.addPhotosToExistingAlbumToolStripMenuItem.Name = "addPhotosToExistingAlbumToolStripMenuItem";
            this.addPhotosToExistingAlbumToolStripMenuItem.Size = new System.Drawing.Size(266, 26);
            this.addPhotosToExistingAlbumToolStripMenuItem.Text = "Add photos to this album";
            this.addPhotosToExistingAlbumToolStripMenuItem.Click += new System.EventHandler(this.addPhotosToExistingAlbumToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(263, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(266, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(57, 25);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Enabled = false;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // albumListView
            // 
            this.albumListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.albumListView.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.albumListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.albumListView.LargeImageList = this.defaultImageList;
            this.albumListView.Location = new System.Drawing.Point(12, 27);
            this.albumListView.MultiSelect = false;
            this.albumListView.Name = "albumListView";
            this.albumListView.Size = new System.Drawing.Size(565, 371);
            this.albumListView.TabIndex = 1;
            this.albumListView.TileSize = new System.Drawing.Size(255, 52);
            this.albumListView.UseCompatibleStateImageBehavior = false;
            this.albumListView.ItemActivate += new System.EventHandler(this.albumListView_ItemActivate);
            this.albumListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.albumListView_ItemSelectionChanged);
            // 
            // defaultImageList
            // 
            this.defaultImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("defaultImageList.ImageStream")));
            this.defaultImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.defaultImageList.Images.SetKeyName(0, "Book_Green_48x48.png");
            // 
            // openAlbumContextMenuStrip
            // 
            this.openAlbumContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.openAlbumContextMenuStrip.Name = "albumContextMenuStrip";
            this.openAlbumContextMenuStrip.Size = new System.Drawing.Size(153, 48);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.testToolStripMenuItem.Text = "Open";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // mainFormBackbutton
            // 
            this.mainFormBackbutton.Enabled = false;
            this.mainFormBackbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainFormBackbutton.Location = new System.Drawing.Point(502, 404);
            this.mainFormBackbutton.Name = "mainFormBackbutton";
            this.mainFormBackbutton.Size = new System.Drawing.Size(75, 27);
            this.mainFormBackbutton.TabIndex = 2;
            this.mainFormBackbutton.Text = "Back";
            this.mainFormBackbutton.UseVisualStyleBackColor = true;
            this.mainFormBackbutton.Click += new System.EventHandler(this.mainFormBackbutton_Click);
            // 
            // photoListView
            // 
            this.photoListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.photoListView.BackColor = System.Drawing.SystemColors.Window;
            this.photoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.photoListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.photoListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.photoListView.LabelEdit = true;
            this.photoListView.LabelWrap = false;
            this.photoListView.Location = new System.Drawing.Point(12, 27);
            this.photoListView.MultiSelect = false;
            this.photoListView.Name = "photoListView";
            this.photoListView.Size = new System.Drawing.Size(565, 371);
            this.photoListView.SmallImageList = this.photoImageList;
            this.photoListView.TabIndex = 3;
            this.photoListView.UseCompatibleStateImageBehavior = false;
            this.photoListView.View = System.Windows.Forms.View.SmallIcon;
            this.photoListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.photoListView_AfterLabelEdit);
            this.photoListView.ItemActivate += new System.EventHandler(this.photoListView_ItemActivate);
            this.photoListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.photoListView_ItemSelectionChanged);
            // 
            // photoImageList
            // 
            this.photoImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("photoImageList.ImageStream")));
            this.photoImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.photoImageList.Images.SetKeyName(0, "generic_picture.ico");
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 418);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 4;
            // 
            // photoOpenFileDialog
            // 
            this.photoOpenFileDialog.Filter = "Jpeg(*.jpg)|*.jpg|Jpeg(*.jpeg)|*.jpeg";
            this.photoOpenFileDialog.Multiselect = true;
            // 
            // photoContextMenuStrip
            // 
            this.photoContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.photoContextMenuStrip.Name = "photoContextMenuStrip";
            this.photoContextMenuStrip.Size = new System.Drawing.Size(118, 48);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // addAlbumContextMenuStrip
            // 
            this.addAlbumContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewAlbumToolStripMenuItem});
            this.addAlbumContextMenuStrip.Name = "addAlbumContextMenuStrip";
            this.addAlbumContextMenuStrip.Size = new System.Drawing.Size(163, 26);
            // 
            // addNewAlbumToolStripMenuItem
            // 
            this.addNewAlbumToolStripMenuItem.Name = "addNewAlbumToolStripMenuItem";
            this.addNewAlbumToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.addNewAlbumToolStripMenuItem.Text = "Add New Album";
            this.addNewAlbumToolStripMenuItem.Click += new System.EventHandler(this.addNewAlbumToolStripMenuItem_Click);
            // 
            // mainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 443);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.mainFormBackbutton);
            this.Controls.Add(this.programMenuStrip);
            this.Controls.Add(this.albumListView);
            this.Controls.Add(this.photoListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.programMenuStrip;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(599, 475);
            this.MinimumSize = new System.Drawing.Size(599, 475);
            this.Name = "mainGUI";
            this.Text = "Photobombers";
            this.programMenuStrip.ResumeLayout(false);
            this.programMenuStrip.PerformLayout();
            this.openAlbumContextMenuStrip.ResumeLayout(false);
            this.photoContextMenuStrip.ResumeLayout(false);
            this.addAlbumContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip programMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ListView albumListView;
        private System.Windows.Forms.ImageList defaultImageList;
        private System.Windows.Forms.ToolStripMenuItem createNewAlbumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addPhotosToExistingAlbumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button mainFormBackbutton;
        private System.Windows.Forms.ListView photoListView;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.OpenFileDialog photoOpenFileDialog;
        private System.Windows.Forms.ImageList photoImageList;
        private System.Windows.Forms.ContextMenuStrip openAlbumContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip photoContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip addAlbumContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addNewAlbumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}