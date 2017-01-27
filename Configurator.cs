using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;


namespace Behold_Emailer
{
    class Configurator
    {
        public static void EncryptConfig()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            //ConfigurationSection c = config.AppSettings;
            ConfigurationSection c = config.GetSection("userSettings/Behold_Emailer.Properties.Settings");
            c.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            c.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            
            //defaultSection.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Full);
        }

        public static string GetConfig(string setting)
        {
            return (string) Properties.Settings.Default[setting];
            
        }

        public static StringCollection GetConfigCollection(string setting)
        {
            return (StringCollection)Properties.Settings.Default[setting];
        }

        public static SerializableStringDictionary GetConfigSerializableStringDict(string setting)
        {
            return (SerializableStringDictionary)Properties.Settings.Default[setting];
        }

        public static void SetConfig(string setting, string val)
        {
            Properties.Settings.Default[setting] = val;
            
        }

        public static void SetConfig(string setting, StringCollection vals)
        {
            Properties.Settings.Default[setting] = vals;
        }

        public static void SetConfig(string setting, SerializableStringDictionary vals)
        {
            Properties.Settings.Default[setting] = vals;
        }


        public static void SaveConfig()
        {
            Configurator.EncryptConfig();
            Properties.Settings.Default.Save();
        }

     
    }

    // From http://stackoverflow.com/questions/922047/store-dictionarystring-string-in-application-settings , https://weblogs.asp.net/pwelter34/444961
    public class SerializableStringDictionary : StringDictionary, IXmlSerializable
    {
        public SerializableStringDictionary()
        {

        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read() &&
                !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName == this.GetType().Name))
            {
                var name = reader["Name"];
                if (name == null)
                    throw new FormatException();

                var value = reader["Value"];
                this[name] = value;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (DictionaryEntry entry in this)
            {
                writer.WriteStartElement("Pair");
                writer.WriteAttributeString("Name", (string)entry.Key);
                writer.WriteAttributeString("Value", (string)entry.Value);
                writer.WriteEndElement();
            }
        }
    }


}
