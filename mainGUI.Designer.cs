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
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Add New Album", 0);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Unsorted Pictures", 0);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainGUI));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addPhotosToExistingAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.albumListView = new System.Windows.Forms.ListView();
            this.albumImageList = new System.Windows.Forms.ImageList(this.components);
            this.mainFormBackbutton = new System.Windows.Forms.Button();
            this.photoListView = new System.Windows.Forms.ListView();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(589, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewAlbumToolStripMenuItem,
            this.toolStripSeparator1,
            this.addPhotosToExistingAlbumToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // createNewAlbumToolStripMenuItem
            // 
            this.createNewAlbumToolStripMenuItem.Name = "createNewAlbumToolStripMenuItem";
            this.createNewAlbumToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.createNewAlbumToolStripMenuItem.Text = "Create New Album";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // addPhotosToExistingAlbumToolStripMenuItem
            // 
            this.addPhotosToExistingAlbumToolStripMenuItem.Name = "addPhotosToExistingAlbumToolStripMenuItem";
            this.addPhotosToExistingAlbumToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.addPhotosToExistingAlbumToolStripMenuItem.Text = "Add photos to existing album";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // albumListView
            // 
            this.albumListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.albumListView.Font = new System.Drawing.Font("Corbel", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.albumListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.albumListView.LargeImageList = this.albumImageList;
            this.albumListView.Location = new System.Drawing.Point(12, 27);
            this.albumListView.MultiSelect = false;
            this.albumListView.Name = "albumListView";
            this.albumListView.Size = new System.Drawing.Size(565, 371);
            this.albumListView.TabIndex = 1;
            this.albumListView.UseCompatibleStateImageBehavior = false;
            this.albumListView.ItemActivate += new System.EventHandler(this.albumListView_ItemActivate);
            // 
            // albumImageList
            // 
            this.albumImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("albumImageList.ImageStream")));
            this.albumImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.albumImageList.Images.SetKeyName(0, "Book_Green_48x48.png");
            // 
            // mainFormBackbutton
            // 
            this.mainFormBackbutton.Location = new System.Drawing.Point(502, 408);
            this.mainFormBackbutton.Name = "mainFormBackbutton";
            this.mainFormBackbutton.Size = new System.Drawing.Size(75, 23);
            this.mainFormBackbutton.TabIndex = 2;
            this.mainFormBackbutton.Text = "Back";
            this.mainFormBackbutton.UseVisualStyleBackColor = true;
            // 
            // photoListView
            // 
            this.photoListView.Location = new System.Drawing.Point(12, 27);
            this.photoListView.Name = "photoListView";
            this.photoListView.Size = new System.Drawing.Size(565, 371);
            this.photoListView.TabIndex = 3;
            this.photoListView.UseCompatibleStateImageBehavior = false;
            this.photoListView.ItemActivate += new System.EventHandler(this.photoListView_ItemActivate);
            // 
            // mainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 443);
            this.Controls.Add(this.mainFormBackbutton);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.albumListView);
            this.Controls.Add(this.photoListView);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "mainGUI";
            this.Text = "Photobombers";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ListView albumListView;
        private System.Windows.Forms.ImageList albumImageList;
        private System.Windows.Forms.ToolStripMenuItem createNewAlbumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addPhotosToExistingAlbumToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button mainFormBackbutton;
        private System.Windows.Forms.ListView photoListView;
    }
}