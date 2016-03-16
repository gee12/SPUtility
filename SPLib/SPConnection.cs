using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace SPLib
{
    public class SPConnection
    {
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

        public Action<byte[]> DataReceivingHandler = null;

        /// <summary>
        /// 
        /// </summary>
        public SPConnection()
        {
            this.port = new SerialPort();
            this.buffer = new List<byte[]>();

            this.port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
            
        }

        /// <summary>
        /// 
        /// </summary>
        void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (DataReceivingHandler == null) return;

            try
            {
                //Thread.Sleep(500);
                int length = this.port.BytesToRead;
                byte[] bytes = new byte[length];
                this.port.Read(bytes, 0, length);
                //if (DataReceivingHandler != null)
                DataReceivingHandler.Invoke(bytes);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenPort()
        {
            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                Log.Add(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePort()
        {
            port.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddData(byte[] bytes)
        {
            buffer.Add(bytes);
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

        /// <summary>
        /// 
        /// </summary>
        public void SetPortSettings(SerialPort src)
        {
            port.PortName = src.PortName;
            port.BaudRate = src.BaudRate;
            port.DataBits = src.DataBits;
            port.StopBits = src.StopBits;
            port.Parity = src.Parity;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                port.PortName, port.BaudRate, port.DataBits, port.StopBits, port.Parity, port.ReadTimeout, port.WriteTimeout);
        }
    }
}
