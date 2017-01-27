using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace Behold_Emailer
{
    public partial class Watermark_Edit : Form
    {
        public bool existing_config;
        public string page_location;
        public Watermark_Edit(string page_location)
        {
            InitializeComponent();
            this.page_location = page_location;
            // Check if page location has an existing setting for this type

            // If so, load the info

        }

        public Watermark_Edit()
        {
            InitializeComponent();
        }

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            
            SerializableStringDictionary watermark_settings = new SerializableStringDictionary();
            watermark_settings =  save_settings(watermark_settings);
            Configurator.SetConfig(this.page_location, watermark_settings);
            Configurator.SaveConfig();
            Label label = this.Owner.Controls.Find("label_" + page_location, true).FirstOrDefault() as Label;
            string new_label_text = "";
            if (watermark_settings["watermark_type"] == "text"){
                new_label_text = "Text";
            }
            else if (watermark_settings["watermark_type"] == "image")
            {
                new_label_text = "Image";
            }
            else if (watermark_settings["watermark_type"] == "page_number")
            {
                new_label_text = "Page Number";
            }
            label.Text = new_label_text;
            Button button = this.Owner.Controls.Find("edit_" + page_location, true).FirstOrDefault() as Button;
            button.Text = "Edit";

            this.Close();
        }

        protected virtual SerializableStringDictionary save_settings(SerializableStringDictionary watermark_settings){
            return watermark_settings;
        }
    }
}
