namespace Behold_Emailer
{
    partial class Image_Watermark
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
            this.button2 = new System.Windows.Forms.Button();
            this.pickHeaderLogoLocation = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.wmImage = new System.Windows.Forms.TextBox();
            this.openLogoLocation = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 78);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 52;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // pickHeaderLogoLocation
            // 
            this.pickHeaderLogoLocation.Location = new System.Drawing.Point(74, 78);
            this.pickHeaderLogoLocation.Name = "pickHeaderLogoLocation";
            this.pickHeaderLogoLocation.Size = new System.Drawing.Size(75, 23);
            this.pickHeaderLogoLocation.TabIndex = 51;
            this.pickHeaderLogoLocation.Text = "Browse";
            this.pickHeaderLogoLocation.UseVisualStyleBackColor = true;
            this.pickHeaderLogoLocation.Click += new System.EventHandler(this.pickHeaderLogoLocation_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 36);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(144, 13);
            this.label17.TabIndex = 50;
            this.label17.Text = "Image File (Max Height 40px)";
            // 
            // wmImage
            // 
            this.wmImage.Location = new System.Drawing.Point(15, 52);
            this.wmImage.Name = "wmImage";
            this.wmImage.Size = new System.Drawing.Size(134, 20);
            this.wmImage.TabIndex = 49;
            // 
            // Image_Watermark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 243);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pickHeaderLogoLocation);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.wmImage);
            this.Name = "Image_Watermark";
            this.Text = "Add / Edit Image";
            this.Controls.SetChildIndex(this.wmImage, 0);
            this.Controls.SetChildIndex(this.label17, 0);
            this.Controls.SetChildIndex(this.pickHeaderLogoLocation, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button pickHeaderLogoLocation;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox wmImage;
        private System.Windows.Forms.OpenFileDialog openLogoLocation;
    }
}