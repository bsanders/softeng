namespace TestApp
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
            this.xmlPathTE = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // loadXML
            // 
            this.loadXML.Location = new System.Drawing.Point(24, 185);
            this.loadXML.Name = "loadXML";
            this.loadXML.Size = new System.Drawing.Size(75, 23);
            this.loadXML.TabIndex = 0;
            this.loadXML.Text = "loadXml";
            this.loadXML.UseVisualStyleBackColor = true;
            this.loadXML.Click += new System.EventHandler(this.loadXML_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(105, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "SaveXml";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(24, 32);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(237, 122);
            this.output.TabIndex = 3;
            this.output.Text = "";
            // 
            // loadAlbums
            // 
            this.loadAlbums.Location = new System.Drawing.Point(187, 184);
            this.loadAlbums.Name = "loadAlbums";
            this.loadAlbums.Size = new System.Drawing.Size(75, 23);
            this.loadAlbums.TabIndex = 4;
            this.loadAlbums.Text = "loadAlbums";
            this.loadAlbums.UseVisualStyleBackColor = true;
            this.loadAlbums.Click += new System.EventHandler(this.loadAlbums_Click);
            // 
            // xmlPathTE
            // 
            this.xmlPathTE.Location = new System.Drawing.Point(24, 230);
            this.xmlPathTE.Name = "xmlPathTE";
            this.xmlPathTE.Size = new System.Drawing.Size(100, 20);
            this.xmlPathTE.TabIndex = 5;
            this.xmlPathTE.Text = "test.xml";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.xmlPathTE);
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
        private System.Windows.Forms.TextBox xmlPathTE;
    }
}

