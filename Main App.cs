using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Text;
using Npgsql;
using System.Collections.Specialized;
using System.IO;

namespace Behold_Emailer
{
    public partial class Configure : Form
    {
        Logger logger;
        System.Windows.Forms.Timer scheduleMonitorTimer;
        public List<string[]> active_schedules;
        public string active_schedules_queue_filename;
        public StreamWriter completed_schedules_file;
        public string[] watermark_page_location_names;

        public Configure()
        {
            InitializeComponent();
            this.watermark_page_location_names =  new string[]{ "topLeft", "topCenter", "topRight", "bottomLeft", "bottomCenter", "bottomRight"};
            // Initialize shared logger
            this.logger = new Logger("log.txt");

            // Initializing queue system
            int minutes_between_timer = 1;
            this.scheduleMonitorTimer = new System.Windows.Forms.Timer();
            scheduleMonitorTimer.Interval = minutes_between_timer * 60 * 1000;
            this.scheduleMonitorTimer.Tick += runSchedules_TimedEvent;
            this.active_schedules_queue_filename = "active_schedules.csv";
            string completed_schedules_filename = "completed_schedules.csv";
            this.active_schedules = new List<string[]>();
            try
            {
                // Read anything in the active_schedules file
                string[] last_active_scheds = File.ReadAllLines(active_schedules_queue_filename);
                foreach (string sched in last_active_scheds)
                {
                    string[] cols = sched.Split('|');
                    active_schedules.Add(cols);
                    this.logger.Log(String.Format("Loaded {0} at {1} to active schedules from disk", cols[0], cols[1]));
                }

                // Open up the streams to keep track of what happens even if the program fails

                this.completed_schedules_file = new StreamWriter(completed_schedules_filename, true, new UTF8Encoding());
                this.completed_schedules_file.AutoFlush = true;
            }
            catch (IOException ex)
            {
                this.logger.Log(ex.Message);
            }

            // Simple test for first time load with no configs set
            if (Configurator.GetConfig("tableau_server") != "")
            {
                loadAllConfigs();
                loadPageLayoutConfigs();
            
                // Load the Inactive Subscriptions from the Repository
                loadSubscriptionsFromRepository();

               
          }

        }

        private void loadSubscriptionsFromRepository()
        {

            try
            {
                TableauRepository rep = new TableauRepository(Configurator.GetConfig("tableau_server"),
                Configurator.GetConfig("repository_pw"), "readonly");
                rep.logger = this.logger;
                NpgsqlDataReader dr = rep.query_inactive_subscription_schedules();
                Dictionary<string, int> input_box_schedules = new Dictionary<string, int>();
                int rowCount = 0;
                availableSchedulesList.Items.Clear();
                if (dr.HasRows == true)
                {
                    while (dr.Read())
                    {
                        availableSchedulesList.Items.Add(dr.GetString(1));
                        input_box_schedules[dr.GetString(1)] = rowCount;
                        rowCount++;
                    }

                }
                dr.Close();

                // Check the selected schedules from the configs
                StringCollection checked_schedules = Configurator.GetConfigCollection("selected_schedule_names");
                if (checked_schedules != null)
                {

                    foreach (String sched_name in checked_schedules)
                    {
                        availableSchedulesList.SetItemChecked(input_box_schedules[sched_name], true);
                    }
                }
            }
            catch (NpgsqlException ne)
            {
                this.logger.Log("Error with repository while loading the available schedules. Press reload button to try again. Going with existing checked schedules for now");
                this.logger.Log(ne.Message);

                // Fill in the checkbox list based on the existing selected schedules
                StringCollection checked_schedules = Configurator.GetConfigCollection("selected_schedule_names");
                if (checked_schedules != null)
                {
                    var i = 0;
                    Dictionary<string, int> input_box_schedules = new Dictionary<string, int>();
                    availableSchedulesList.Items.Clear();
                    foreach (String sched_name in checked_schedules)
                    {
                        availableSchedulesList.Items.Add(sched_name);
                        input_box_schedules[sched_name] = i;
                        availableSchedulesList.SetItemChecked(input_box_schedules[sched_name], true);
                        i++;
                    }
                }
            }

            catch (ConfigurationException ce)
            {
                this.logger.Log("Incorrect credentials for repository readonly user. Cannot connect to repository.");
                MessageBox.Show("Credentials were not correct for the Repository \"readonly\" user. Please fix");
            }
        }

