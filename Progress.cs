using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Behold_Emailer
{
    public partial class Progress : Form
    {
        public Progress(int default_progress, string description_text)
        {
            InitializeComponent();
            sendProgress.Value = default_progress;
            descriptionText.Text = description_text;
            statusLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void finish_progress_bar(int progress_length)
        {
            sendProgress.Value = progress_length;
            okButton.Visible = true;
            
        }
        public void update_status(string new_status)
        {
            statusLabel.Text = new_status;
        }

        private void statusLabel_Click(object sender, EventArgs e)
        {

        }

        private void sendEmail_FinishedLoading(object sender, EventArgs e)
        {

        }
    }
}
