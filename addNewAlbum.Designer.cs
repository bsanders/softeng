namespace SoftwareEng
{
    partial class addNewAlbum
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(addNewAlbum));
            this.promptLabel = new System.Windows.Forms.Label();
            this.albumNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.selectFiles = new System.Windows.Forms.OpenFileDialog();
            this.newAlbumMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // promptLabel
            // 
            this.promptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.promptLabel.AutoSize = true;
            this.promptLabel.Location = new System.Drawing.Point(110, 22);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(199, 13);
            this.promptLabel.TabIndex = 0;
            this.promptLabel.Text = "Please enter a name for your new album.";
            // 
            // albumNameTextBox
            // 
            this.albumNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.albumNameTextBox.Location = new System.Drawing.Point(12, 57);
            this.albumNameTextBox.Name = "albumNameTextBox";
            this.albumNameTextBox.Size = new System.Drawing.Size(435, 20);
            this.albumNameTextBox.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.Location = new System.Drawing.Point(13, 114);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.Location = new System.Drawing.Point(372, 114);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 6;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // selectFiles
            // 
            this.selectFiles.FileName = "openFileDialog1";
            this.selectFiles.Multiselect = true;
            // 
            // newAlbumMaskedTextBox
            // 
            this.newAlbumMaskedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newAlbumMaskedTextBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Insert;
            this.newAlbumMaskedTextBox.Location = new System.Drawing.Point(77, 84);
            this.newAlbumMaskedTextBox.Mask = "aaaaaaaaaaaaaaaa";
            this.newAlbumMaskedTextBox.Name = "newAlbumMaskedTextBox";
            this.newAlbumMaskedTextBox.PromptChar = ' ';
            this.newAlbumMaskedTextBox.Size = new System.Drawing.Size(302, 20);
            this.newAlbumMaskedTextBox.TabIndex = 7;
            // 
            // addNewAlbum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 149);
            this.ControlBox = false;
            this.Controls.Add(this.newAlbumMaskedTextBox);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.albumNameTextBox);
            this.Controls.Add(this.promptLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(475, 187);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(475, 187);
            this.Name = "addNewAlbum";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New Album";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label promptLabel;
        private System.Windows.Forms.TextBox albumNameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.OpenFileDialog selectFiles;
        private System.Windows.Forms.MaskedTextBox newAlbumMaskedTextBox;
    }
}