        /*
         * Loading and Configuration 
         */
        private void saveConfig_Click(object sender, EventArgs e)
        {
            Configurator.SetConfig("server_admin_username", server_admin_username.Text);
            Configurator.SetConfig("server_admin_password", server_password.Text);
            Configurator.SetConfig("tableau_server", tableau_server_url.Text);
            Configurator.SetConfig("repository_pw", repositoryPW.Text);
            Configurator.SetConfig("smtp_server", emailServer.Text);
            Configurator.SetConfig("text_email_template_filename", textEmailFilename.Text);
            Configurator.SetConfig("html_email_template_filename", htmlEmailFilename.Text);
            Configurator.SetConfig("email_sender", emailSender.Text);
            Configurator.SetConfig("export_archive_folder", exportArchiveFolder.Text);
            Configurator.SetConfig("tabcmd_program_location", tabcmdProgramLocation.Text);
            Configurator.SetConfig("tabcmd_config_location", tabcmdConfigLocation.Text);

            StringCollection checked_schedules = new StringCollection();
            foreach (object itemChecked in availableSchedulesList.CheckedItems)
            {
                checked_schedules.Add(itemChecked.ToString());
            }
            Configurator.SetConfig("selected_schedule_names", checked_schedules);

            try
            {
                Configurator.SaveConfig();
                MessageBox.Show("Settings Saved Successfully!");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Settings were not saved correctly.\n\nPlease check all your entries, retry saving, and look at log files for additional info");
                this.logger.Log("Saving settings failed");
                this.logger.Log(exc.Message);
            }
        }

        private void loadConfig_Click(object sender, EventArgs e)
        {
            loadAllConfigs();
        }

        private void loadAllConfigs()
        {
            server_admin_username.Text = Configurator.GetConfig("server_admin_username");
            server_password.Text = Configurator.GetConfig("server_admin_password");
            tableau_server_url.Text = Configurator.GetConfig("tableau_server");
            repositoryPW.Text = Configurator.GetConfig("repository_pw");
            emailServer.Text = Configurator.GetConfig("smtp_server");
            textEmailFilename.Text = Configurator.GetConfig("text_email_template_filename");
            htmlEmailFilename.Text = Configurator.GetConfig("html_email_template_filename");
            emailSender.Text = Configurator.GetConfig("email_sender");
            tabcmdProgramLocation.Text = Configurator.GetConfig("tabcmd_program_location");
            tabcmdConfigLocation.Text = Configurator.GetConfig("tabcmd_config_location");
            exportArchiveFolder.Text = Configurator.GetConfig("export_archive_folder");

        }

