namespace Behold_Emailer
{
    partial class Progress
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
            this.sendProgress = new System.Windows.Forms.ProgressBar();
            this.descriptionText = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // sendProgress
            // 
            this.sendProgress.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.sendProgress.ForeColor = System.Drawing.SystemColors.Window;
            this.sendProgress.Location = new System.Drawing.Point(86, 49);
            this.sendProgress.Name = "sendProgress";
            this.sendProgress.Size = new System.Drawing.Size(100, 13);
            this.sendProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.sendProgress.TabIndex = 33;
            this.sendProgress.Value = 30;
            // 
            // descriptionText
            // 
            this.descriptionText.AutoSize = true;
            this.descriptionText.Location = new System.Drawing.Point(63, 23);
            this.descriptionText.Name = "descriptionText";
            this.descriptionText.Size = new System.Drawing.Size(161, 13);
            this.descriptionText.TabIndex = 34;
            this.descriptionText.Text = "Creating and Sending Test Email";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(104, 117);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(66, 23);
            this.okButton.TabIndex = 35;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Visible = false;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(36, 74);
            this.statusLabel.MinimumSize = new System.Drawing.Size(200, 40);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(200, 40);
            this.statusLabel.TabIndex = 36;
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statusLabel.Click += new System.EventHandler(this.statusLabel_Click);
            // 
            // Progress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 152);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.descriptionText);
            this.Controls.Add(this.sendProgress);
            this.Name = "Progress";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar sendProgress;
        private System.Windows.Forms.Label descriptionText;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label statusLabel;
    }
}