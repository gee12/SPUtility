using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IsHubApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string ASSEMBLY_NAME = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        //public static readonly string SQLBACKUPPER_CONFIG_FILE_NAME = "IsHub.exe.cfg";
        public static readonly string SQLBACKUPPER_CONFIG_FILE_NAME = System.AppDomain.CurrentDomain.FriendlyName + ".cfg";
        public static readonly string LOG_PATH = ".";

        public static IsHubConfig CurrentConfig;

        /// <summary>
        /// 
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bool res = AppInit();

            if (!res)
            {
                string s = string.Format("Не удается создать конфигурационный файл [{0}]\nЗапуск программы невозможен", SQLBACKUPPER_CONFIG_FILE_NAME);
                MessageBox.Show(s);
                return;
            }
            AppRun();
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool AppInit()
        {
            CurrentConfig = new IsHubConfig(ASSEMBLY_NAME, SQLBACKUPPER_CONFIG_FILE_NAME, LOG_PATH);
            if (!CurrentConfig.IsSetConfig)
            {
                return (CurrentConfig.WriteConfig());
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void AppRun()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

    }
}