        private void loadPageLayoutConfigs()
        {
            foreach (string page_layout_location in this.watermark_page_location_names)
            {
                SerializableStringDictionary watermark_settings = Configurator.GetConfigSerializableStringDict(page_layout_location);
                if (watermark_settings != null)
                {
                    Label label = this.Controls.Find("label_" + page_layout_location, true).FirstOrDefault() as Label;
                    string new_label_text = "";
                    if (watermark_settings["watermark_type"] == "text")
                    {
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
                    // Don't change labels if no response, exit early
                    else
                    {
                        return;
                    }
                    label.Text = new_label_text;
                    Button button = this.Controls.Find("edit_" + page_layout_location, true).FirstOrDefault() as Button;
                    button.Text = "Edit";
                }
            }
        }

        /*
         * Schedules Tab
         * */
        private void runSchedules_TimedEvent(Object source, EventArgs e)
        {
            // Only run on one minute after every 15
            if (DateTime.Now.Minute == 1 || DateTime.Now.Minute == 16 || DateTime.Now.Minute == 31 || DateTime.Now.Minute == 46)
            {
                this.logger.Log("Schedule checks begin");
                this.scheduledEmailsStatus.Text = "Scheduled Emails are running";
                try
                {
                    // Read the schedules that are currently in the repository, to update the active schedule queue
                    TableauRepository rep = new TableauRepository(Configurator.GetConfig("tableau_server"),
                        Configurator.GetConfig("repository_pw"), "readonly");
                    rep.logger = this.logger;

                    NpgsqlDataReader dr = rep.query_inactive_subscription_schedules_for_next_run_time();
                    if (dr.HasRows == true)
                    {
                        using (StreamWriter active_schedules_file = new StreamWriter(this.active_schedules_queue_filename, false, new UTF8Encoding()))
                        {
                            while (dr.Read())
                            {
                                string sched_name = dr.GetString(0);
                                string sched_next_run_time = dr.GetDateTime(1).ToString();

                                // Only add if the sched_name is in the checked schedules
                                int num_checked_items = availableSchedulesList.CheckedItems.Count;
                                string[] checked_schedule_names = new string[num_checked_items];
                                var k=0;
                                foreach (object itemChecked in availableSchedulesList.CheckedItems)
                                {
                                    checked_schedule_names[k] = itemChecked.ToString();
                                    k++;
                                }
                               
                                string[] sched = new string[2] { sched_name, sched_next_run_time };
                                bool schedule_exists_in_queue = false;
                                foreach (string[] queued_sched in this.active_schedules)
                                {
                                    if (queued_sched[0] == sched_name && queued_sched[1] == sched_next_run_time)
                                    {
                                        schedule_exists_in_queue = true;
                                    }
                                }
                                if (schedule_exists_in_queue == false)
                                {
                                    // Only add checked schedule names
                                    if (checked_schedule_names.Any(sched.Contains) == true)
                                    {
                                        this.active_schedules.Add(sched);
                                        this.logger.Log(String.Format("Schedule {0} at {1} added to the queue", sched_name, sched_next_run_time));
                                        active_schedules_file.WriteLine(String.Format("{0}|{1}", sched_name, sched_next_run_time));
                                    }
                                }
                            }
                        }

                    }
                    dr.Close();
                }
                catch (NpgsqlException ne)
                {
                    this.logger.Log("Connecting to repository to update the active queue failed. Ignoring for now, will update at next interval");
                    this.logger.Log(ne.Message);
                }

                // Look at the Active Queue and determine if anything is past it's due date
                foreach (string[] queued_sched in this.active_schedules.Reverse<string[]>())
                {
                    if (DateTime.Now.ToUniversalTime() > DateTime.Parse(queued_sched[1]))
                    {
                        this.logger.Log(String.Format("Schedule {0} at {1} is now eligible to be run.", queued_sched[0], queued_sched[1]));
                        bool result = send_email(queued_sched[0]);
                        if (result == true)
                        {
                            this.logger.Log(String.Format("Schedule {0} at {1} has run successfully. Removing from queue", queued_sched[0], queued_sched[1]));
                            // Write completed schedules into the schedule_run log
                            this.completed_schedules_file.WriteLine(String.Format("{0}|{1}", queued_sched[0], queued_sched[1]));
                            this.active_schedules.Remove(queued_sched);

                        }

                    }
                }
                // Write the queues to the active_schedules files
                using (StreamWriter active_schedules_file = new StreamWriter(this.active_schedules_queue_filename, false, new UTF8Encoding()))
                {
                    foreach (string[] queued_sched in this.active_schedules)
                    {
                        active_schedules_file.WriteLine(String.Format("{0}|{1}", queued_sched[0], queued_sched[1]));
                    }
                }

                this.logger.Log("Schedule check completed successfully");
                this.scheduledEmailsStatus.Text = "Waiting to Run Next Scheduled Emails";
            }
        }

        private void toggleScheduleMonitor(object sender, EventArgs e)
        {
            if (this.scheduleMonitorTimer.Enabled == true){
                this.scheduleMonitorTimer.Stop();
                this.scheduledEmailsStatus.Text = "Scheduled Emails are stopped";
                this.startScheduleMonitoring.Text = "Start Scheduled Emails";
            }
            else if (this.scheduleMonitorTimer.Enabled == false){
                this.scheduleMonitorTimer.Start();
                this.scheduledEmailsStatus.Text = "Waiting to Run Next Scheduled Emails";
                this.startScheduleMonitoring.Text = "Stop Scheduled Emails";
            }
        }

        private void stopTimer(object sender, EventArgs e)
        {
            this.scheduleMonitorTimer.Stop();
        }

        private void runSelectedSchedules_Click(object sender, EventArgs e)
        {
            Progress progress = new Progress(0, "Running selected schedules once");
            progress.Show(this);
            progress.Update();
            foreach (object itemChecked in availableSchedulesList.CheckedItems)
            {
                this.send_email(itemChecked.ToString());
            }
            progress.finish_progress_bar(100);
            progress.update_status("Schedule runs completed!");

        }

        private void exportArchiveButton_Click(object sender, EventArgs e)
        {
            DialogResult result = exportArchivePicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = exportArchivePicker.SelectedPath + "\\";
                exportArchiveFolder.Text = filename;
            }
        }


