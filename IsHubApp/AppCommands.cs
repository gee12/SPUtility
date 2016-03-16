using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IsHubApp
{
    public class AppCommands
    {
        public static RoutedUICommand ShowPortSettingsFormCommand { get; protected set; }
        public static RoutedUICommand OpenPortCommand { get; protected set; }
        public static RoutedUICommand ClosePortCommand { get; protected set; }
        public static RoutedUICommand AboutCommand { get; protected set; }
        public static RoutedUICommand StartPollCommand { get; protected set; }
        public static RoutedUICommand StopPollCommand { get; protected set; }
        public static RoutedUICommand StartFlowCommand { get; protected set; }
        public static RoutedUICommand StopFlowCommand { get; protected set; }
        public static RoutedUICommand PollTimerCommand { get; protected set; }
        public static RoutedUICommand InitCommand { get; protected set; }
        public static RoutedUICommand DoseCommand { get; protected set; }
        public static RoutedUICommand SaveParametersCommand { get; protected set; }
        
        static AppCommands()
        {
            ShowPortSettingsFormCommand = new RoutedUICommand("", "ShowSettingsFormCommand", typeof(AppCommands));
            OpenPortCommand = new RoutedUICommand("", "OpenPortCommand", typeof(AppCommands));
            ClosePortCommand = new RoutedUICommand("", "ClosePortCommand", typeof(AppCommands));
            AboutCommand = new RoutedUICommand("", "AboutCommand", typeof(AppCommands));
            StartPollCommand = new RoutedUICommand("", "StartPollCommand", typeof(AppCommands));
            StopPollCommand = new RoutedUICommand("", "StopPollCommand", typeof(AppCommands));
            StartFlowCommand = new RoutedUICommand("", "StartFlowCommand", typeof(AppCommands));
            StopFlowCommand = new RoutedUICommand("", "StopFlowCommand", typeof(AppCommands));
            PollTimerCommand = new RoutedUICommand("", "PollTimeoutCommand", typeof(AppCommands));
            InitCommand = new RoutedUICommand("", "InitCommand", typeof(AppCommands));
            DoseCommand = new RoutedUICommand("", "DoseCommand", typeof(AppCommands));
            SaveParametersCommand = new RoutedUICommand();
        }

    }
}
