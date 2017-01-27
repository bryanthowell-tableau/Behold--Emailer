using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Diagnostics;

namespace Behold_Emailer
{
    /*
     * TableauRepository represents a connection to the PostgreSQL repository in Tableau Server
     * This class is just a convenience which builds in a connection and useful queries and processing
     * You could make the thing yourself if you needed to 
     */
    class TableauRepository
    {
        string[] repository_users = new string[3] { "tableau", "readonly", "tblwgadmin" };
        int repository_port;
        string repository_db;
        string repository_user;
        string repository_pw;
        string repository_server;
        NpgsqlConnection repository;
        public Logger logger;

        public TableauRepository(string tableau_server_url, string repository_password, string repository_username) 
        {
            if (String.Equals(repository_username, "")) { this.repository_user = "readonly"; }
            else
            {
                this.repository_user = repository_username;
            }
            this.repository_port = 8060;
            this.repository_db = "workgroup";
            this.repository_pw = repository_password;
            this.logger = null;
            // Don't use user "tableau", you need at least "readonly" right
            // Only need tblwgadmin if you need to write into the repository, only for advanced hack cases

            // Remove the http:// or https:// to log in to the repository. (Do we need things if this is SSL?)
            int colon_slash_slash = tableau_server_url.IndexOf("://");
            if (colon_slash_slash != -1){
                this.repository_server = tableau_server_url.Substring(colon_slash_slash+3);
            }
            else{
                this.repository_server = tableau_server_url;
            }
            // Take off any extra stuff after the server main (including port number extensions)
            int final_colon = this.repository_server.IndexOf(":");
            if (final_colon != -1)
            {
                this.repository_server = this.repository_server.Substring(0, final_colon);
            }
            int extra_slash = this.repository_server.IndexOf("/");
            if (extra_slash != -1)
            {
                this.repository_server = this.repository_server.Substring(0, extra_slash);
            }

            this.repository = new NpgsqlConnection(String.Format("Host={0};Username={1};Password={2};Database={3};Port={4};Pooling=false", this.repository_server, 
                this.repository_user, this.repository_pw, this.repository_db, this.repository_port));
            try
            {
                this.log(String.Format("Opening connection to repository on {0}", this.repository_server));
                this.repository.Open();
            }
            catch (NpgsqlException)
            {
                throw new ConfigurationException("Cannot connect to Repository, please check credentials");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Timeout"))
                {
                    this.log("Issue with PG timeout. Going to keep on trucking");
                }
                else
                {
                    throw e;
                }
            }

        }

        // Destructor to free up resource
        ~TableauRepository()
        {
            this.repository.Close();
        }

        public void log(string l)
        {
            if (this.logger != null)
            {
                this.logger.Log(l);
            }
        }

        public NpgsqlDataReader query_sessions(string username)
        {
        // Trusted tickets sessions do not have anything in the 'data' column
        //The auth token is contained within the shared_wg_write column, stored as JSON
            string sessions_sql = @"
            SELECT
            sessions.session_id,
            sessions.data,
            sessions.updated_at,
            sessions.user_id,
            sessions.shared_wg_write,
            sessions.shared_vizql_write,
            system_users.name AS user_name,
            users.system_user_id
            FROM sessions,
            system_users,
            users
            WHERE sessions.user_id = users.id AND users.system_user_id = system_users.id
        ";
            if (username != "")
            {
                sessions_sql += "AND system_users.name = @uname\n";
            }
            sessions_sql += "ORDER BY sessions.updated_at DESC;";

            NpgsqlCommand cmd = new NpgsqlCommand(sessions_sql, this.repository);
            cmd.Parameters.AddWithValue("@uname", username);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            
            return dr;

        }

        public NpgsqlDataReader query_subscriptions_for_users(string schedule_name, Boolean views_only)
        {
            string subscriptions_sql = @"
                SELECT
                s.id,
                s.subject,
                s.user_name,
                s.site_name,
                COALESCE(cv.repository_url, s.view_url) as view_url,
                sch.name,
                su.email
                FROM _subscriptions s
                LEFT JOIN _customized_views cv  ON s.customized_view_id = cv.id
                JOIN _schedules sch ON sch.name = s.schedule_name
                JOIN system_users su ON su.name = s.user_name
            ";

            if (schedule_name != "")
            {
                subscriptions_sql += "WHERE sch.name = @sched_name\n";

                if (views_only == true)
                {
                    subscriptions_sql += "AND s.view_url IS NOT NULL -- Export command in tabcmd requires a View not a Workbook";
                }
            }
            else
            {
                if (views_only == true)
                {
                    subscriptions_sql += "WHERE s.view_url IS NOT NULL -- Export command in tabcmd requires a View not a Workbook";
                }
            }

            NpgsqlCommand cmd = new NpgsqlCommand(subscriptions_sql, this.repository);
            if (schedule_name != "")
            {
                cmd.Parameters.AddWithValue("@sched_name", schedule_name);
            }

            NpgsqlDataReader dr = cmd.ExecuteReader();
            return dr;

        }

        public NpgsqlDataReader query_inactive_subscription_schedules()
        {
            string sub_sched_sql = @"                
                SELECT * 
                FROM _schedules sch 
                WHERE scheduled_action_type = 'Subscriptions' 
                AND active=false
            ";

            NpgsqlCommand cmd = new NpgsqlCommand(sub_sched_sql, this.repository);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        public NpgsqlDataReader query_inactive_subscription_schedules_for_next_run_time()
        {
            string sub_sched_sql = @"                
                SELECT 
			        name,
			        run_next_at
                FROM _schedules sch 
                WHERE scheduled_action_type = 'Subscriptions' AND active=false
            ";

            NpgsqlCommand cmd = new NpgsqlCommand(sub_sched_sql, this.repository);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
    }
}
