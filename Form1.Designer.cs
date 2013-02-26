namespace SoftwareEng
{
    partial class Form1
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
            this.loadXML = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.output = new System.Windows.Forms.RichTextBox();
            this.loadAlbums = new System.Windows.Forms.Button();
            this.loadAlbumPictures = new System.Windows.Forms.Button();
            this.uidTE = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.getPictureByUIDButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.addAlbumButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadXML
            // 
            this.loadXML.Location = new System.Drawing.Point(12, 367);
            this.loadXML.Name = "loadXML";
            this.loadXML.Size = new System.Drawing.Size(106, 23);
            this.loadXML.TabIndex = 0;
            this.loadXML.Text = "reload Databases";
            this.loadXML.UseVisualStyleBackColor = true;
            this.loadXML.Click += new System.EventHandler(this.loadXML_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(149, 367);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "SaveXml";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(12, 12);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(547, 349);
            this.output.TabIndex = 3;
            this.output.Text = "";
            // 
            // loadAlbums
            // 
            this.loadAlbums.Location = new System.Drawing.Point(260, 367);
            this.loadAlbums.Name = "loadAlbums";
            this.loadAlbums.Size = new System.Drawing.Size(75, 23);
            this.loadAlbums.TabIndex = 4;
            this.loadAlbums.Text = "loadAlbums";
            this.loadAlbums.UseVisualStyleBackColor = true;
            this.loadAlbums.Click += new System.EventHandler(this.loadAlbums_Click);
            // 
            // loadAlbumPictures
            // 
            this.loadAlbumPictures.Location = new System.Drawing.Point(393, 367);
            this.loadAlbumPictures.Name = "loadAlbumPictures";
            this.loadAlbumPictures.Size = new System.Drawing.Size(98, 23);
            this.loadAlbumPictures.TabIndex = 6;
            this.loadAlbumPictures.Text = "loadAlbumPictures";
            this.loadAlbumPictures.UseVisualStyleBackColor = true;
            this.loadAlbumPictures.Click += new System.EventHandler(this.loadAlbumPictures_Click);
            // 
            // uidTE
            // 
            this.uidTE.Location = new System.Drawing.Point(391, 427);
            this.uidTE.Name = "uidTE";
            this.uidTE.Size = new System.Drawing.Size(100, 20);
            this.uidTE.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(388, 411);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "UID";
            // 
            // getPictureByUIDButton
            // 
            this.getPictureByUIDButton.Location = new System.Drawing.Point(260, 411);
            this.getPictureByUIDButton.Name = "getPictureByUIDButton";
            this.getPictureByUIDButton.Size = new System.Drawing.Size(97, 23);
            this.getPictureByUIDButton.TabIndex = 10;
            this.getPictureByUIDButton.Text = "getPictureByUID";
            this.getPictureByUIDButton.UseVisualStyleBackColor = true;
            this.getPictureByUIDButton.Click += new System.EventHandler(this.getPictureByUIDButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 411);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "addNewPicture";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // addAlbumButton
            // 
            this.addAlbumButton.Location = new System.Drawing.Point(133, 411);
            this.addAlbumButton.Name = "addAlbumButton";
            this.addAlbumButton.Size = new System.Drawing.Size(75, 23);
            this.addAlbumButton.TabIndex = 12;
            this.addAlbumButton.Text = "addAlbum";
            this.addAlbumButton.UseVisualStyleBackColor = true;
            this.addAlbumButton.Click += new System.EventHandler(this.addAlbumButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 459);
            this.Controls.Add(this.addAlbumButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.getPictureByUIDButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.uidTE);
            this.Controls.Add(this.loadAlbumPictures);
            this.Controls.Add(this.loadAlbums);
            this.Controls.Add(this.output);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.loadXML);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadXML;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox output;
        private System.Windows.Forms.Button loadAlbums;
        private System.Windows.Forms.Button loadAlbumPictures;
        private System.Windows.Forms.TextBox uidTE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button getPictureByUIDButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button addAlbumButton;
    }
}

