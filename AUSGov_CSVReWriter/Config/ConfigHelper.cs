using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace AUSGov_CSVReWriter.Config
{
    [Serializable, XmlRoot("Config")]
    public class ConfigHelper
    {
        [XmlAttribute(AttributeName = "FirstRowIsHeader")]
        public bool FirstRowIsHeader { get; set; }

        [XmlAttribute(AttributeName = "InputFileName")]
        public string InputFileName { get; set; }

        [XmlAttribute(AttributeName = "OutputFileName")]
        public string OutputFileName { get; set; }

        [XmlAttribute(AttributeName = "DelimitOnWhiteSpace")]
        public bool DelimitOnWhiteSpace { get; set; }

        [XmlAttribute(AttributeName = "DelimitCharacter")]
        public string DelimitCharacter { get; set; }

        [XmlAttribute(AttributeName = "WhiteSpaceExpression")]
        public string WhiteSpaceExpression { get; set; }


        public List<AUSGov_CSVReWriter.Config.Column> ColumnDefinitions { get; set; }


        public static bool ConfigFileLoadSuccessful = false;
        public static string ConfigFilenameLoadedFrom = null;


        public static ConfigHelper LoadConfig(string cliSwitchArgConfigFilename)
        {
            ConfigHelper resultConfigHelper = null;

            // If cliSwitchArgConfigFilename filename was passed, it takes presednet, Try Loading Config.xml from this path ONLY
            if (cliSwitchArgConfigFilename != null)
            {
                resultConfigHelper = TryLoadConfig(cliSwitchArgConfigFilename);
                if (resultConfigHelper != null)
                    // If loaded a config.xml file return it
                    return resultConfigHelper;
                else
                {
                    Environment.Exit(-1);
                    return null;
                }
            }
            else
            {
                // Try from assembly run location
                resultConfigHelper = TryLoadConfig(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\config.xml");
                if (resultConfigHelper != null)
                    // If loaded a config.xml file return it
                    return resultConfigHelper;
                else
                {
                    // Try from parent of assembly run location (..\config.xml next folder down)
                    resultConfigHelper = TryLoadConfig(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\..\\config.xml");
                    if (resultConfigHelper != null)
                        // If loaded a config.xml file return it
                        return resultConfigHelper;
                    else
                    {
                        Console.WriteLine($"LoadConfig: Cannot find a config file to load, Exiting!");
                        Environment.Exit(-1);
                        return null;
                    }
                }
            }
        }
        private static ConfigHelper TryLoadConfig(string configFilename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigHelper));

            try
            {
                // Cannot use LoggerFactory to log here as it uses ConfigHelper to read config file;

                Console.WriteLine($"LoadConfig: Trying to load config file {configFilename}");

                FileStream myFileStream = new FileStream(configFilename, FileMode.Open, FileAccess.Read);
                ConfigHelper PoCo = (ConfigHelper)serializer.Deserialize(myFileStream);
                myFileStream.Close();

                if (PoCo != null)
                {
                    ConfigFileLoadSuccessful = true;
                    ConfigFilenameLoadedFrom = configFilename;
                }
                return (PoCo);
            }
            catch (Exception ex)
            {
                // If we cant read the config file, dont know where to log file, below causes stack overflow loop
                // LIS_Library.Logging.Logger.Instance.Log(ex.Message, LIS_Library.Logging.Logger.MessageType.Exception, true);
                // This is a critical error, Abort Not logged!
                // Can be casues in Debug if no command line switch is used to point to a 'common' config file location
                // Add command line in Projects Propertes when debuging -c "D:/DATA/My Projects/2018/LIS/Config.xml"
                Console.WriteLine($"LoadConfig: Exception: Could not load config file {ex.Message} \n {ex.InnerException}");

                return null;
            }
        }
    }

   



}
