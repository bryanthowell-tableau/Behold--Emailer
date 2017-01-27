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
    public partial class Text_Watermark : Watermark_Edit
    {
        public Text_Watermark(string caller_name): base(caller_name)
        {
            InitializeComponent();
        }

        public Text_Watermark(string caller_name, SerializableStringDictionary watermark_settings)
            : base(caller_name)
        {
            InitializeComponent();
            wmFontName.Text = watermark_settings["font_name"];
            wmFontSize.Text = watermark_settings["font_size"];
            wmFontStyle.Text = watermark_settings["font_style"];
            wmText.Text = watermark_settings["text"];
            wmTimestamp.Text = watermark_settings["add_timestamp"];
            FontStyle fstyle;
            if (wmFontStyle.Text == "Regular")
            {
                fstyle = FontStyle.Regular;
            }
            else if (wmFontStyle.Text == "Bold")
            {
                fstyle = FontStyle.Bold ;
            }
            else if (wmFontStyle.Text == "Italic")
            {
                fstyle = FontStyle.Italic;
            }
            else
            {
                fstyle = FontStyle.Regular;
            }
            Font settings_font = new Font(watermark_settings["font_name"], Int32.Parse(watermark_settings["font_size"]), fstyle);
            fontDialog1.Font = settings_font;
        }

        public Text_Watermark()
        {
            InitializeComponent();
        }

        protected override SerializableStringDictionary save_settings(SerializableStringDictionary watermark_settings){
            watermark_settings["watermark_type"] = "text";
            watermark_settings["font_name"] = wmFontName.Text;
            watermark_settings["font_size"] = wmFontSize.Text;
            watermark_settings["font_style"] = wmFontStyle.Text;
            watermark_settings["text"] = wmText.Text;
            watermark_settings["add_timestamp"] = wmTimestamp.Text;
            return watermark_settings;
        }


        private void setFont_Click(object sender, EventArgs e)
        {
            FontStyle fstyle;
            if (wmFontStyle.Text == "Regular")
            {
                fstyle = FontStyle.Regular;
            }
            else if (wmFontStyle.Text == "Bold")
            {
                fstyle = FontStyle.Bold ;
            }
            else if (wmFontStyle.Text == "Italic")
            {
                fstyle = FontStyle.Italic;
            }
            else
            {
                fstyle = FontStyle.Regular;
            }
            fontDialog1.Font = new Font(wmFontName.Text, float.Parse(wmFontSize.Text), fstyle);
            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                wmFontSize.Text = Math.Round(fontDialog1.Font.SizeInPoints, 0).ToString();
                wmFontName.Text = fontDialog1.Font.Name;
                wmFontStyle.Text = fontDialog1.Font.Style.ToString();
            }
        }
    }
}
