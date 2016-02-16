using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPLib
{
    /// <summary>
    /// Interaction logic for SPSettingsForm.xaml
    /// </summary>
    public partial class SPSettingsForm : Window
    {
        public SerialPort SerialPort { get; private set; }

        public SPSettingsForm()
        {
            InitializeComponent();

            SerialPort = new SerialPort();
            SerialPort.PortName = Utils.Ports[0];
            SerialPort.BaudRate = Utils.BaudRates[0];
            SerialPort.DataBits = Utils.DataBits[0];
            //SerialPort.StopBits = StopBits.None;
            SerialPort.Parity = Parity.None;

            gMain.DataContext = SerialPort;
            //ItemsSource = Enum.GetValues(typeof(EffectStyle)).Cast<EffectStyle>();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.TestCommand.Equals(command))
            {
                Test();
            }
        }

        void Test()
        {
            new TestTask().Start(this, this.Dispatcher, "");
        }

        /// <summary>
        /// 
        /// </summary>
        class TestTask : Utils.AsyncTask<SPSettingsForm, String, String>
        {
            protected override void pre(SPSettingsForm form)
            {
                form.pbCircular.Visibility = Visibility.Visible;
            }

            protected override string run(string param)
            {
                return "";
            }

            protected override void post(SPSettingsForm form, string res)
            {
                form.pbCircular.Visibility = Visibility.Collapsed;
            }
        }
    }
}
