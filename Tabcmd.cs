using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization.Json;
using Npgsql;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace Behold_Emailer
{
    class Tabcmd
    {
        private string tabcmd_folder;
        private string username;
        private string password;
        private string _site;
        public string site {
            get { return _site; }
            set { if (value.ToLower() == "default") { _site = "default"; } else { _site = value; } }
        }
        public string tableau_server_url;

        private string repository_pw;
        private string user_session_id;
        private string user_auth_token;
        private string tabcmd_config_location;
        private string tabcmd_config_filename;
        public Logger logger;
        public TableauRepository repository;

        public Tabcmd(string tabcmd_folder, string tableau_server_url,
            string username, string password, string site, string repository_password, 
            string tabcmd_config_location)
        {
            // Tabcmd program configurations
            this.tabcmd_folder = tabcmd_folder;
            this.tabcmd_config_filename = "tabcmd-session.xml";
            this.tabcmd_config_location = tabcmd_config_location;
            // Open the configuration file and test whether it resembles the real file
            try {
                StreamReader tabcmd_config = new StreamReader(tabcmd_config_location + tabcmd_config_filename);
                // Read first line
                tabcmd_config.ReadLine();
                // Read second line, should be <session>. May come up with better test
                string second_line = tabcmd_config.ReadLine();
                if(second_line.Contains("<session>") == false){
                    throw new ConfigurationException("tabcmd-session.xml file information is incorrect. File is not tabcmd-session.xml");
                }
                tabcmd_config.Close();
            }
            catch(IOException){

                throw new ConfigurationException("tabcmd-config file information is incorrect. Config file could not be opened");
            }

            this.username = username;
            this.password = password;
            this.site = site;
            this.tableau_server_url = tableau_server_url;
            
            this.repository_pw = repository_password;
            this.repository = new TableauRepository(this.tableau_server_url, this.repository_pw, "readonly");

            // This preps tabcmd for subsequent calls
            this.log("Preping the tabcmd admin session");
            this.create_tabcmd_admin_session();
            this.logger = null;
        }

        public Tabcmd(string tabcmd_folder, string tableau_server_url, string username, string password,
            string site, string tabcmd_config_location, TableauRepository TableauRepository)
        {
            this.tabcmd_folder = tabcmd_folder;
            this.username = username;
            this.password = password;
            this.site = site;
            this.tableau_server_url = tableau_server_url;
            this.tabcmd_config_filename = "tabcmd-session.xml";
            this.tabcmd_config_location = tabcmd_config_location;

            this.repository = TableauRepository;

            // This preps tabcmd for subsequent calls
            this.log("Preping the tabcmd admin session");
            this.create_tabcmd_admin_session();
            this.logger = null;
        }

        public Tabcmd(string tabcmd_folder, string tableau_server_url, string username, string password,
            string site, string tabcmd_config_location, TableauRepository TableauRepository, Logger logger)
        {
            this.tabcmd_folder = tabcmd_folder;
            this.username = username;
            this.password = password;
            this.site = site;
            this.tableau_server_url = tableau_server_url;

            this.tabcmd_config_filename = "tabcmd-session.xml";
            this.tabcmd_config_location = tabcmd_config_location;

            this.logger = logger;

            this.repository = TableauRepository;

            // This preps tabcmd for subsequent calls
            this.log(String.Format("Preping the tabcmd admin session for site {0}", this.site));
            this.create_tabcmd_admin_session();
            
        }

        public void log(string l)
        {
            if (this.logger != null)
            {
                this.logger.Log(l);
            }
        }

        // All the build_x_cmd methods are just for convenience of wrapping tabcmd
        public string build_directory_cmd()
        {
            return String.Format("cd {0}", this.tabcmd_folder);
        }
     
        public string build_login_cmd(string pw_filename)
        { 
            try
            {   // Open the text file for writing using File
                File.WriteAllText(pw_filename, this.password);
            
            }
            catch (Exception e)
            {
                Console.WriteLine("The password file could not be read:");
                Console.WriteLine(e.Message);
            }
            string cmd;
            if ( this.site.ToLower() == "default"){
                cmd = String.Format("tabcmd login -s {0} -u {1} --password-file \"{2}\"", this.tableau_server_url,
                    this.username, pw_filename);
            }
            else {
                 cmd = String.Format("tabcmd login -s {0} -t {1} -u {2} --password-file \"{3}\"", this.tableau_server_url,
                     this.site, this.username, pw_filename);
            }
            return cmd;
        }

        public string build_export_cmd(string export_type, string filename, string view_url, Dictionary<string, string> view_filter_dictionary,
            bool refresh)
        {
            string cmd;
            string[] allowable_export_types = new string[4] { "pdf", "csv", "png", "fullpdf" };

            if (view_url == "")
            {
                throw new ConfigurationException("No view_url was provided");
            }

            if (allowable_export_types.Contains(export_type.ToLower()) == false)
            {
                // Exception
            }
            string additional_url_params = "";
            if (view_filter_dictionary != null)
            {
                
               // WebUtility.HtmlEncode
                var first_param = 0;
                foreach (KeyValuePair<string, string> pair in view_filter_dictionary)
                {
                    if (first_param == 0){
                        additional_url_params += "?";
                        first_param++;
                    }
                    else {
                        additional_url_params += "&";
                    }
                    // Gotta double the % sign because batch files use %2 as a replacement token.
                    additional_url_params += Uri.EscapeUriString(pair.Key).Replace("%","%%") + "=" + (pair.Value);
                }

                if (refresh == true)
                {
                    additional_url_params += "&:refresh";
                }
            }
            else if (view_filter_dictionary == null)
            {
                if (refresh == true)
                {
                    additional_url_params += "?:refresh";
                }
            }
            view_url += additional_url_params;
            cmd = String.Format("tabcmd export \"{0}\" --filename \"{1}\" --{2}",
                view_url, filename, export_type);
            // Additional parameters for export options
            string extra_params = "--pagelayout {4} --pagesize {5} --width {6} --height {7}";
            return cmd;
        }

        // You need to log in to tabcmd successfully with admin privileges the first time
        private void create_tabcmd_admin_session(){
            this.log("Creating a tabcmd admin session");
            string pw_filename = this.tabcmd_folder + "sh3zoy2lya.txt";
            string[] cmds = new string[2];
            
            cmds[0] = this.build_directory_cmd();
            cmds[1] = this.build_login_cmd(pw_filename);
            try
            {
                File.WriteAllLines("login.bat", cmds);
            }
            catch(IOException)
            {
                throw new ConfigurationException("Could not write login.bat file, please restart and check all files");
            }
            var results = Cmd.Run("login.bat", true);
            // Check tabcmd results?

            this.log(results[0]);
            this.log(results[1]);

            // Clear admin password file after each run
            File.Delete(pw_filename);
            File.Delete("login.bat");
        }

        public string create_export(string export_type, string view_location,
            Dictionary<string, string> view_filter_dictionary, string user_to_impersonate,
            string filename)
        {

            if (view_location == "")
            {
                throw new ConfigurationException("view_location is not specified");
            }

            if (String.Equals(user_to_impersonate, "") == false)
            {
                this.create_session_and_configure_tabcmd_for_user(user_to_impersonate, view_location);
            }

            string[] cmds = new string[2];
            cmds[0] = this.build_directory_cmd();
            
            
            string saved_filename;
            if (String.Equals(export_type.ToLower(), "fullpdf"))
            {
                saved_filename = String.Format("{0}.{1}", filename, "pdf");
            }
            else
            {
                saved_filename = String.Format("{0}.{1}", filename, export_type);
            }

            //cmds[1] = String.Format("del \"{0}\"", saved_filename);
            cmds[1] = this.build_export_cmd(export_type, saved_filename, view_location, view_filter_dictionary, false);
            
            try
            {
                File.WriteAllLines("export.bat", cmds);
            }
            catch (IOException)
            {
                MessageBox.Show("Could not write export.bat file");
            }

            string full_file_location = this.tabcmd_folder + saved_filename;
            this.log(String.Format("Writing to file {0}", full_file_location));
            
            // Run the commands
            var results = Cmd.Run("export.bat", true);
            
            this.log(results[0]);
            this.log(results[1]);
            if (results[1].Contains("===== Saved"))
            {
                this.log("Export file generated correctly");
            }
            else
            {
                throw new ConfigurationException("Export command failed, most likely configuration issue.");
            }

            File.Delete("export.bat");
            return full_file_location;
        }

        /*
         * The essence of how this works is that you can use trusted tickets to create a session
         * then rewrite tabcmd's config file with that session info instead. Because tabcmd has
         * session continuation, you can keep running tabcmd without a signin action, but continually
         * switching to the correct user
         */

        /* 
         * tabcmd keeps a session history, stored within its XML configuration file.
         * Rather than logging into tabcmd again, once there is a session history, we simply substitute in the
         * impersonated user's info directly into the XML.
         */ 
        private void configure_tabcmd_config_for_user_session(string user)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.tabcmd_config_location + this.tabcmd_config_filename);

            XmlWriterSettings xwsSettings = new XmlWriterSettings();
            xwsSettings.Indent = true;
            xwsSettings.IndentChars = " ";

            XmlNode root = doc.DocumentElement;
           
            XmlNode uname = root.SelectSingleNode("username");
            uname.InnerText = user;
            
            XmlNode baseUrl = root.SelectSingleNode("base-url");
            baseUrl.InnerText = this.tableau_server_url;
            
            XmlNode sessionId = root.SelectSingleNode("session-id");
            sessionId.InnerText = this.user_session_id;
            
            XmlNode authToken = root.SelectSingleNode("authenticity-token");
            authToken.InnerText = this.user_auth_token;

            XmlNode sitePrefix = root.SelectSingleNode("site-prefix");
            if (this._site.ToLower() != "default")
            {
                sitePrefix.InnerText = String.Format("t/{0}", this._site);
            }
            else
            {
                sitePrefix.InnerText = null;
            }
            using (XmlWriter xwWriter = XmlWriter.Create(this.tabcmd_config_location + this.tabcmd_config_filename, xwsSettings))
            {
                doc.PreserveWhitespace = true;
                doc.Save(xwWriter);
            }
        }

        // By querying the sessions table, there is a JSON string which includes the auth token
        private void set_tabcmd_auth_info_from_repository_for_impersonation(string username_to_impersonate)
        {
            
            NpgsqlDataReader dr = this.repository.query_sessions(username_to_impersonate);
            if (dr.HasRows == true)
            {
                dr.Read();
                this.user_session_id = dr.GetString(0);
                string wg_json = dr.GetString(4);
                var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(wg_json), new System.Xml.XmlDictionaryReaderQuotas());
                var XmlDoc = new XmlDocument();
                XmlDoc.Load(jsonReader);
                XmlNode root = XmlDoc.DocumentElement;
                XmlNode auth_token = root.SelectSingleNode("auth_token");
                this.user_auth_token = auth_token.InnerText;
            }
            
            else
            {
                // Throw some kind of exception because you didn't find any sessions for that user
                // Something must have broken in the trusted tickets stuff
            }
            dr.Close();
        }

        private void create_session_and_configure_tabcmd_for_user(string user, string view_location)
        {
            TableauHTTP tabhttp = new TableauHTTP(this.tableau_server_url);
            tabhttp.logger = this.logger;
            if (tabhttp.create_trusted_ticket_session(view_location, user, this._site, ""))
            {
                this.set_tabcmd_auth_info_from_repository_for_impersonation(user);
                this.configure_tabcmd_config_for_user_session(user);
            }
            else
            {
                this.log("Trusted ticket session could not be established");
            }
  
        }

    }
}

