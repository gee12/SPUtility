using SPLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly string ASSEMBLY_NAME = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string LOG_PATH = ".";

        //SerialPort port;
        SPConnection spConnection;
        //bool isPortOpened { get { return (port != null && port.IsOpen); }}
        //delegate void DataReceivedDeleg(byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;

            Log.Init(ASSEMBLY_NAME, LOG_PATH, false, WriteLog);


            //this.port = new SerialPort();
            //this.port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
            this.spConnection = new SPConnection();

            this.spConnection.DataReceivingHandler += DataReceived;
        }

        //void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    //Thread.Sleep(500);
        //    //string data = this.port.ReadExisting();
        //    int length = this.port.BytesToRead;
        //    byte[] bytes = new byte[length];
        //    //int r = this.port.Read(bytes, 0, length);
        //    //string data = Encoding.Default.GetString(bytes);
        //    //tbData.Dispatcher.Invoke(new DataReceivedDeleg(DataReceived), new object[] { data });
        //    tbData.Dispatcher.Invoke(new DataReceivedDeleg(DataReceived), bytes);
        //}

        /// <summary>
        /// 
        /// </summary>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command;
            if (AppCommands.ShowPortSettingsFormCommand.Equals(command))
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
            else if (AppCommands.AddDataCommand.Equals(command))
            {
                AddData();
            }
            else if (AppCommands.SendCommand.Equals(command))
            {
                SendData();
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
                e.CanExecute = !this.spConnection.IsPortOpened;
            }
            else if (AppCommands.ClosePortCommand.Equals(command))
            {
                e.CanExecute = this.spConnection.IsPortOpened;
            }
            else if (AppCommands.AddDataCommand.Equals(command))
            {
                e.CanExecute = !string.IsNullOrEmpty(tbData.Text);
            }
            else if (AppCommands.SendCommand.Equals(command))
            {
                e.CanExecute = this.spConnection != null && this.spConnection.IsPortOpened && !string.IsNullOrEmpty(tbBuffer.Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ShowPortSettings()
        {
            var form = new SPSettingsForm();
            form.Owner = this;
            if (form.ShowModal(this.spConnection.Port))
            {
                //this.port = form.SerialPort;
                this.spConnection.SetPortSettings(form.SerialPort);
                Log.Add("Параметры COM-порта успешно изменены");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OpenPort()
        {
            Log.Add("Открытие порта: " + spConnection.Port.PortName);
            spConnection.OpenPort();
            Log.Add("Результат: " + spConnection.IsPortOpened);
        }

        /// <summary>
        /// 
        /// </summary>
        void ClosePort()
        {
            Log.Add("Закрытие порта: " + spConnection.Port.PortName);
            spConnection.ClosePort();
        }

        /// <summary>
        /// 
        /// </summary>
        void AddData()
        {
            AddData(tbData.Text);
        }

        void AddData(string data)
        {
            byte[] bytes = null;
            if (rbText.IsChecked.GetValueOrDefault())
            {
                //this.port.Write(data);
                bytes = Encoding.Default.GetBytes(data);
                //Log.Add("Добавлено в буфер: " + data);
            }
            else if (rbHex.IsChecked.GetValueOrDefault())
            {
                try
                {
                    bytes = Utils.ToByteArrayFromHex(data, " ");
                }
                catch
                {
                    MessageBoxes.Warning(this, "Ошибка формата данных");
                    return;
                }

            }
            //var s = string.Join(" ", bytes);
            var s = Utils.ToHexString(bytes);
            Log.Add("Добавлено в буфер: " + data + " Внутреннее представление: " + s + ")");
            tbBuffer.AppendText(data + Environment.NewLine);

            this.spConnection.AddData(bytes);
            tbData.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        void DataReceived(byte[] bytes)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    //int length = bytes.Length;
                    //int r = this.port.Read(bytes, 0, length);
                    string data = "";
                    if (rbText.IsChecked.GetValueOrDefault())
                        data = Encoding.Default.GetString(bytes);
                    else if (rbHex.IsChecked.GetValueOrDefault())
                        data = Utils.ToHexString(bytes);
                    //var s = string.Join(" ", bytes);
                    var s = Utils.ToHexString(bytes);
                    Log.Add("Принято: " + data.Trim() + " (Внутреннее представление: " + s + ")");
                }));
        }

        void SendData()
        {
            if (rbText.IsChecked.GetValueOrDefault())
                this.spConnection.SendMode = SPConnection.SendModes.Text;
            else if (rbHex.IsChecked.GetValueOrDefault())
                this.spConnection.SendMode = SPConnection.SendModes.HexDecimal;
            var bytes = this.spConnection.SendData();
            //var s = string.Join(" ", bytes);
            var s = Utils.ToHexString(bytes);
            Log.Add("Отправлено: " + s);
            tbBuffer.Clear();
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

        /// <summary>
        /// 
        /// </summary>
        //class OpenPortTask : Utils.AsyncTask<MainWindow, SerialPort, bool>
        //{
        //    protected override void pre(MainWindow form)
        //    {
        //        form.pbCircular.Visibility = Visibility.Visible;
        //    }

        //    protected override bool run(SerialPort port)
        //    {
        //        try
        //        {
        //            port.Open();
        //        }
        //        catch {}
        //        return port.IsOpen;
        //    }

        //    protected override void post(MainWindow form, bool res)
        //    {
        //        form.pbCircular.Visibility = Visibility.Collapsed;
        //    }
        //}
    }
}
