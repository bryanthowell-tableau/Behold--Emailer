To use Behold! Emailer, you follow these steps:

   1) Input configurations for all necessary Tableau Server technologies. Behold! Emailer takes advantage of Trusted Tickets, the "readonly" repository user, and tabcmd
   2) Input configuration for your SMTP email server
   3) Set the Tableau Server e-mail schedules you want to listen on (more explanation on this further down)
   4) Configure any "watermarking" you'd like on the PDF exports - headers, footers, page number and logos

Behold! Emailer can be installed on any machine, but the Tableau Server must be set to Trust that machine for Trusted Tickets (https://onlinehelp.tableau.com/current/server/en-us/trusted_auth_trustIP.htm ), and it must have tabcmd installed.


Configuration


The Configure tab contains all of the major settings you'll need to set for Behold! Emailer to work. We'll use the middle-out algorithm to go through the settings

Tableau Server

The first things you need to input are your Tableau Server URL (including http:// or https://), a Tableau Server Administrator Username and that user's Password. This is necessary to establish the first tabcmd session so that all of the PDF exporting will work smoothly.
Next, enter the password for the "readonly" user of the Tableau Server repository. If you've never set the password for this user before, the instructions are at https://onlinehelp.tableau.com/current/server/en-us/perf_collect_server_repo.htm. We need this repository user for access to the scheduling information among other things. As the "readonly" user, there is no danger to your repository and no ability to write or change anything.
Next, browse for the location of your tabcmd executable file. If you are installing Tableau Emailer on your Tableau Server, this will be Program Files\Tableau\Tableau Server\{version}\bin\, but you can also install tabcmd separately on a different machine using the installer at Program Files\Tableau\Tableau Server\{version}\extras\TabcmdInstaller-x64.exe, and in that case the location will be where ever you installed it.
Lastly, browse for the location of your tabcmd configuration files. You may need to run tabcmd at least once from the command line for the configuration folder to exist; a simple tabcmd login command will suffice to generate it. The file should be located in C:\Users\{user}\AppData\Local\Tableau\Tabcmd\ . The file is called tabcmd-session.xml, if you see that, you've found the right folder.
Now that you have all of those things, press the Save Configuration button.


Email Server Configuration

All you need for this portion to work is a standard SMTP server on the network. Enter in the server name or IP address first. Email Sender Address is the name that will show up in the From: field of the e-mail. This can be a "noreply@yourcompany.com" type address or something that indicates it comes from Tableau.
The message body of the e-mail will be based on the templates files you select. The HTML version will show in most e-mail applications, but you should have a plain text version as well for a backup.

An HTML template can be as plain as:

[code language='html']
<html>
<head></head>

<body>

<p>Here is your e-mail
</body></html>

[/code]


Currently there is no type of substitution of phrases within these templates, but that might be a version 2 type feature.
Save the Configuration!

Watermarking

The Watermarking configuration allows you to add additional text or images onto the PDF exported from tabcmd. There are currently three types:

    Text: Allows a message, including a timestamp in UTC
    Page Number: Numbers each page
    Image: An image that is shorter than 40px. (The viz itself starts at 50px from the top of the page and ends 50px before the bottom of the page, so anything larger would overlap the exported viz)


Testing the Configuration

The Test tab allows you generate a single file or e-mail based on all of the settings you have configured. Put in the Site ("default" if it is default), the View location (this is the part of a Tableau Server URL after views/ but before the ?) and the Username you want the view run as (this helps you test Row Level Security)
You can choose either to send a Test E-mail or to save a Test File. There is a Location (Folder/Directory) selector for the Test file, then give it a name (without the .PDF extension).


Scheduling E-mails

All of the testing features are great, but the real power of Behold! Emailer is that it can read Disabled Schedules from a Tableau Server. This means that your end users can still select them from the Subscribe drop-down in their Server view, but it's actually being generated from Behold! Emailer instead of the Tableau Server Backgrounder process.
In the Schedules menu, you may want to hit the "Refresh Schedules List" button; this will requery the Repository to get the latest list of Disabled Schedules. Click the ones you want to monitor until they are checked, then press the Save Configuration button. You also may want to set a folder under the Export File Archive Folder, so that anything generated is duplicated for auditing purposes. Once you have all of that, press the Start Scheduled Emails button.
This starts a monitor which only fires off on the :01, the :16, the :31 and the :46 minute of the hour. Since the Tableau Server subscription options are 15 minute increments at the smallest, this will capture any next subscription to send. I don't recommend allowing PDF exports on a 15 minutes schedule, because there is more effort involved, particularly if it generates a long list, but it is possible.