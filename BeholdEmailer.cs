using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.IO;
using System.Diagnostics;
using System.Net.Mime;
using Npgsql;
using System.Data;

namespace Behold_Emailer
{
    class BeholdEmailer
    {
        string tableau_server_url;
        string repository_pw;
        SmtpClient smtp_server;
        Tabcmd tabcmd;
        public string html_email_template_filename {get; set;}
        public string text_email_template_filename {get; set;}
        public Logger logger;
        public string export_archive_folder;

        public BeholdEmailer(string tabcmd_dir, string tabcmd_config_location, string repository_pw, string tableau_server_url, 
            string tableau_server_admin_user, string tableau_server_admin_pw, string smtp_server, string smtp_username, string smtp_password)
        {
            this.tableau_server_url = tableau_server_url;
            this.repository_pw = repository_pw;
            this.smtp_server = new SmtpClient(smtp_server);
            this.html_email_template_filename = "";
            this.text_email_template_filename = "";
            // Add credentials stuff here

            this.tabcmd = new Tabcmd(tabcmd_dir, tableau_server_url, tableau_server_admin_user, tableau_server_admin_pw, "default", repository_pw, tabcmd_config_location);
            this.logger = null;
            this.export_archive_folder = null;
        }

        public BeholdEmailer(Tabcmd Tabcmd, string smtp_server)
        {
            this.tabcmd = Tabcmd;
            this.tableau_server_url = this.tabcmd.tableau_server_url;
            this.smtp_server = new SmtpClient(smtp_server);
            this.html_email_template_filename = "";
            this.text_email_template_filename = "";
            this.logger = null;
            this.export_archive_folder = null;
        }

        public BeholdEmailer(Tabcmd Tabcmd, SmtpClient smtp_client)
        {
            this.tabcmd = Tabcmd;
            this.tableau_server_url = this.tabcmd.tableau_server_url;
            this.smtp_server = smtp_client;
            this.html_email_template_filename = "";
            this.text_email_template_filename = "";
            this.logger = null;
            this.export_archive_folder = null;
        }

        public void log(string l)
        {
            if (this.logger != null)
            {
                this.logger.Log(l);
            }
        }

