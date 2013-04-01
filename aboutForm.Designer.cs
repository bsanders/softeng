namespace SoftwareEng
{
    partial class aboutForm
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
            this.createdByLabel = new System.Windows.Forms.Label();
            this.creatorsLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // createdByLabel
            // 
            this.createdByLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.createdByLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createdByLabel.Location = new System.Drawing.Point(23, 9);
            this.createdByLabel.Name = "createdByLabel";
            this.createdByLabel.Size = new System.Drawing.Size(213, 78);
            this.createdByLabel.TabIndex = 0;
            this.createdByLabel.Text = "Created by PhotoBomber Studios, LLC";
            this.createdByLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // creatorsLabel
            // 
            this.creatorsLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.creatorsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creatorsLabel.Location = new System.Drawing.Point(67, 87);
            this.creatorsLabel.Name = "creatorsLabel";
            this.creatorsLabel.Size = new System.Drawing.Size(125, 110);
            this.creatorsLabel.TabIndex = 1;
            this.creatorsLabel.Text = "Ryan Causey Ryan Moe Julian Nguyen William Sanders Alejandro Sosa";
            this.creatorsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(93, 213);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 26);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK!";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // aboutForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 251);
            this.ControlBox = false;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.creatorsLabel);
            this.Controls.Add(this.createdByLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(271, 283);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(271, 283);
            this.Name = "aboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label createdByLabel;
        private System.Windows.Forms.Label creatorsLabel;
        private System.Windows.Forms.Button okButton;
    }
}