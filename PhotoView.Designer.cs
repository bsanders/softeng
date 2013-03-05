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
            this.photoboxPanel2 = new System.Windows.Forms.Panel();
            this.photoboxPanel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.photoBox)).BeginInit();
            this.photoboxPanel2.SuspendLayout();
            this.photoboxPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // photoBox
            // 
            this.photoBox.Location = new System.Drawing.Point(0, 0);
            this.photoBox.Margin = new System.Windows.Forms.Padding(3, 3, 23, 23);
            this.photoBox.MinimumSize = new System.Drawing.Size(290, 270);
            this.photoBox.Name = "photoBox";
            this.photoBox.Size = new System.Drawing.Size(309, 282);
            this.photoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.photoBox.TabIndex = 0;
            this.photoBox.TabStop = false;
            // 
            // okPhotoViewButton
            // 
            this.okPhotoViewButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okPhotoViewButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okPhotoViewButton.Location = new System.Drawing.Point(126, 311);
            this.okPhotoViewButton.MaximumSize = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.Name = "okPhotoViewButton";
            this.okPhotoViewButton.Size = new System.Drawing.Size(75, 23);
            this.okPhotoViewButton.TabIndex = 1;
            this.okPhotoViewButton.Text = "Ok";
            this.okPhotoViewButton.UseVisualStyleBackColor = true;
            this.okPhotoViewButton.Click += new System.EventHandler(this.okPhotoViewButton_Click);
            // 
            // photoboxPanel2
            // 
            this.photoboxPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.photoboxPanel2.AutoScroll = true;
            this.photoboxPanel2.AutoSize = true;
            this.photoboxPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.photoboxPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.photoboxPanel2.Controls.Add(this.photoboxPanel1);
            this.photoboxPanel2.Location = new System.Drawing.Point(1, 2);
            this.photoboxPanel2.MinimumSize = new System.Drawing.Size(290, 270);
            this.photoboxPanel2.Name = "photoboxPanel2";
            this.photoboxPanel2.Size = new System.Drawing.Size(335, 308);
            this.photoboxPanel2.TabIndex = 4;
            // 
            // photoboxPanel1
            // 
            this.photoboxPanel1.AutoScroll = true;
            this.photoboxPanel1.AutoSize = true;
            this.photoboxPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.photoboxPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.photoboxPanel1.Controls.Add(this.photoBox);
            this.photoboxPanel1.Location = new System.Drawing.Point(0, 0);
            this.photoboxPanel1.MaximumSize = new System.Drawing.Size(1000, 700);
            this.photoboxPanel1.Name = "photoboxPanel1";
            this.photoboxPanel1.Size = new System.Drawing.Size(332, 305);
            this.photoboxPanel1.TabIndex = 5;
            // 
            // PhotoViewWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(316, 335);
            this.ControlBox = false;
            this.Controls.Add(this.okPhotoViewButton);
            this.Controls.Add(this.photoboxPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(308, 345);
            this.Name = "PhotoViewWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PhotoView";
            ((System.ComponentModel.ISupportInitialize)(this.photoBox)).EndInit();
            this.photoboxPanel2.ResumeLayout(false);
            this.photoboxPanel2.PerformLayout();
            this.photoboxPanel1.ResumeLayout(false);
            this.photoboxPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox photoBox;
        private System.Windows.Forms.Button okPhotoViewButton;
        private System.Windows.Forms.Panel photoboxPanel2;
        private System.Windows.Forms.Panel photoboxPanel1;
    }
}