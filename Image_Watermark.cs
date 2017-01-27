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
    public partial class Image_Watermark : Watermark_Edit
    {
        public Image_Watermark(string caller_name)
            : base(caller_name)
        {
            InitializeComponent();
        }

        public Image_Watermark(string caller_name, SerializableStringDictionary watermark_settings)
            : base(caller_name)
        {
            InitializeComponent();
            wmImage.Text = watermark_settings["image_location"];
        }

        protected override SerializableStringDictionary save_settings(SerializableStringDictionary watermark_settings)
        {
            {
                watermark_settings["watermark_type"] = "image";
                watermark_settings["image_location"] = wmImage.Text;
                return watermark_settings;
            }

        }

        private void pickHeaderLogoLocation_Click(object sender, EventArgs e)
        {
            DialogResult result = openLogoLocation.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = openLogoLocation.FileName;
                Bitmap img = new Bitmap(openLogoLocation.FileName);
                if (img.Height > 40)
                {
                    MessageBox.Show("Chosen image is taller than 40px and will cover the content. Please choose a shorter image");
                }
                else
                {
                    this.wmImage.Text = filename;
                }
                img.Dispose();
            }
        }


    }
}
