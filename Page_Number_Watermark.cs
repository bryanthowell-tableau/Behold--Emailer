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
    public partial class Page_Number_Watermark : Watermark_Edit
    {
        public Page_Number_Watermark(string caller_name)
            : base(caller_name)
        {
            InitializeComponent();
        }

        public Page_Number_Watermark(string caller_name, SerializableStringDictionary watermark_settings)
            : base(caller_name)
        {
            InitializeComponent();
            pageNumberFontName.Text = watermark_settings["font_name"];
            pageNumberFontSize.Text = watermark_settings["font_size"];
            pageNumbersPrefix.Text = watermark_settings["text"];
            FontStyle fstyle;
            if (pageNumberFontStyle.Text == "Regular")
            {
                fstyle = FontStyle.Regular;
            }
            else if (pageNumberFontStyle.Text == "Bold")
            {
                fstyle = FontStyle.Bold;
            }
            else if (pageNumberFontStyle.Text == "Italic")
            {
                fstyle = FontStyle.Italic;
            }
            else
            {
                fstyle = FontStyle.Regular;
            }
            Font settings_font = new Font(watermark_settings["font_name"], Int32.Parse(watermark_settings["font_size"]), fstyle);
   
        }

        private void Page_Number_Watermark_Load(object sender, EventArgs e)
        {

        }

        protected override SerializableStringDictionary save_settings(SerializableStringDictionary watermark_settings)
        {
            {
                watermark_settings["watermark_type"] = "page_number";
                watermark_settings["font_name"] = pageNumberFontName.Text;
                watermark_settings["font_size"] = pageNumberFontSize.Text;
                watermark_settings["font_style"] = pageNumberFontStyle.Text;
                watermark_settings["text"] = pageNumbersPrefix.Text;
                watermark_settings["show_total"] = pageNumbersShowTotal.Text;
                return watermark_settings;

            }
        }

        private void setFont_Click(object sender, EventArgs e)
        {
            FontStyle fstyle;
            if (pageNumberFontStyle.Text == "Regular")
            {
                fstyle = FontStyle.Regular;
            }
            else if (pageNumberFontStyle.Text == "Bold")
            {
                fstyle = FontStyle.Bold;
            }
            else if (pageNumberFontStyle.Text == "Italic")
            {
                fstyle = FontStyle.Italic;
            }
            else
            {
                fstyle = FontStyle.Regular;
            }
            fontDialog1.Font = new Font(pageNumberFontName.Text,  float.Parse(pageNumberFontSize.Text), fstyle);
            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                pageNumberFontSize.Text = Math.Round(fontDialog1.Font.SizeInPoints, 0).ToString();
                pageNumberFontName.Text = fontDialog1.Font.Name;
                pageNumberFontStyle.Text = fontDialog1.Font.Style.ToString();
            }
        }
    }
}