        public void email_file_from_template(string from_user, string[] to_users, string[] cc_users, string[] bcc_users, string subject, string simple_message_text, string filename_to_attach)
        {
            string message_text = "";
            // Load Text template
            if (this.text_email_template_filename != "")
            {
                try
                {
                    using (StreamReader sr = new StreamReader(this.text_email_template_filename))
                    {
                        message_text = sr.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else{
                message_text = simple_message_text;
            }

            MailMessage message = new MailMessage(from_user, to_users[0]);
            message.Body = message_text;
            message.Subject = subject;

            var to_collect = message.To;
            var cc_collect = message.CC;
            var bcc_collect = message.Bcc;
            for (var i = 1; i < to_users.Length; i++)
            {
                var add = new MailAddress(to_users[i]);
                to_collect.Add(add);
            }

            for (var i = 0; i < cc_users.Length; i++)
            {
                if (cc_users[i] == "") { continue; }
                var add = new MailAddress(cc_users[i]);
                cc_collect.Add(add);
            }

            for (var i = 0; i < bcc_users.Length; i++)
            {
                if (bcc_users[i] == "") { continue; }
                var add = new MailAddress(bcc_users[i]);
                bcc_collect.Add(add);
            }

            // Load HTML template as alternative
            if (this.html_email_template_filename != "")
            {
                try
                {
                    using (StreamReader sr = new StreamReader(this.html_email_template_filename))
                    {
                        ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                        AlternateView htmlAltView = AlternateView.CreateAlternateViewFromString(sr.ReadToEnd(), mimeType);
                        message.AlternateViews.Add(htmlAltView);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
  
            Attachment att = new Attachment(filename_to_attach);
            message.Attachments.Add(att);
            this.smtp_server.Send(message);

            // Must close the attachment or file will stay locked to the program and you can't clean up
            att.Dispose();

        }

        public string generate_export_and_watermark(string view_user, string view_location, string content_type, 
            Dictionary<string, string> view_filter_dictionary, Watermarker watermark)
        {
            string filename_to_attach = this.tabcmd.create_export(content_type, view_location, view_filter_dictionary, view_user, "exported_workbook");
            this.log(String.Format("PDF created and saved successfully as {0}", filename_to_attach));

            // Watermark the PDF (working copy)
            if (watermark != null)
            {
                this.log("Adding watermark / header / footer to exported PDF");
                watermark.add_watermark_to_pdf(filename_to_attach);
                this.log("Watermarking performed successfully");
            }
            return filename_to_attach;          
        }

        public bool generate_email_from_view(string email_from_user, string[] email_to, string[] email_cc, string[] email_bcc, string email_subject, string email_template_name,
            string view_user, string view_location, string email_content_type, Dictionary<string, string> view_filter_dictionary, Watermarker watermark)
        {
            this.log(String.Format("Creating PDF export from tabcmd for {0} for user {1}", view_location, view_user));
            try
            {

                string filename_to_attach = this.generate_export_and_watermark(view_user, view_location, email_content_type, view_filter_dictionary, watermark);
                this.log(String.Format("PDF created and saved successfully as {0}", filename_to_attach));

                // Copy the file with a new name so that it can be archived
                string[] file_ending = filename_to_attach.Split('.');

                string timestamp_string = DateTime.UtcNow.ToString("s");
                timestamp_string = timestamp_string.Replace(":", "_");
                timestamp_string = timestamp_string.Replace("-", "_");
                string final_filename = String.Format("{0} - {1} - {2}.{3}", email_subject, view_user, timestamp_string, file_ending[file_ending.Length-1]);
                if (this.export_archive_folder != null)
                {
                    final_filename = this.export_archive_folder + final_filename;
                    this.log(String.Format("Achiving export to {0}", final_filename));
                }
                
                File.Copy(filename_to_attach, final_filename, true);

                this.log(String.Format("Sending e-mail of exported and watermarked PDF to {0}", email_to[0]));
                this.email_file_from_template(email_from_user, email_to, email_cc, email_bcc, email_subject, email_template_name, final_filename);
                this.log(String.Format("Removing original file {0}", filename_to_attach));
                File.Delete(filename_to_attach);
                
                // Cleanup if no archive
                if(this.export_archive_folder == ""){
                    this.log(String.Format("Removing e-mailed file {0}", final_filename));
                    File.Delete(final_filename);
                }

                this.log("Email sent successfully");
                return true;
            }            
            catch (ConfigurationException ce)
            {
                this.log(ce.Message);
                return false;
            }
        }

        public bool generate_emails_from_named_schedule_in_repository(string schedule_name, string from_user, Watermarker watermarker)
        {
            NpgsqlDataReader dr = this.tabcmd.repository.query_subscriptions_for_users(schedule_name, true);
            bool all_suceeded = true;
            var dataTable = new DataTable();
            dataTable.Load(dr);
            dr.Close();
            foreach(DataRow row in dataTable.Rows)
            {
                string email_subject = row[1].ToString();
                string user = row[2].ToString();
                string site = row[3].ToString();
                string view_location = row[4].ToString();
                string user_email = row[6].ToString();

                this.tabcmd.site = site;
                this.log(String.Format("Generating e-mail of view {0} on site {1} for {2} at {3}", view_location, site, user, user_email));


                bool result = this.generate_email_from_view(from_user, new string[1] {user_email}, new string[0]{}, new string[0]{}, email_subject, "", user, view_location, "fullpdf", 
                    new Dictionary<string,string>(), watermarker);
                if (all_suceeded == true && result == false){
                    all_suceeded = false;
                }
                if (result == true)
                {
                    this.log("Email generated succesfully");
                }
                else
                {
                    this.log("Error creating email");
                }
            }
            return all_suceeded;
        }




    }

    class Watermarker
    {
        public double page_height;
        public double page_width;
        public Dictionary <string, Watermark> page_locations;

        public Watermarker()
        {
            this.page_locations = new Dictionary<string,Watermark>();
            this.page_locations["top_left"] = null;
            this.page_locations["top_right"] = null;
            this.page_locations["top_center"] = null;
            this.page_locations["bottom_left"] = null;
            this.page_locations["bottom_center"] = null;
            this.page_locations["bottom_right"] = null;
        }

        public bool setPageLocationWatermarkFromConfig(string page_location, SerializableStringDictionary config_dict){
            if (config_dict != null)
            {

                if (config_dict["watermark_type"] == "text")
                {
                    page_locations[page_location] = new Text_Watermark(config_dict["text"]);
                    page_locations[page_location].font_name = config_dict["font_name"];
                    page_locations[page_location].font_size = Int32.Parse(config_dict["font_size"]);
                    page_locations[page_location].font_style = config_dict["font_style"];
                    if (config_dict["add_timestamp"] == "Yes")
                    {
                        page_locations[page_location].add_timestamp = true;
                    }
                    page_locations[page_location].page_location = page_location;
                }
                else if (config_dict["watermark_type"] == "image"){
                    page_locations[page_location] = new Image_Watermark(config_dict["image_location"]);
                    page_locations[page_location].page_location = page_location;
                }

                else if (config_dict["watermark_type"] == "page_number")
                {
                    bool show_total = false;
                    if ( config_dict["show_total"] == "Yes"){
                        show_total = true;
                    }
                    page_locations[page_location] = new PageNumberer(config_dict["text"], show_total);
                    page_locations[page_location].font_name = config_dict["font_name"];
                    page_locations[page_location].font_size = Int32.Parse(config_dict["font_size"]);
                    page_locations[page_location].font_style = config_dict["font_style"];
                    page_locations[page_location].page_location = page_location;

                }
            }
            return true;
        }

        
        public abstract class Watermark
        {
            public string message { get; set; }
            public int height_offset_max { get; set; }
            public string logo_file_location { get; set; }
            protected string _justification;
            protected string _page_location;
            abstract public string page_location { get; set; }
            
            public int width_offset { get; set; }
            public int font_size { get; set; }
            public string font_name { get; set; }
            public string font_style { get; set; }
            public bool add_timestamp { get; set; }
            public double page_height;
            public double page_width;
            protected XRect drawing_box;
            public double box_padding_left_right;
            public double box_padding_top_bottom;
            protected XStringFormat string_format;

            public Watermark(string message)
            {
                this.message = message;
                this.logo_file_location = null;
                this.font_size = 8;
                this.font_name = "Times New Roman";
                this.font_style = "Regular";
                this.add_timestamp = false;
                this.box_padding_left_right = 10;
                this.box_padding_top_bottom = 10;
                this.string_format = new XStringFormat();

                // There is about 40 pixels to work with before the viz starts and 40 from bottom after viz ends
                this.height_offset_max = 40;
            }

            public void setPageHeightAndWidth(double height, double width)
            {
                this.page_height = height;
                this.page_width = width;
                if (this._page_location.Contains("top")){
                    this.drawing_box = new XRect(0 + this.box_padding_left_right, 0 + this.box_padding_top_bottom,
                        this.page_width - (this.box_padding_left_right * 2), this.height_offset_max);
                }
                else if (this._page_location.Contains("bottom")){
                    this.drawing_box = new XRect(0 + this.box_padding_left_right, this.page_height - this.height_offset_max, 
                    this.page_width - (this.box_padding_left_right * 2), this.height_offset_max - this.box_padding_top_bottom);
                }
            }

            public virtual void write_watermark(XGraphics gfx, int page_number, int pages_count)
            {
                
               if (logo_file_location != null)
                {
                    XImage img = XImage.FromFile(this.logo_file_location);
                    // Align the image to the text
                    double h = img.PointHeight;
                    double w = img.PointWidth;
                    // Push the text box by the width of the image
                    this.drawing_box.X = this.drawing_box.X + w;
                    this.drawing_box.Width = this.drawing_box.Width - w;
                    
                    XPoint image_point = new XPoint(this.drawing_box.Left - w, this.drawing_box.Top);
                    gfx.DrawImage(img, image_point);
                    //double new_text_width = 30.0 + 5.0 + w;
                    //text_point = new XPoint(new_text_width, point.Y);
                }
                XFontStyle fstyle;
                if (this.font_style == "Regular")
                {
                    fstyle = XFontStyle.Regular;
                }
                else if (this.font_style == "Bold")
                {
                    fstyle = XFontStyle.Bold ;
                }
                else if (this.font_style == "Italic")
                {
                    fstyle = XFontStyle.Italic;
                }
                else
                {
                    fstyle = XFontStyle.Regular;
                }
                XFont font = new XFont(this.font_name, this.font_size, fstyle);
                string final_message = this.message;
                if (this.add_timestamp == true)
                {
                    final_message += " " + DateTime.UtcNow.ToString("s");
                }
                //gfx.DrawString(this.message, font, XBrushes.Black, text_point);

                gfx.DrawString(final_message, font, XBrushes.Black, this.drawing_box, this.string_format);
            }

        }

        public class Text_Watermark : Watermark
        {
            public override string page_location
            {
                get { return _page_location; }
                set
                {
                    this._justification = value;
                    this._page_location = value;

                    // Vertical alignment
                    if (value.Contains("top"))
                    {
                        this.string_format.LineAlignment = XLineAlignment.Near;
                    }
                    if (value.Contains("bottom"))
                    {
                        this.string_format.LineAlignment = XLineAlignment.Far;
                    }
                    
                    // Horizontal alignment
                    if (value.Contains("left"))
                    {
                        this.string_format.Alignment = XStringAlignment.Near;
                       
                    }
                    else if (value.Contains("center"))
                    {
                        this.string_format.Alignment = XStringAlignment.Center;
                    }
                    else if (value.Contains("right"))
                    {
                        this.string_format.Alignment = XStringAlignment.Far;
                    }
                }
            }

            public Text_Watermark(string message): base(message)
            {

            }
        }

        public class Image_Watermark : Watermark
        {
            public string image_location;
            public override string page_location
            {
                get { return _page_location; }
                set
                {
                    this._justification = value;
                    this._page_location = value;

                    // Vertical alignment
                    if (value.Contains("top"))
                    {
                        this.string_format.LineAlignment = XLineAlignment.Near;
                    }
                    if (value.Contains("bottom"))
                    {
                        this.string_format.LineAlignment = XLineAlignment.Far;
                    }

                    // Horizontal alignment
                    if (value.Contains("left"))
                    {
                        this.string_format.Alignment = XStringAlignment.Near;

                    }
                    else if (value.Contains("center"))
                    {
                        this.string_format.Alignment = XStringAlignment.Center;
                    }
                    else if (value.Contains("right"))
                    {
                        this.string_format.Alignment = XStringAlignment.Far;
                    }
                }
            }

            public Image_Watermark(string image_location)
                : base("")
            {
                this.image_location = image_location;
            }

            public override void write_watermark(XGraphics gfx, int page_number, int pages_count)
            {

                XImage img = XImage.FromFile(this.image_location);
                // Align the image to the text
                double h = img.PointHeight;
                double w = img.PointWidth;
                // Push the text box by the width of the image
                this.drawing_box.X = this.drawing_box.X + w;
                this.drawing_box.Width = this.drawing_box.Width - w;

                XPoint image_point = new XPoint(this.drawing_box.Left - w, this.drawing_box.Top);
                gfx.DrawImage(img, image_point);
            }
        }

        public class PageNumberer : Text_Watermark
        {
            public bool page_of_total { get; set; }
            public string page_number_prefix { get; set; }

            public PageNumberer(string page_number_prefix, bool page_of_total)
                : base(page_number_prefix)
            {
                // page_number_prefix becomes message of Text_Watermark parent class
                this.page_of_total = page_of_total;

            }

            public override void write_watermark(XGraphics gfx, int page_number, int pages_count)
            {
                string page_number_text;
                // X of N
                if (this.page_of_total == true)
                {
                    page_number_text = String.Format("{0} of {1}", page_number + 1, pages_count);
                }
                else
                {
                    page_number_text = String.Format("{0}", page_number + 1);
                }

                if (this.message != "")
                {
                    page_number_text = String.Format("{0} {1}", this.message, page_number_text);
                }
                XFontStyle fstyle;
                if (this.font_style == "Regular")
                {
                    fstyle = XFontStyle.Regular;
                }
                else if (this.font_style == "Bold")
                {
                    fstyle = XFontStyle.Bold;
                }
                else if (this.font_style == "Italic")
                {
                    fstyle = XFontStyle.Italic;
                }
                else
                {
                    fstyle = XFontStyle.Regular;
                }

                XFont pnfont = new XFont(this.font_name, this.font_size, fstyle);
                gfx.DrawString(page_number_text, pnfont, XBrushes.Black, this.drawing_box, this.string_format);
            }

        }

        public void add_watermark_to_pdf(string filename)
        {
            // Don't bother doing anything if no watermark is set
            bool any_watermark_objects = false;
            foreach (string page_location in this.page_locations.Keys)
            {
                if (page_locations[page_location] != null)
                {
                    any_watermark_objects = true;
                }
            }
            if (any_watermark_objects == false)
            {
                return;
            }
            
            PdfDocument document = PdfReader.Open(filename);
            
            int page_count = document.Pages.Count;
            
           /* if (this.first_page_only == true)
            {
                page_count = 1;
            }*/
            for (int i = 0; i < page_count; i++)
            {
                PdfPage page = document.Pages[i];
                XGraphics gfx = XGraphics.FromPdfPage(page);
                this.page_height = page.Height.Point;
                this.page_width = page.Width.Point;
                
                foreach(string page_location in page_locations.Keys){
                    if (page_locations[page_location] != null)
                    {
                        page_locations[page_location].setPageHeightAndWidth(this.page_height, this.page_width);
                        page_locations[page_location].write_watermark(gfx, i, page_count);
                    }
                }

                gfx.Dispose();
            }

            document.Save(filename);

            document.Close();
            document.Dispose();
        }

    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException() { }

    }



    class Logger
    {
        private StreamWriter logfile;
        public Logger(string logfile)
        {
                this.logfile = File.AppendText(logfile);
                this.Separator();
                this.logfile.AutoFlush = true;
                this.Log("Tableau Emailer started, logging begins");
                
        }

        public void Log(string logMessage)
        {
            this.logfile.WriteLine("{0} {1} : {2}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString(), logMessage);
            
        }

        public void Separator()
        {
            this.logfile.WriteLine("-------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