        /*
         * Test Page Methods
         */ 
      

        private void create_test_export(object sender, EventArgs e)
        {
            // Needs validation here
            TextBox[] elements_to_validate = { testFileSaveLocation, testFilename, testSite, testUsernameForImpersonation, testViewLocation };
            bool validation = this.validate_set_of_elements(elements_to_validate);
            if (validation == false)
            {
                return;
            }
            Progress progress = new Progress(0, "Creating test file");
            progress.Show(this);
            progress.Update();
            try
            {
                TableauRepository tabrep = new TableauRepository(tableau_server_url.Text, repositoryPW.Text, "readonly");
                tabrep.logger = this.logger;
                Tabcmd tabcmd = new Tabcmd(tabcmdProgramLocation.Text, tableau_server_url.Text, server_admin_username.Text, server_password.Text, testSite.Text,
                    tabcmdConfigLocation.Text, tabrep, this.logger);
                
                BeholdEmailer tabemailer = new BeholdEmailer(tabcmd, emailServer.Text, "", "");
                Watermarker wm = new Watermarker();
                string[] page_locations = { "top_left", "top_center", "top_right", "bottom_left", "bottom_center", "bottom_right" };
                foreach (string page_location in page_locations)
                {
                    string settingsPageLocation = page_location.Split('_')[0] + page_location.Split('_')[1].First() + page_location.Split('_')[1].Substring(1);
                    wm.setPageLocationWatermarkFromConfig(page_location, Configurator.GetConfigSerializableStringDict(settingsPageLocation));
                }
                
                string filename = tabemailer.generate_export_and_watermark(testUsernameForImpersonation.Text, testViewLocation.Text, "fullpdf", new Dictionary<string, string>(), wm);
                string[] file_ending = filename.Split('.');

                string final_filename = String.Format("{0}{1}.{2}",testFileSaveLocation.Text, testFilename.Text, file_ending[file_ending.Length - 1]);
                
                this.logger.Log(String.Format("Finalizing file and putting it here: {0}", final_filename));
                File.Copy(filename, final_filename, true);
                this.logger.Log(String.Format("Removing original file {0}", filename));
                File.Delete(filename);
                progress.finish_progress_bar(100);
                progress.update_status("Export created successfully!");

            }
            catch (ConfigurationException ce)
            {
                progress.finish_progress_bar(33);
                progress.update_status("Export failed for some reason, most likely bad settings.\nCheck logs for more info");
                this.logger.Log(ce.Message);
            }
        }

