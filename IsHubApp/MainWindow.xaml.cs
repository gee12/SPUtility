using SPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IsHubApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly string ASSEMBLY_NAME = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string LOG_PATH = ".";

        SPConnection comPort;
        IsHub isHub;
        Timer autoPollTimer;
        bool isPollStarted = false;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            Log.Init(ASSEMBLY_NAME, LOG_PATH, false, WriteLog);

            this.comPort = new SPConnection();
            this.comPort.DataReceivingHandler += DataReceived;
            this.isHub = new IsHub();

            autoPollTimer = new Timer();
            autoPollTimer.Elapsed += PollTimerElapsed;
            tbPollTimer.ValueChanged += tbInterval_ValueChanged;
            tbPollTimer.PreviewTextInput += SPLib.Utils.OnUnsignedIntPreviewTextInput;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.AboutCommand.Equals(command))
            {
                ShowAboutForm();
            }
            // port
            else if (AppCommands.ShowPortSettingsFormCommand.Equals(command))
            {
                ShowPortSettings();
            }
            else if (AppCommands.OpenPortCommand.Equals(command))
            {
                OpenPort();
            }
            else if (AppCommands.ClosePortCommand.Equals(command))
            {
                ClosePort();
            }
            // init connect
            else if (AppCommands.InitCommand.Equals(command))
            {
                Init();
            }
            // poll
            else if (AppCommands.StartPollCommand.Equals(command))
            {
                StartPoll();
            }
            else if (AppCommands.PollTimerCommand.Equals(command))
            {
                PollTimer();
            }
            // flow
            else if (AppCommands.StartFlowCommand.Equals(command))
            {
                StartFlow();
            }
            else if (AppCommands.StopFlowCommand.Equals(command))
            {
                StopFlow();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.OpenPortCommand.Equals(command))
            {
                e.CanExecute = !this.comPort.IsPortOpened;
            }
            else if (AppCommands.ClosePortCommand.Equals(command))
            {
                e.CanExecute = this.comPort.IsPortOpened;
            }
           
            if (AppCommands.InitCommand.Equals(command))
            {
                e.CanExecute = this.comPort.IsPortOpened && !this.isHub.IsOnline;
            }
            
            //
            if (!this.isHub.IsOnline)
            {
                e.CanExecute = false;
                return;
            }

            else if (AppCommands.StartPollCommand.Equals(command))
            {
                e.CanExecute = !this.isHub.IsFlowAllowed;
            }
            else if (AppCommands.StopFlowCommand.Equals(command))
            {
                e.CanExecute = this.isHub.IsFlowAllowed;
            }
            else if (AppCommands.PollTimerCommand.Equals(command))
            {
                if (!this.isHub.IsOnline)
                {
                    e.CanExecute = false;
                }
                else if (!cbAutoPoll.IsChecked.Value)
                {
                    int res;
                    bool parsed = Int32.TryParse(tbPollTimer.Value.ToString(), out res);
                    e.CanExecute = parsed && res >= tbPollTimer.Minimum;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ShowPortSettings()
        {
            var form = new SPSettingsForm();
            form.Owner = this;
            if (form.ShowModal(this.comPort.Port))
            {
                //this.port = form.SerialPort;
                this.comPort.SetPortSettings(form.SerialPort);
                Log.Add("Параметры COM-порта успешно изменены");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OpenPort()
        {
            Log.Add("Открытие порта: " + comPort.Port.PortName);
            comPort.OpenPort();
            Log.Add("Результат: " + comPort.IsPortOpened);
        }

        /// <summary>
        /// 
        /// </summary>
        void ClosePort()
        {
            Log.Add("Закрытие порта: " + comPort.Port.PortName);
            comPort.ClosePort();
        }

        /// <summary>
        /// 
        /// </summary>
        void DataReceived(byte[] bytes)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                var s = Utils.ToHexString(bytes);
                Log.Add("Принято: " + s);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        void Init()
        {
            var bytes = this.isHub.SendInit();
            var s = Utils.ToHexString(bytes);
            Log.Add("Инициализация: " + s);
        }

        /// <summary>
        /// 
        /// </summary>
        void StartPoll()
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        void PollTimer()
        {
            bool isNeedTimer = cbAutoPoll.IsChecked.GetValueOrDefault();
            if (isNeedTimer)
            {
                int value = Int32.Parse(tbInterval.Value.ToString());
                autoPollTimer.Interval = value;
                autoPollTimer.Start();
            }
            else
            {
                autoPollTimer.Stop();
            }
            tbPollTimer.IsEnabled = !isNeedTimer;
        }

        void PollTimerElapsed(object sender, ElapsedEventArgs e)
        {
            StartPoll();
        }

        //void tbInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    int res;
        //    bool parsed = Int32.TryParse(tbPollTimer.Value.ToString(), out res);
        //    bSetInterval.IsEnabled = parsed && res >= tbPollTimer.Minimum;
        //}

        /// <summary>
        /// 
        /// </summary>
        void StartFlow()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        void StopFlow()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        void WriteLog(string message)
        {
            tbLogs.AppendText("> " + message + Environment.NewLine);
            tbLogs.ScrollToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Log.Add("Exit");
            Log.AddLine();
        }

        void ShowAboutForm()
        {

        }
    }
}
