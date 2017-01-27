using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
namespace Behold_Emailer
{
    class TableauHTTP
    {
        string tableau_server_url;
        public Logger logger;

        public TableauHTTP(string tableau_server_url)
        {
            this.tableau_server_url = tableau_server_url;
            this.logger = null;
        }

        public void log(string l)
        {
            if (this.logger != null)
            {
                this.logger.Log(l);
            }
        }

        public string get_trusted_ticket_for_user(string username, string site, string ip)
        {
            if (site == ""){ site = "default";}
            this.log(String.Format("Requesting trusted ticket for {0} on site {1}", username, site));
            
            string trusted_url = this.tableau_server_url + "/trusted";
            Debug.WriteLine(trusted_url);
            WebClient client = new WebClient();
            
            byte[] response;
            try
            {
                if (site == "default")
                {
                    response = client.UploadValues(trusted_url, new System.Collections.Specialized.NameValueCollection() 
                        {
                            { "username", username }
                        }
                   );
                }
                else
                {
                    response = client.UploadValues(trusted_url, new System.Collections.Specialized.NameValueCollection() 
                        {
                            { "username", username },
                            { "target_site", site }
                        }
                    );
                }

                string result = System.Text.Encoding.UTF8.GetString(response);
                if (result == "-1")
                {
                    // If you don't get -1, you should have a trusted ticket, raise an exception
                    string error = String.Format("Trusted ticket for {0} on site {1} from server {2} returned -1, some error occurred", username, site, this.tableau_server_url);
                    this.log(error);
                    throw new ConfigurationException(error);

                }
                // If misconfigured, the Tableau Server returns a redirect page
                else if (result.Contains("html") == true)
                {
                    // If you don't get -1, you should have a trusted ticket, raise an exception
                    string error = String.Format("Trusted ticket for {0} on site {1} from server {2} returned the redirect page, some error occurred", username, site, this.tableau_server_url);
                    this.log(error);
                    throw new ConfigurationException(error);
                }

                this.log(String.Format("Trusted ticket for {0} on site {1} from server {2} returned {3}", username, site, this.tableau_server_url, result));
                return result;
            }
            catch (WebException)
            {   
                throw new ConfigurationException("Trusted tickets not working, check configuration of Tableau Server and the configuration program");
            }
            
        }

        public bool redeem_trusted_ticket(string view_to_redeem, string trusted_ticket, string site){
            if (site == "" || site.ToLower() == "default")
            {
                site = "default";
            }
            string trusted_view_url = String.Format("{0}/trusted/{1}", this.tableau_server_url, trusted_ticket);
            if (site.ToLower() != "default")
            {
                trusted_view_url += String.Format("/t/{0}/views/{1}", site, view_to_redeem);
            }
            else
            {
                trusted_view_url += String.Format("/views/{0}", view_to_redeem);
            }

            WebClient client = new WebClient();
            try
            {
                this.log(String.Format("Redeeming trusted ticket via {0}", trusted_view_url));
                byte[] response = client.DownloadData(trusted_view_url);
                this.log(String.Format("Trusted ticket redeemed succesfully"));
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var status_code = ((HttpWebResponse)ex.Response).StatusCode;
                    var status_description = ((HttpWebResponse)ex.Response).StatusDescription;
                    this.log(String.Format("Trusted ticket redemption failed with Status Code {0} and Description {1}", status_code, status_description));

                } 
                return false;
            }

        }

        public bool create_trusted_ticket_session(string view_to_redeem, string username, string site, string ip)
        {
            string ticket = this.get_trusted_ticket_for_user(username, site, ip);
            this.log(String.Format("Trusted ticket returned {0}", ticket));
            bool result = this.redeem_trusted_ticket(view_to_redeem, ticket, site);
            return result;
        }
    }
}
