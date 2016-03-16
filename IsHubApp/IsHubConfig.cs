using SPLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace IsHubApp
{
    public class IsHubConfig : SPLib.Config
    {
        public const string IS_SAVE_PARAMETERS_TAG = "IsSaveParameters";
        public const string TIMER_MSEC_TAG = "TimerMsec";
        public const string DOSE_ALG_TAG = "DoseAlgorithm";
        public const string PISTOL_TAG = "Pistol";

        public const bool DEF_IS_SAVE_PARAMETERS = true;
        public const int DEF_TIMER_MSEC = 100;
        public const int DEF_ALG = 0;
        public const int DEF_PISTOL = 0;

        bool isSaveParameters = DEF_IS_SAVE_PARAMETERS;
        int timerMsec = DEF_TIMER_MSEC;
        int alg = DEF_ALG;
        int pistol = DEF_PISTOL;

        public bool IsSaveParameters { get { return isSaveParameters; } set { isSaveParameters = value; } }
        public int TimerMsec { get { return timerMsec; } set { timerMsec = value; } }
        public int DoseAlgorithm { get { return alg; } set { alg = value; } }
        public int Pistol { get { return pistol; } set { pistol = value; } }

        /// <summary>
        /// 
        /// </summary>
        public IsHubConfig(string assemblyName)
            : base(assemblyName, false)
        {
            Init();
        }

        public IsHubConfig(string assemblyName, string configFileFullName, string logFilePath)
            : base(assemblyName, configFileFullName, logFilePath, false)
        {
            //Init(configFileName);
            Init();
        }

        void Init()
        {
            // Нельзя устанавливать, т.к. идет перезапить считываемого значения
            //IsSaveParameters = DEF_IS_SAVE_PARAMETERS;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool ReadConfig(string fileName)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(fileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // base
                        if (reader.Name.Equals(CONFIG_ELEMENT_TAG))
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                string name = reader.Name;
                                string value = reader.Value;
                                switch (name)
                                {
                                    case IS_SAVE_PARAMETERS_TAG: IsSaveParameters = bool.Parse(value); break;
                                    case TIMER_MSEC_TAG: TimerMsec = int.Parse(value); break;
                                    case DOSE_ALG_TAG: DoseAlgorithm = int.Parse(value); break;
                                    case PISTOL_TAG: Pistol = int.Parse(value); break;
                                }
                                //
                                ReadBaseAttributes(name, value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WriteConfig()
        {
            return WriteConfig(this.ConfigFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool WriteConfig(string fileName)
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(fileName, Encoding.Unicode);
                writer.Formatting = Formatting.Indented;

                writer.WriteStartElement("Config");

                // base
                WriteBaseAttributes(writer);
                writer.WriteAttributeString(IS_SAVE_PARAMETERS_TAG, IsSaveParameters.ToString());
                writer.WriteAttributeString(TIMER_MSEC_TAG, TimerMsec.ToString());
                writer.WriteAttributeString(DOSE_ALG_TAG, DoseAlgorithm.ToString());
                writer.WriteAttributeString(PISTOL_TAG, Pistol.ToString());

                writer.Flush();
                writer.Close();
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
