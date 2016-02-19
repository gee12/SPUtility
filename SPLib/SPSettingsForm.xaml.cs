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
            //ItemsSource = Enum.GetValues(typeof(EffectStyle)).Cast<EffectStyle>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.ApplyCommand.Equals(command))
            {
                Close(true);
            }
            else if (AppCommands.CancelCommand.Equals(command))
            {
                Close(false);
            }
            else if (AppCommands.TestCommand.Equals(command))
            {
                Test();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowModal(SerialPort port = null)
        {
            Init(port);

           return base.ShowDialog().GetValueOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init(SerialPort port)
        {
            if (port == null)
            {
                SerialPort = new SerialPort();
                SerialPort.PortName = SPConnection.Ports[0];
                SerialPort.BaudRate = SPConnection.BaudRates[0];
                SerialPort.DataBits = SPConnection.DataBits[0];
                //SerialPort.StopBits = StopBits.None;
                SerialPort.Parity = Parity.None;
            }
            else SerialPort = port;
            gMain.DataContext = SerialPort;
        }

        void Close(bool result)
        {
            this.DialogResult = result;
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        void Test()
        {
            SerialPort test = SPConnection.ClonePort(this.SerialPort);
            new TestTask().Start(this, this.Dispatcher, test);
        }

        /// <summary>
        /// 
        /// </summary>
        class TestTask : Utils.AsyncTask<SPSettingsForm, SerialPort, bool>
        {
            protected override void pre(SPSettingsForm form)
            {
                form.pbCircular.Visibility = Visibility.Visible;
            }

            protected override bool run(SerialPort port)
            {
                bool res = false;
                try
                {
                    port.Open();
                    res = port.IsOpen;
                    port.Close();
                }
                catch {}
                return res;
            }

            protected override void post(SPSettingsForm form, bool res)
            {
                form.pbCircular.Visibility = Visibility.Collapsed;
                MessageBoxes.SerialPortTest(res);
            }
        }
    }
}