// Taken from http://techvalleyprojects.blogspot.com/2012/04/c-using-command-prompt.html
public static class Cmd
{
    public static string[] Run(string command, bool output)
    {
        /*
         *  New array of two strings.
         *  string[0] is the error message.
         *  string[1] is the output message.
         */
        string[] message = new string[2];

        // ProcessStartInfo allows better control over
        // the soon to executed process
        ProcessStartInfo info = new ProcessStartInfo();

        // Input to the process is going to come from the Streamwriter
        info.RedirectStandardInput = true;

        // Output from the process is going to be put into message[1]
        info.RedirectStandardOutput = true;

        // Error, if any, from the process is going to be put into message[0]
        info.RedirectStandardError = true;

        // This must be set to false
        info.UseShellExecute = false;

        // We want to open the command line
        info.FileName = "cmd.exe";

        // We don't want to see a command line window
        info.CreateNoWindow = true;

        // Instantiate a Process object
        Process proc = new Process();

        // Set the Process object's start info to the above StartProcessInfo
        proc.StartInfo = info;

        // Start the process
        proc.Start();


        // The stream writer is replacing the keyboard as the input
        using (StreamWriter writer = proc.StandardInput)
        {
            // If the streamwriter is able to write
            if (writer.BaseStream.CanWrite)
            {
                // Write the command that was passed into the method
                writer.WriteLine(command);
                // Exit the command window
                writer.WriteLine("exit");
            }
            // close the StreamWriter
            writer.Close();
        }

        // Get any Error's that may exist
        message[0] = proc.StandardError.ReadToEnd();

        // If the output flag was set to true
        if (output)
        {
            // Get the output from the command line
            message[1] = proc.StandardOutput.ReadToEnd();
        }

        // close the process
        proc.Close();

        // return the any error/output
        return message;
    }

    public static string[] Run(string[] command, bool output)
    {
        string[] message = new string[2];

        ProcessStartInfo info = new ProcessStartInfo();

        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;

        info.UseShellExecute = false;
        info.FileName = "cmd.exe";
        info.CreateNoWindow = true;

        Process proc = new Process();
        proc.StartInfo = info;
        proc.Start();

        using (StreamWriter writer = proc.StandardInput)
        {
            if (writer.BaseStream.CanWrite)
            {
                foreach (string q in command)
                {
                    writer.WriteLine(q);
                }
                writer.WriteLine("exit");
            }
        }

        message[0] = proc.StandardError.ReadToEnd();

        if (output)
        {
            message[1] = proc.StandardOutput.ReadToEnd();
        }

        // close the process
        proc.Close();

        return message;
    }

}

