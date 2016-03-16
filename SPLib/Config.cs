using System;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.IO.Ports;

namespace SPLib
{
    public abstract class Config
    {
        public static readonly string ASSEMBLY_NAME = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string DEF_CONFIG_PATH = System.AppDomain.CurrentDomain.FriendlyName + ".cfg";
        public static readonly string DEF_LOG_PATH = ".";

        public const string CONFIG_ELEMENT_TAG = "Config";

        public const string PORT_NAME_TAG = "PortName";
        public const string BAUD_RATE_TAG = "BaudRate";
        public const string DATA_BITS_TAG = "DataBits";
        public const string PARITY_TAG = "Parity";
        public const string STOP_BITS_TAG = "StopBits";
        public const string READ_TIMEOUT_TAG = "ReadTimeout";
        public const string WRITE_TIMEOUT_TAG = "WriteTimeout";
        public const string IS_SHOW_AVAILABLE_PORTS_TAG = "IsShowAvailablePorts";

        public const bool DEF_IS_SHOW_AVAILABLE_PORTS = true;

        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        public bool IsShowAvailablePorts { get; set; }

        public bool IsSetConfig = false;
        public string ConfigFilePath = DEF_CONFIG_PATH;
        public string LogFilePath = DEF_LOG_PATH;

        /// <summary>
        /// 
        /// </summary>
        public Config(bool splitLog = true)
        {
            Init(ASSEMBLY_NAME, DEF_CONFIG_PATH, DEF_LOG_PATH, splitLog);
        }

        public Config(string assemblyName, bool splitLog = true)
        {
            Init(assemblyName, DEF_CONFIG_PATH, DEF_LOG_PATH, splitLog);
        }

        public Config(string assemblyName, string configFileFullName, string logFilePath, bool splitLog = true)
        {
            Init(assemblyName, configFileFullName, logFilePath, splitLog);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Init(string assemblyName, string configFileFullName, string logPath, bool splitLog)
        {
            IsShowAvailablePorts = DEF_IS_SHOW_AVAILABLE_PORTS;

            // init log
            Log.Init(assemblyName, logPath, splitLog);

            if (!SetConfig(configFileFullName))
            {
                // set default config file
                SetConfig(DEF_CONFIG_PATH);
            }

            Log.Add("Config file [{0}] set successfully", configFileFullName);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool SetConfig(string configFileFullName)
        {
            IsSetConfig = false;
            this.ConfigFilePath = configFileFullName;

            if (new FileInfo(configFileFullName).Exists)
            {
                if (ReadConfig(configFileFullName))
                {
                    IsSetConfig = true;
                }
                else Log.Add("Error with config file [{0}]", configFileFullName);
            }
            else
            {
                Log.Add("Config file [{0}] is missing..!", configFileFullName);
            }
            return IsSetConfig;
        }

        public abstract bool ReadConfig(string configFileName);

        /// <summary>
        /// 
        /// </summary>
        public void ReadBaseElements(XmlTextReader reader)
        {
            if (reader == null) return;

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name.Equals(CONFIG_ELEMENT_TAG))
                {
                    while (reader.MoveToNextAttribute())
                    {
                        ReadBaseAttributes(reader.Name, reader.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReadBaseAttributes(string name, string value)
        {
            switch (name)
            {
                case PORT_NAME_TAG: PortName = value; break;
                case BAUD_RATE_TAG: BaudRate = Int32.Parse(value); break;
                case DATA_BITS_TAG: DataBits = Int32.Parse(value); break;
                case PARITY_TAG: Parity = (Parity)Enum.Parse(typeof(Parity), value); break;
                case STOP_BITS_TAG: StopBits = (StopBits)Enum.Parse(typeof(StopBits), value); break;
                case READ_TIMEOUT_TAG: ReadTimeout = Int32.Parse(value); break;
                case WRITE_TIMEOUT_TAG: WriteTimeout = Int32.Parse(value); break;
                case IS_SHOW_AVAILABLE_PORTS_TAG: IsShowAvailablePorts = bool.Parse(value); break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WriteBaseAttributes(XmlTextWriter writer)
        {
            if (writer == null) return false;

            try
            {
                writer.WriteAttributeString(PORT_NAME_TAG, PortName);
                writer.WriteAttributeString(BAUD_RATE_TAG, BaudRate.ToString());
                writer.WriteAttributeString(DATA_BITS_TAG, DataBits.ToString());
                writer.WriteAttributeString(PARITY_TAG, Parity.ToString());
                writer.WriteAttributeString(STOP_BITS_TAG, StopBits.ToString());
                writer.WriteAttributeString(READ_TIMEOUT_TAG, ReadTimeout.ToString());
                writer.WriteAttributeString(WRITE_TIMEOUT_TAG, WriteTimeout.ToString());
                writer.WriteAttributeString(IS_SHOW_AVAILABLE_PORTS_TAG, IsShowAvailablePorts.ToString());
                
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message);
                return false;
            }
            return true;
        }
    }
}
