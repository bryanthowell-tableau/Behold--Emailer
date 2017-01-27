namespace Behold_Emailer
{
    partial class Text_Watermark
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
            this.label32 = new System.Windows.Forms.Label();
            this.wmTimestamp = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.wmText = new System.Windows.Forms.TextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.wmFontName = new System.Windows.Forms.Label();
            this.wmFontSize = new System.Windows.Forms.Label();
            this.wmFontStyle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(12, 140);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(86, 13);
            this.label32.TabIndex = 72;
            this.label32.Text = "Add Timestamp?";
            // 
            // wmTimestamp
            // 
            this.wmTimestamp.FormattingEnabled = true;
            this.wmTimestamp.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.wmTimestamp.Location = new System.Drawing.Point(15, 158);
            this.wmTimestamp.Name = "wmTimestamp";
            this.wmTimestamp.Size = new System.Drawing.Size(74, 21);
            this.wmTimestamp.TabIndex = 71;
            this.wmTimestamp.Text = "No";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(12, 9);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(59, 13);
            this.label28.TabIndex = 69;
            this.label28.Text = "Font Name";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(148, 9);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 68;
            this.label18.Text = "Font Size";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 90);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 64;
            this.label13.Text = "Text";
            // 
            // wmText
            // 
            this.wmText.Location = new System.Drawing.Point(15, 106);
            this.wmText.Name = "wmText";
            this.wmText.Size = new System.Drawing.Size(254, 20);
            this.wmText.TabIndex = 63;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 79;
            this.button1.Text = "Set Font";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.setFont_Click);
            // 
            // wmFontName
            // 
            this.wmFontName.AutoSize = true;
            this.wmFontName.Location = new System.Drawing.Point(20, 28);
            this.wmFontName.Name = "wmFontName";
            this.wmFontName.Size = new System.Drawing.Size(97, 13);
            this.wmFontName.TabIndex = 80;
            this.wmFontName.Text = "Times New Roman";
            // 
            // wmFontSize
            // 
            this.wmFontSize.AutoSize = true;
            this.wmFontSize.Location = new System.Drawing.Point(162, 28);
            this.wmFontSize.Name = "wmFontSize";
            this.wmFontSize.Size = new System.Drawing.Size(19, 13);
            this.wmFontSize.TabIndex = 81;
            this.wmFontSize.Text = "10";
            // 
            // wmFontStyle
            // 
            this.wmFontStyle.AutoSize = true;
            this.wmFontStyle.Location = new System.Drawing.Point(225, 28);
            this.wmFontStyle.Name = "wmFontStyle";
            this.wmFontStyle.Size = new System.Drawing.Size(44, 13);
            this.wmFontStyle.TabIndex = 83;
            this.wmFontStyle.Text = "Regular";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 82;
            this.label2.Text = "Font Style";
            // 
            // Text_Watermark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 244);
            this.Controls.Add(this.wmFontStyle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.wmFontSize);
            this.Controls.Add(this.wmFontName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.wmTimestamp);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.wmText);
            this.Name = "Text_Watermark";
            this.Text = "Add / Modify Text Element";
            this.Controls.SetChildIndex(this.wmText, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.label18, 0);
            this.Controls.SetChildIndex(this.label28, 0);
            this.Controls.SetChildIndex(this.wmTimestamp, 0);
            this.Controls.SetChildIndex(this.label32, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.wmFontName, 0);
            this.Controls.SetChildIndex(this.wmFontSize, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.wmFontStyle, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.ComboBox wmTimestamp;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox wmText;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label wmFontName;
        private System.Windows.Forms.Label wmFontSize;
        private System.Windows.Forms.Label wmFontStyle;
        private System.Windows.Forms.Label label2;
    }
}