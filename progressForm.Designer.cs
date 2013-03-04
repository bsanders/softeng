namespace SoftwareEng
{
    partial class progressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(progressForm));
            this.finishButton = new System.Windows.Forms.Button();
            this.importProgressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.importNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // finishButton
            // 
            this.finishButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(177, 146);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 0;
            this.finishButton.Text = "Finish";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // importProgressBar
            // 
            this.importProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importProgressBar.Location = new System.Drawing.Point(12, 64);
            this.importProgressBar.Maximum = 1000;
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(414, 23);
            this.importProgressBar.Step = 1;
            this.importProgressBar.TabIndex = 1;
            // 
            // progressLabel
            // 
            this.progressLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.progressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressLabel.Location = new System.Drawing.Point(9, 20);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(417, 20);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.Text = "Pictures Importing...";
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // importNotifyIcon
            // 
            this.importNotifyIcon.BalloonTipText = "Click Here to See";
            this.importNotifyIcon.BalloonTipTitle = "Pictures successfully imported!";
            this.importNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("importNotifyIcon.Icon")));
            this.importNotifyIcon.Text = "test";
            this.importNotifyIcon.Visible = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(177, 146);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressForm
            // 
            this.AcceptButton = this.finishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(432, 181);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.importProgressBar);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "progressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Photo Import";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.ProgressBar importProgressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.NotifyIcon importNotifyIcon;
        private System.Windows.Forms.Button cancelButton;
    }
}