namespace SoftwareEng
{
    partial class PhotoViewWindow
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
            this.photoBox = new System.Windows.Forms.PictureBox();
            this.okPhotoViewButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.photoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // photoBox
            // 
            this.photoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.photoBox.Location = new System.Drawing.Point(1, 2);
            this.photoBox.MinimumSize = new System.Drawing.Size(290, 270);
            this.photoBox.Name = "photoBox";
            this.photoBox.Size = new System.Drawing.Size(290, 270);
            this.photoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.photoBox.TabIndex = 0;
            this.photoBox.TabStop = false;
            // 
            // okPhotoViewButton
            // 
            this.okPhotoViewButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okPhotoViewButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okPhotoViewButton.Location = new System.Drawing.Point(112, 278);
            this.okPhotoViewButton.MaximumSize = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.Name = "okPhotoViewButton";
            this.okPhotoViewButton.Size = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.TabIndex = 1;
            this.okPhotoViewButton.Text = "OK";
            this.okPhotoViewButton.UseVisualStyleBackColor = true;
            this.okPhotoViewButton.Click += new System.EventHandler(this.okPhotoViewButton_Click);
            // 
            // PhotoViewWindow
            // 
            this.AcceptButton = this.okPhotoViewButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(292, 313);
            this.ControlBox = false;
            this.Controls.Add(this.okPhotoViewButton);
            this.Controls.Add(this.photoBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(302, 345);
            this.Name = "PhotoViewWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PhotoView";
            ((System.ComponentModel.ISupportInitialize)(this.photoBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox photoBox;
        private System.Windows.Forms.Button okPhotoViewButton;
    }
}