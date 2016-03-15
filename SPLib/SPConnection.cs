using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace SPLib
{
    public class SPConnection
    {
        public static string[] Ports;
        //{
        //    "COM1",
        //    "COM2",
        //    "COM3",
        //    "COM4",
        //    "COM5",
        //    "COM6",
        //    "COM7",
        //    "COM8",
        //};

        public static int[] BaudRates = 
        {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            14400,
            19200,
            28800,
            36000,
            115000
        };

        public static int[] DataBits =
        {
            7, 
            8, 
            9
        };


        public enum SendModes
        {
            Text,
            HexDecimal
        }

        SerialPort port;
        public SerialPort Port { get { return port; } }
        public bool IsPortOpened { get { return (port != null && port.IsOpen); } }
        List<byte[]> buffer;
        public List<byte[]> Buffer { get { return Buffer; } }
        SendModes sendMode;
        public SendModes SendMode { get { return sendMode; } set { sendMode = value; } }

        public Action<byte[]> DataReceivingHandler;

        public SPConnection()
        {
            this.port = new SerialPort();
            this.buffer = new List<byte[]>();

            this.port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);

            Ports = new string[50];
            for (int i = 0; i < 50; i++)
            {
                Ports[i] = "COM" + (i + 1).ToString();
            }
        }

        void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //try
            //{
            //    //Thread.Sleep(500);
            //    //string data = this.port.ReadExisting();
            //    int length = this.port.BytesToRead;
            //    byte[] bytes = new byte[length];
            //    //int r = this.port.Read(bytes, 0, length);
            //    //string data = Encoding.Default.GetString(bytes);
            //    //tbData.Dispatcher.Invoke(new DataReceivedDeleg(DataReceived), new object[] { data });
            //    //tbData.Dispatcher.Invoke(new DataReceivedDeleg(ReceiveData), bytes);
            //    this.port.Read(bytes, 0, length);
            //    //ReceiveData(bytes);
            //    if (DataReceivingHandler != null)
            //        DataReceivingHandler.Invoke(bytes);
            //}
            //catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenPort()
        {
            //Log.Add("Открытие порта: " + port.PortName);
            //new OpenPortTask().Start(this, this.Dispatcher, this.port);
            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message);
            }
            //Log.Add("Результат: " + IsPortOpened);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePort()
        {
            //Log.Add("Закрытие порта: " + port.PortName);
            port.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        //bool AddData(string data)
        //{
        //    byte[] bytes = null;
        //    if (sendMode == SendModes.Text)
        //    {
        //        bytes = Encoding.Default.GetBytes(data);
        //    }
        //    else if (sendMode == SendModes.HexDecimal)
        //    {
        //        try
        //        {
        //            bytes = Utils.ToByteHexArray(data, " ");
        //        }
        //        catch
        //        {
        //            //MessageBoxes.Warning(this, "Ошибка формата данных");
        //            return false;
        //        }

        //    }
        //    buffer.Add(bytes);
        //    var s = string.Join(" ", bytes);
        //    Log.Add("Добавлено в буфер: " + s);
        //    return true;
        //}

        public void AddData(byte[] bytes)
        {
            buffer.Add(bytes);
            //var s = string.Join(" ", bytes);
            //Log.Add("Добавлено в буфер: " + s);
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] SendData()
        {
            byte[] bytes = new byte[this.buffer.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in this.buffer)
            {
                System.Buffer.BlockCopy(array, 0, bytes, offset, array.Length);
                offset += array.Length;
            }

            SendData(bytes);
            ClearBuffer();

            return bytes;
        }

        public void SendData(byte[] bytes)
        {
            try
            {
                this.port.Write(bytes, 0, bytes.Length);
            }
            catch {}
        }

        void ClearBuffer()
        {
            this.buffer.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ReadData()
        {
            byte[] bytes = null;
            byte[] actualBytes = null;
            int length = this.port.ReadBufferSize;
            int actualLength = 0;

            try
            {
                do
                {
                    // read
                    bytes = new byte[length];
                    actualLength = this.port.Read(bytes, 0, bytes.Length);

                    if (actualBytes == null)
                    {
                        // first bytes array
                        actualBytes = new byte[actualLength];
                        System.Buffer.BlockCopy(bytes, 0, actualBytes, 0, actualLength);
                    }
                    else
                    {
                        // join arrays
                        byte[] temp = new byte[actualLength + actualBytes.Length];
                        System.Buffer.BlockCopy(actualBytes, 0, temp, 0, actualBytes.Length);
                        System.Buffer.BlockCopy(bytes, 0, temp, actualBytes.Length, actualLength);
                        actualBytes = temp;
                    }
                }
                while (this.port.BytesToRead > 0);
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            return actualBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        //private void ReceiveData(byte[] bytes)
        //{
        //    int length = bytes.Length;
        //    int r = this.port.Read(bytes, 0, length);
        //    string data = "";
        //    if (rbText.IsChecked.GetValueOrDefault())
        //        data = Encoding.Default.GetString(bytes);
        //    else if (rbHex.IsChecked.GetValueOrDefault())
        //        data = Utils.ToString(bytes);
        //    Log.Add("Принято: " + data.Trim());
        //}

        public static SerialPort ClonePort(SerialPort src)
        {
            var dest = new SerialPort();
            dest.PortName = src.PortName;
            dest.BaudRate = src.BaudRate;
            dest.DataBits = src.DataBits;
            dest.StopBits = src.StopBits;
            dest.Parity = src.Parity;
            return dest;
        }

        public void SetPortSettings(SerialPort src)
        {
            port.PortName = src.PortName;
            port.BaudRate = src.BaudRate;
            port.DataBits = src.DataBits;
            port.StopBits = src.StopBits;
            port.Parity = src.Parity;
        }

        public string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                port.PortName, port.BaudRate, port.DataBits, port.StopBits, port.Parity, port.ReadTimeout, port.WriteTimeout);
        }
    }
}
