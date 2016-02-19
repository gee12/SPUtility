using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TestApp
{
    public class AppCommands
    {
        public static RoutedUICommand ShowPortSettingsFormCommand { get; protected set; }
        public static RoutedUICommand OpenPortCommand { get; protected set; }
        public static RoutedUICommand ClosePortCommand { get; protected set; }
        public static RoutedUICommand AddDataCommand { get; protected set; }
        public static RoutedUICommand SendCommand { get; protected set; }
        
        static AppCommands()
        {
            ShowPortSettingsFormCommand = new RoutedUICommand("", "ShowSettingsFormCommand", typeof(AppCommands));
            OpenPortCommand = new RoutedUICommand("", "OpenPortCommand", typeof(AppCommands));
            ClosePortCommand = new RoutedUICommand("", "ClosePortCommand", typeof(AppCommands));
            AddDataCommand = new RoutedUICommand("", "AddDataCommand", typeof(AppCommands));
            SendCommand = new RoutedUICommand("", "SendCommand", typeof(AppCommands));
        }

    }
}