        private void send_test_email(object sender, EventArgs e)
        {
            // Valdate the form elements
            TextBox[] elements_to_validate = { testSite, testViewLocation, testEmailRecipient, testEmailSubject };
            bool validation = this.validate_set_of_elements(elements_to_validate);
            if (validation == false)
            {
                return;
            }

            // Show progress popup
            Progress p = new Progress(30, "Creating and Sending Test Email");
            p.Show(this);
            p.Update();
            bool result = this.send_email("");
            if (result == true)
            {
                p.finish_progress_bar(100);
                p.update_status("E-mail sent succesfully");
            }
            else
            {
                p.finish_progress_bar(30);
                p.update_status("E-mail failed.\nPlease check configurations and try again.\nSee log for details");
            }
           
        }

        /*
         * Main E-mailing methods (used by test and schedule)
         */ 
        public bool send_email(string schedule_name)
        {

            try
            {
                TableauRepository tabrep = new TableauRepository(tableau_server_url.Text, repositoryPW.Text, "readonly");
                tabrep.logger = this.logger;
                Tabcmd tabcmd = new Tabcmd(tabcmdProgramLocation.Text, tableau_server_url.Text, server_admin_username.Text, server_password.Text, testSite.Text,
                    tabcmdConfigLocation.Text, tabrep, this.logger);

                BeholdEmailer tabemailer = new BeholdEmailer(tabcmd, emailServer.Text, "", "");
                tabemailer.logger = this.logger;
                tabemailer.html_email_template_filename = htmlEmailFilename.Text;
                tabemailer.text_email_template_filename = textEmailFilename.Text;
                Watermarker wm = new Watermarker();
                string[] page_locations = { "top_left", "top_center", "top_right", "bottom_left", "bottom_center", "bottom_right" };
                foreach (string page_location in page_locations)
                {

                    string settingsPageLocation = page_location.Split('_')[0] + page_location.Split('_')[1].First() + page_location.Split('_')[1].Substring(1);
                    wm.setPageLocationWatermarkFromConfig(page_location, Configurator.GetConfigSerializableStringDict(settingsPageLocation));
                }

                try
                {
                    bool result;
                    

                    if (schedule_name != "")
                    {
                        tabemailer.export_archive_folder = exportArchiveFolder.Text;
                        result = tabemailer.generate_emails_from_named_schedule_in_repository(schedule_name, emailSender.Text, wm);
                    }
                    else
                    {
                        
                        result = tabemailer.generate_email_from_view(emailSender.Text, testEmailRecipient.Text, testEmailSubject.Text, "Please see attached Tableau PDF", testUsernameForImpersonation.Text,
                        testViewLocation.Text, "fullpdf", new Dictionary<String, String>(), wm);
                    }

                    return result;
                }
                catch (ConfigurationException ce)
                {

                    this.logger.Log(ce.Message);
                    return false;
                }
            }
            // From Repository Failing
            catch (ConfigurationException ce){
                this.logger.Log(ce.Message);
                return false;
            }
            
        }

        /*
         * Tableau Server Settings Tab Events
         */ 

