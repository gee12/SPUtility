using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPLib
{
    public class AppCommands
    {
        public static RoutedUICommand TestCommand { get; protected set; }
        public static RoutedUICommand ApplyCommand { get; protected set; }
        public static RoutedUICommand CancelCommand { get; protected set; }
        public static RoutedUICommand AvailablePortsCommand { get; protected set; }
        

        static AppCommands()
        {
            TestCommand = new RoutedUICommand("", "TestCommand", typeof(AppCommands));
            ApplyCommand = new RoutedUICommand("", "ApplyCommand", typeof(AppCommands));
            CancelCommand = new RoutedUICommand("", "CancelCommand", typeof(AppCommands));
            AvailablePortsCommand = new RoutedUICommand();
        }

        public static void SetCommandBinding(CommandBindingCollection commandCollection, ICommand command, 
            ExecutedRoutedEventHandler executed, CanExecuteRoutedEventHandler canExecute = null)
        {
            var binding = (canExecute == null)
                ? new CommandBinding(command, executed)
                : new CommandBinding(command, executed, canExecute);
            commandCollection.Remove(binding);
            commandCollection.Add(binding);
        }

        public static void SetInputBinding(InputBindingCollection inputCollection, ICommand command, InputGesture gesture)
        {
            var binding = new InputBinding(command, gesture);
            inputCollection.Remove(binding);
            inputCollection.Add(binding);
        }
    }
}
