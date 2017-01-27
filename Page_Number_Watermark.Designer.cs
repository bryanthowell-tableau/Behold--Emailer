namespace Behold_Emailer
{
    partial class Page_Number_Watermark
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
            this.label24 = new System.Windows.Forms.Label();
            this.pageNumbersShowTotal = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.pageNumbersPrefix = new System.Windows.Forms.TextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.pageNumberFontSize = new System.Windows.Forms.Label();
            this.pageNumberFontName = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.pageNumberFontStyle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(16, 97);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(150, 13);
            this.label24.TabIndex = 84;
            this.label24.Text = "Show Total Pages (N of Total)";
            // 
            // pageNumbersShowTotal
            // 
            this.pageNumbersShowTotal.FormattingEnabled = true;
            this.pageNumbersShowTotal.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.pageNumbersShowTotal.Location = new System.Drawing.Point(19, 115);
            this.pageNumbersShowTotal.Name = "pageNumbersShowTotal";
            this.pageNumbersShowTotal.Size = new System.Drawing.Size(74, 21);
            this.pageNumbersShowTotal.TabIndex = 83;
            this.pageNumbersShowTotal.Text = "No";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(16, 149);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(146, 13);
            this.label23.TabIndex = 82;
            this.label23.Text = "Text in Front of Page Number";
            // 
            // pageNumbersPrefix
            // 
            this.pageNumbersPrefix.Location = new System.Drawing.Point(19, 165);
            this.pageNumbersPrefix.Name = "pageNumbersPrefix";
            this.pageNumbersPrefix.Size = new System.Drawing.Size(134, 20);
            this.pageNumbersPrefix.TabIndex = 81;
            // 
            // pageNumberFontSize
            // 
            this.pageNumberFontSize.AutoSize = true;
            this.pageNumberFontSize.Location = new System.Drawing.Point(166, 28);
            this.pageNumberFontSize.Name = "pageNumberFontSize";
            this.pageNumberFontSize.Size = new System.Drawing.Size(19, 13);
            this.pageNumberFontSize.TabIndex = 89;
            this.pageNumberFontSize.Text = "10";
            // 
            // pageNumberFontName
            // 
            this.pageNumberFontName.AutoSize = true;
            this.pageNumberFontName.Location = new System.Drawing.Point(24, 28);
            this.pageNumberFontName.Name = "pageNumberFontName";
            this.pageNumberFontName.Size = new System.Drawing.Size(97, 13);
            this.pageNumberFontName.TabIndex = 88;
            this.pageNumberFontName.Text = "Times New Roman";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 87;
            this.button1.Text = "Set Font";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.setFont_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(16, 9);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(59, 13);
            this.label28.TabIndex = 86;
            this.label28.Text = "Font Name";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(152, 9);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 85;
            this.label18.Text = "Font Size";
            // 
            // pageNumberFontStyle
            // 
            this.pageNumberFontStyle.AutoSize = true;
            this.pageNumberFontStyle.Location = new System.Drawing.Point(236, 28);
            this.pageNumberFontStyle.Name = "pageNumberFontStyle";
            this.pageNumberFontStyle.Size = new System.Drawing.Size(44, 13);
            this.pageNumberFontStyle.TabIndex = 91;
            this.pageNumberFontStyle.Text = "Regular";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 90;
            this.label2.Text = "Font Style";
            // 
            // Page_Number_Watermark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 255);
            this.Controls.Add(this.pageNumberFontStyle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pageNumberFontSize);
            this.Controls.Add(this.pageNumberFontName);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.pageNumbersShowTotal);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.pageNumbersPrefix);
            this.Name = "Page_Number_Watermark";
            this.Text = "Add / Edit Page Numberer";
            this.Load += new System.EventHandler(this.Page_Number_Watermark_Load);
            this.Controls.SetChildIndex(this.pageNumbersPrefix, 0);
            this.Controls.SetChildIndex(this.label23, 0);
            this.Controls.SetChildIndex(this.pageNumbersShowTotal, 0);
            this.Controls.SetChildIndex(this.label24, 0);
            this.Controls.SetChildIndex(this.label18, 0);
            this.Controls.SetChildIndex(this.label28, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.pageNumberFontName, 0);
            this.Controls.SetChildIndex(this.pageNumberFontSize, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.pageNumberFontStyle, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox pageNumbersShowTotal;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox pageNumbersPrefix;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Label pageNumberFontSize;
        private System.Windows.Forms.Label pageNumberFontName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label pageNumberFontStyle;
        private System.Windows.Forms.Label label2;
    }
}