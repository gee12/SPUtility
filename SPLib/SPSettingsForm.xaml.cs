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
        public SerialPort SP { get; private set; }

        public string[] AllPorts;
        public bool IsShowAvailablePorts { get { return cbIsAvailablePorts.IsChecked.GetValueOrDefault(); } set { cbIsAvailablePorts.IsChecked = value; PortsList(); } }

        /// <summary>
        /// 
        /// </summary>
        public SPSettingsForm()
        {
            InitializeComponent();

            //ItemsSource = Enum.GetValues(typeof(EffectStyle)).Cast<EffectStyle>();
            //
            this.AllPorts = new string[50];
            for (int i = 0; i < 50; i++)
            {
                this.AllPorts[i] = "COM" + (i + 1).ToString();
            }
            IsShowAvailablePorts = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.ApplyCommand.Equals(command))
            {
                Apply();
            }
            else if (AppCommands.CancelCommand.Equals(command))
            {
                Close(false);
            }
            else if (AppCommands.TestCommand.Equals(command))
            {
                Test();
            }
            else if (AppCommands.AvailablePortsCommand.Equals(command))
            {
                PortsList();
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
                SP = new SerialPort();
                SP.PortName = this.AllPorts[0];
                SP.BaudRate = SPConnection.BaudRates[0];
                SP.DataBits = SPConnection.DataBits[0];
                //SerialPort.StopBits = StopBits.None;
                SP.Parity = Parity.None;

                SP.ReadTimeout = SerialPort.InfiniteTimeout;
                SP.WriteTimeout = SerialPort.InfiniteTimeout;
            }
            else SP = port;
            gMain.DataContext = SP;
            //cbPort.DataContext = this;
        }

        void Apply()
        {
            Close(true);
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
            SerialPort test = SPConnection.ClonePort(this.SP);
            new TestTask().Start(this, this.Dispatcher, test);
        }

        void PortsList()
        {
            string[] ports = null;
            if (IsShowAvailablePorts)
            {
                // HKEY_LOCAL_MACHINE\HARDWARE\DEVICEMAP\SERIALCOMM
                ports = SerialPort.GetPortNames();
            }
            else
            {
                ports = this.AllPorts;
            }
            cbPort.ItemsSource = ports;
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
