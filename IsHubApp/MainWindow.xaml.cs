using IsHubLib;
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
        Gilbarco isHub { get; set; }
        Timer autoPollTimer;
        //bool isPollStarted = false;
        int curPistol;
        bool IsSaveParameters { get { return cbIsSaveParameters.IsChecked; } set { cbIsSaveParameters.IsChecked = value; } }

        //bool IsAutoPoll { get { return cbAutoPoll.IsChecked.GetValueOrDefault(); } set { cbAutoPoll.IsChecked = value; } }
        bool isAutoPoll;
        bool IsAutoPoll { get { return isAutoPoll; } set { isAutoPoll = value; tbPollTimeout.IsEnabled = !value; } }

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            //Log.Init(ASSEMBLY_NAME, LOG_PATH, false, WriteLog);
            Log.LogHandler = WriteLog;

            this.isHub = new Gilbarco(Gilbarco.GilbarcoDoseAlgorithms.Algorithm1, DataSent, DataReceived);

            autoPollTimer = new Timer();
            autoPollTimer.Elapsed += PollTimerElapsed;
            tbPollTimeout.PreviewTextInput += SPLib.Utils.OnUnsignedIntPreviewTextInput;

            gIndicators.DataContext = isHub;

            InitFromConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitFromConfig()
        {
            IsHubConfig config = App.CurrentConfig;
            IsSaveParameters = config.IsSaveParameters;

            if (IsSaveParameters)
            {
                var port = this.isHub.ComPort.Port;
                port.PortName = config.PortName;
                port.BaudRate = config.BaudRate;
                port.DataBits = config.DataBits;
                port.StopBits = config.StopBits;
                port.Parity = config.Parity;
                port.ReadTimeout = config.ReadTimeout;
                port.WriteTimeout = config.WriteTimeout;

                tbPollTimeout.Value = config.TimerMsec;

                var pistolRB = GetRadioButtonFromTag(gPistolRadioButtons.Children, config.Pistol.ToString());
                if (pistolRB != null) pistolRB.IsChecked = true;
                this.curPistol = config.Pistol;

                var algRB = GetRadioButtonFromTag(gAlgRadioButtons.Children, config.DoseAlgorithm.ToString());
                if (algRB != null) algRB.IsChecked = true;
                this.isHub.DoseAlgorithm = Gilbarco.GilbarcoDoseAlgorithms.FromValue<Gilbarco.GilbarcoDoseAlgorithms>(config.DoseAlgorithm);
            }
        }

        static RadioButton GetRadioButtonFromTag(UIElementCollection collection, string tag)
        {
            foreach (var child in collection)
            {
                RadioButton rb = child as RadioButton;
                if (rb != null && rb.Tag.ToString() == tag)
                {
                    return rb;
                }
            }
            return null;
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
            //else if (AppCommands.InitCommand.Equals(command))
            //{
            //    Init();
            //}
            // poll
            else if (AppCommands.StartPollCommand.Equals(command))
            {
                StartPoll();
            }
            //else if (AppCommands.PollTimerCommand.Equals(command))
            //{
            //    PollTimer();
            //}
            else if (AppCommands.StopPollCommand.Equals(command))
            {
                StopPoll();
            }
            // flow
            else if (AppCommands.StartFlowCommand.Equals(command))
            {
                StartFlow();
            }
            else if (AppCommands.DoseCommand.Equals(command))
            {
                DefineDose();
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
            if (this.isHub == null) return;

            var command = e.Command; 
            // port
            if (AppCommands.ShowPortSettingsFormCommand.Equals(command))
            {
                e.CanExecute = !this.isHub.ComPort.IsPortOpened;
            } 
            else if (AppCommands.OpenPortCommand.Equals(command))
            {
                e.CanExecute = !this.isHub.ComPort.IsPortOpened;
            }
            else if (AppCommands.ClosePortCommand.Equals(command))
            {
                e.CanExecute = this.isHub.ComPort.IsPortOpened;
            }
            // init
            //else if (AppCommands.InitCommand.Equals(command))
            //{
            //    e.CanExecute = this.isHub.ComPort.IsPortOpened && !this.isHub.IsOnline && this.isHub.IsFree;
            //}
            else if (AppCommands.StopPollCommand.Equals(command))
            {
                e.CanExecute = IsAutoPoll;
            }
            //////////// IsOnline
            //else if (!this.isHub.IsOnline || !this.isHub.IsFree)
            //else if (!this.isHub.IsFree)
            //{
            //    e.CanExecute = false;
            //}
            // poll
            //else if (AppCommands.StartPollCommand.Equals(command))
            //{
            //    //e.CanExecute = !this.isHub.IsFlowAllowed;
            //    e.CanExecute = true;
            //}
            //else if (AppCommands.PollTimerCommand.Equals(command))
            else if (AppCommands.StartPollCommand.Equals(command))
            {
                //if (!cbAutoPoll.IsChecked.Value)
                if (this.isHub.ComPort.IsPortOpened && !IsAutoPoll)
                {
                    int res;
                    bool parsed = Int32.TryParse(tbPollTimeout.Value.ToString(), out res);
                    e.CanExecute = parsed && res >= tbPollTimeout.Minimum;
                }
                //else e.CanExecute = false;
            }
            else if (!this.isHub.IsOnline)
            {
                e.CanExecute = false;
            }
            // flow
            else if (AppCommands.StartFlowCommand.Equals(command))
            {
                e.CanExecute = !this.isHub.IsFlowAllowed && IsAutoPoll && isHub.IsOnePistolOnSide();
            }
            else if (AppCommands.DoseCommand.Equals(command))
            {
                e.CanExecute = this.isHub.IsFlowAllowed;
            }
            else if (AppCommands.StopFlowCommand.Equals(command))
            {
                e.CanExecute = this.isHub.IsFlowAllowed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ShowPortSettings()
        {
            var form = new SPSettingsForm();
            form.Owner = this;
            form.IsShowAvailablePorts = App.CurrentConfig.IsShowAvailablePorts;
            if (form.ShowModal(this.isHub.ComPort.Port))
            {
                //this.port = form.SerialPort;
                this.isHub.ComPort.SetPortSettings(form.SP);
                Log.Add("Параметры COM-порта изменены");

                App.CurrentConfig.IsShowAvailablePorts = form.IsShowAvailablePorts;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OpenPort()
        {
            Log.Add("Открытие порта: " + isHub.ComPort.ToString());
            isHub.Open();
            Log.Add("Результат: " + isHub.ComPort.IsPortOpened);
        }

        /// <summary>
        /// 
        /// </summary>
        void ClosePort()
        {
            IsAutoPoll = false;

            Log.Add("Закрытие порта: " + isHub.ComPort.Port.PortName);
            isHub.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        //void StartPoll()
        //{
        //    Log.Add("Простой опрос вручную");
        //    this.isHub.SendPoll();
        //}
        
        /// <summary>
        /// 
        /// </summary>
        void StartPoll()
        {
            //StartInit();

            ////System.Threading.Thread.Sleep(1000);

            //if (this.isHub.IsOnline)
            //{
            //    StartAutoPoll();
            //}

            StartAutoPoll();
        }

        /// <summary>
        /// 
        /// </summary>
        void StartInit()
        {
            Log.Add("Инициализация");
            this.isHub.SendInit();
        }

        void StartAutoPoll()
        {
            IsAutoPoll = true;

            isHub.CurrentMode = Gilbarco.Modes.INIT;

            this.isHub.SendInit();

            tbLogs.Clear();

            Log.Add("Порт: " + isHub.ComPort.ToString());
            Log.Add("Запуск цикла опроса");
            //bool isNeedTimer = cbAutoPoll.IsChecked.GetValueOrDefault();
            //if (IsAutoPoll)
            //{
            int value = Int32.Parse(tbPollTimeout.Value.ToString());
            autoPollTimer.Interval = value;
            autoPollTimer.Start();
            //}
            //else
            //{
            //    autoPollTimer.Stop();
            //}
            //tbPollTimeout.IsEnabled = !IsAutoPoll;
        }

        //bool isFlowMode = false;
        /// <summary>
        /// 
        /// </summary>
        void PollTimerElapsed(object sender, ElapsedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            );
            //
            if (!this.isHub.ComPort.IsPortOpened)// || !this.isHub.IsOnline)
            {
                autoPollTimer.Stop();
                Log.Add("Остановка цикла опроса");
                return;
            }

            //
            //if (this.isHub.IsFree)
            //{
            //    // запускаем только простой опрос
            //    if (!this.isHub.IsFlowAllowed)
            //    {
            //        this.isHub.SendPoll();
            //    }
            //    else
            //    {
            //        // запускаем простой опрос и опрос дозы по очереди
            //        if (isFlowMode)
            //        {
            //            this.isHub.SendDose();
            //        }
            //        else
            //        {
            //            this.isHub.SendPoll();
            //        }
            //        isFlowMode = !isFlowMode;
            //    }
            //}
            //this.isHub.SendPoll();
            //this.isHub.SendStartFlow(curPistol);
            isHub.OnTimerTick();
        }


        void StopPoll()
        {
            IsAutoPoll = false;
            //isHub.IsOnline = false;

            Log.Add("Остановка цикла опроса");
            autoPollTimer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        void StartFlow()
        {
            Log.Add("Запуск отпуска");
            //this.isHub.SendStartFlow(curPistol);

            isHub.CurrentMode = Gilbarco.Modes.START_FLOW;
        }

        /// <summary>
        /// 
        /// </summary>
        void DefineDose()
        {
            Log.Add("Определить отпуск вручную");
            this.isHub.SendDose();
        }

        /// <summary>
        /// 
        /// </summary>
        void StopFlow()
        {
            Log.Add("Остановка отпуска");
            this.isHub.SendStopFlow();
        }

        /// <summary>
        /// 
        /// </summary>
        void DataSent(byte[] bytes)
        {
            if (bytes == null) return;

            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var s = Utils.ToHexString(bytes);
                    Log.Add("Отправлено: " + s);
                }));
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        void DataReceived(byte[] bytes)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (bytes == null)
                    {
                        Log.Add("Нет ответа...");
                    }
                    else
                    {
                        var s = Utils.ToHexString(bytes);
                        Log.Add("Принято: " + s);
                    }
                }));
            }
            catch { }
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
            string s = string.Format("Is-Hub Utility, версия {0}\n2016 г.", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            MessageBox.Show(this, s, "О программе", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            this.curPistol = Int32.Parse(rb.Tag.ToString());
        }

        private void AlgRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            int algNum = Int32.Parse(rb.Tag.ToString());
            this.isHub.DoseAlgorithm = Gilbarco.GilbarcoDoseAlgorithms.FromValue<Gilbarco.GilbarcoDoseAlgorithms>(algNum);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (autoPollTimer.Enabled)
                autoPollTimer.Stop();

            IsHubConfig config = App.CurrentConfig;
            config.IsSaveParameters = IsSaveParameters;

            if (IsSaveParameters)
            {
                var port = this.isHub.ComPort.Port;
                config.PortName = port.PortName;
                config.BaudRate = port.BaudRate;
                config.DataBits = port.DataBits;
                config.StopBits = port.StopBits;
                config.Parity = port.Parity;
                config.ReadTimeout = port.ReadTimeout;
                config.WriteTimeout = port.WriteTimeout;

                config.TimerMsec = tbPollTimeout.Value.GetValueOrDefault();
                config.Pistol = this.curPistol;
                config.DoseAlgorithm = this.isHub.DoseAlgorithm.Value;
            }

            config.WriteConfig();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ByteToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte b = (byte)value;
            return Utils.ToHexString(b);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