        private void pickTabcmdConfigFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = tabcmdFolderPicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = tabcmdFolderPicker.SelectedPath + "\\";
                this.tabcmdConfigLocation.Text = filename;
            }
        }

        private void pickTabcmdProgramFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = tabcmdProgramPicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = tabcmdProgramPicker.SelectedPath + "\\";
                this.tabcmdProgramLocation.Text = filename ;
            }
        }


        private void textEmailTemplateButton_Click(object sender, EventArgs e)
        {
            DialogResult result = textEmailPicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = textEmailPicker.FileName;
                this.textEmailFilename.Text = filename;
            }
        }

        private void htmlEmailTemplateButton_Click(object sender, EventArgs e)
        {
            DialogResult result = htmlEmailPicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = htmlEmailPicker.FileName;
                this.htmlEmailFilename.Text = filename;
            }
        }

        private void validateTextBox(object sender, CancelEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text == "")
            {
                t.BackColor = System.Drawing.Color.LightSalmon;
                t.Focus();
            }
            else
            {
                t.BackColor = System.Drawing.Color.White;
            }
        }

        private void tableau_server_url_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.StartsWith("http") == false)
            {
                MessageBox.Show("Tableau Server URL must start with http:// or https://");
                t.BackColor = System.Drawing.Color.LightSalmon;
                t.Focus();
                
            }
            else if (t.Text.Contains("://") == false){
                MessageBox.Show("Tableau Server URL malformed, must start with http:// or https://");
                t.BackColor = System.Drawing.Color.LightSalmon;
                t.Focus();
            }

        }

        private bool validate_set_of_elements(Array elements)
        {
            bool all_clear = true;
            foreach (TextBox element in elements)
            {
                element.Text = element.Text.Trim();
                if (element.Text == "")
                {
                    all_clear = false;
                    element.BackColor = Color.LightSalmon;
                }
                else
                {
                    element.BackColor = Color.White;
                }

            };
            if (all_clear == false)
            {
                MessageBox.Show("Please fill out all necessary information");
            }
            return all_clear;
        }

        /*
         * Watermarking Tab Events
         */ 
        private void edit_watermarker(object sender, EventArgs e)
        {
            Control box = (Control)sender;
            Button button = (Button)sender;
            
            string page_location = box.Name.Split('_')[1];
            Label label = this.Controls.Find("label_" + page_location, true).FirstOrDefault() as Label;
            if (label.Text == "")
            {
                watermarkContextMenu.Show(button, new Point(0, button.Height));
            }
            else
            {
                SerializableStringDictionary related_config = Configurator.GetConfigSerializableStringDict(page_location);
                if (label.Text == "Text")
                {
                    Form wm = new Text_Watermark(page_location, related_config);
                    wm.ShowDialog(this);
                }
                else if (label.Text == "Page Number")
                {
                    Form wm = new Page_Number_Watermark(page_location, related_config);
                    wm.ShowDialog(this);
                }
                else if (label.Text == "Image")
                {
                    Form wm = new Image_Watermark(page_location, related_config);
                    wm.ShowDialog(this);
                }
            }
        }

        private void clear_watermark_config(object sender, EventArgs e)
        {
            Button s = (Button)sender;
            string page_location = s.Name.Split('_')[1];
            DialogResult results = MessageBox.Show("Delete existing watermark configuration?","Remove Watermark?", MessageBoxButtons.YesNo);
            if (results == DialogResult.Yes)
            {
                SerializableStringDictionary empty_config = new SerializableStringDictionary();
                Configurator.SetConfig(page_location, empty_config);
                Configurator.SaveConfig();
                Label label = this.Controls.Find("label_" + page_location, true).FirstOrDefault() as Label;
                label.Text = "";
                Button button = this.Controls.Find("edit_" + page_location, true).FirstOrDefault() as Button;
                button.Text = "Add";
            }
        }

        private void watermarkContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            ToolStripItem m = (ToolStripItem)e.ClickedItem;
            ContextMenuStrip cm = (ContextMenuStrip)m.Owner;
            Control c = cm.SourceControl;
            string page_location = c.Name.Split('_')[1];

            if (e.ClickedItem.Text == "Text")
            {
                Form wm = new Text_Watermark(page_location);
                wm.ShowDialog(this);
            }
            else if (e.ClickedItem.Text == "Page Number")
            {
                Form wm = new Page_Number_Watermark(page_location);
                wm.ShowDialog(this);
            }
            else if (e.ClickedItem.Text == "Image")
            {
                Form wm = new Image_Watermark(page_location);
                wm.ShowDialog(this);
            }

        }

        private void refreshSchedulesButton_Click(object sender, EventArgs e)
        {
            loadSubscriptionsFromRepository();
        }

        private void testFilePicker_Click(object sender, EventArgs e)
        {
            DialogResult result = exportArchivePicker.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = exportArchivePicker.SelectedPath + "\\";
                this.testFileSaveLocation.Text = filename;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configurator.EncryptConfig();
        }


    }
}
