using SPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IsHubLib
{
    public class IsHub : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract class DoseAlgorithms : Enumeration
        {
            public DoseAlgorithms() : base() { }

            public DoseAlgorithms(int value, string name)
                : base(value, name)
            {
            }

            public abstract int Define(byte[] buf);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public enum Modes
        //{
        //    NONE,
        //    INIT,
        //    POLL,
        //    //STOP_POLL,
        //    START_FLOW,
        //    STOP_FLOW1,
        //    STOP_FLOW2,
        //    DOSE
        //}

        /// <summary>
        /// 
        /// </summary>
        public virtual byte[] INIT_COMMAND { get { return null; } }
        public virtual byte[] POLL_COMMAND { get { return null; } }
        public virtual byte[] START_FLOW_COMMAND { get { return null; } }
        public virtual byte[] STOP_FLOW_COMMAND { get { return null; } }
        public virtual byte[] DOSE_COMMAND { get { return null; } }

        public virtual byte[] INIT_ANSWER { get { return null; } }
        public virtual byte[] POLL_ANSWER { get { return null; } }
        public virtual byte[] START_FLOW_ANSWER { get { return null; } }
        public virtual byte[] DOSE_ANSWER { get { return null; } }
        public virtual byte[] STOP_FLOW_ANSWER { get { return null; } }

        public const int DEF_READ_TIMEOUT = SerialPort.InfiniteTimeout;
        public const int DEF_WRITE_TIMEOUT = SerialPort.InfiniteTimeout;

        /// <summary>
        /// Properties
        /// </summary>
        public bool IsOnline { get { return isOnline && ComPort.IsPortOpened; } protected set { isOnline = value; NotifyPropertyChanged("IsOnline"); } }
        public bool IsFree { get { return isFree; } protected set { isFree = value; NotifyPropertyChanged("IsFree"); } }
        public byte Side1Info { get { return side1Info; } protected set { side1Info = value; NotifyPropertyChanged("Side1Info"); } }
        public byte Side2Info { get { return side2Info; } protected set { side2Info = value; NotifyPropertyChanged("Side2Info"); } }
        public bool IsFlowAllowed { get { return isFlowAllowed; } protected set { isFlowAllowed = value; NotifyPropertyChanged("IsFlowAllowed"); } }
        public int Pistols { get { return pistols; } protected set { pistols = value; NotifyPropertyChanged("Pistols"); } }
        public int CurrentDose { get { return currentDose; } protected set { currentDose = value; NotifyPropertyChanged("CurrentDose"); } }

        /// <summary>
        /// Fields
        /// </summary>
        protected bool isOnline;
        protected bool isFree;
        protected byte side1Info;
        protected byte side2Info;
        protected bool isFlowAllowed;
        protected int pistols;
        protected int currentDose;

        protected int CurPistol;
        //protected Modes curMode = Modes.NONE;

        public SPConnection ComPort { get; protected set; }
        public DoseAlgorithms DoseAlgorithm;
        public Action<byte[]> DataReceivedHandler;
        public Action<byte[]> DataSentHandler;

        /// <summary>
        /// 
        /// </summary>
        public IsHub()
        {
            Init();
        }

        public IsHub(int pistols, DoseAlgorithms alg, Action<byte[]> sentHandler, Action<byte[]> receivedHandler)
        {
            this.pistols = pistols;
            this.DoseAlgorithm = alg;
            this.DataReceivedHandler = receivedHandler;
            this.DataSentHandler = sentHandler;

            Init();
        }

        private void Init()
        {
            this.ComPort = new SPConnection();
            this.ComPort.Port.ReadTimeout = DEF_READ_TIMEOUT;
            this.ComPort.Port.WriteTimeout = DEF_WRITE_TIMEOUT;

            this.isFree = true;
        }

        public void Open()
        {
            this.ComPort.OpenPort();
        }

        public void Close()
        {
            this.ComPort.ClosePort();
            this.IsOnline = false;
            this.IsFree = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected byte[] DataCommunication(byte[] dataToSend, Action<byte[]> onDataReceived)
        {
            byte[] receivedData = null;
            new Thread(() =>
            {
                //receivedData = _DataCommunication(dataToSend, onDataReceived);
                this.IsFree = false;

                // send
                this.ComPort.SendData(dataToSend);
                if (DataSentHandler != null)
                    DataSentHandler.Invoke(dataToSend);

                //Thread.Sleep(1000);

                // receive
                receivedData = this.ComPort.ReadData();

                this.IsFree = true;

                if (DataReceivedHandler != null)
                    DataReceivedHandler.Invoke(receivedData);

                // handle data receiving
                if (onDataReceived != null)
                    onDataReceived.Invoke(receivedData);
            }).Start();

            //Func<byte[], Action<byte[]>, byte[]> func = _DataCommunication;
            //func.BeginInvoke(dataToSend, onDataReceived, null, null);

            return receivedData;
        }

        /// <summary>
        /// Init
        /// </summary>
        public virtual void SendInit()
        {
            //this.curMode = Modes.INIT;
            DataCommunication(INIT_COMMAND, ReceiveInit);
            //ReceiveInit(bytes);
        }

        protected virtual void ReceiveInit(byte[] bytes)
        {
            this.IsOnline = (bytes != null);
        }

        /// <summary>
        /// Poll
        /// </summary>
        public virtual void SendPoll()
        {
            //this.curMode = Modes.POLL;
            DataCommunication(POLL_COMMAND, ReceivePoll);
            //ReceivePoll(bytes);
        }

        protected virtual void ReceivePoll(byte[] bytes)
        {
        }

        /// <summary>
        /// Start flow
        /// </summary>
        public virtual void SendStartFlow(int pistol)
        {
            if (pistol < 0 || pistol > pistols) return;
            //this.curMode = Modes.START_FLOW;
            DataCommunication(START_FLOW_COMMAND, ReceivePoll);
            //ReceivePoll(bytes);
        }

        protected virtual void ReceiveStartFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
            {
                //IsFlowAllowed = false;
            }
        }

        /// <summary>
        /// Stop flow
        /// </summary>
        public virtual void SendStopFlow()
        {
            SendStopFlow(CurPistol);
        }

        protected virtual void SendStopFlow(int pistol)
        {
            if (pistol < 0 || pistol > pistols) return;
            //this.curMode = Modes.STOP_FLOW1;
            DataCommunication(DOSE_COMMAND, ReceiveStopFlow);
            //ReceiveStopFlow(bytes);
        }

        protected virtual void ReceiveStopFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1)
            {
                //IsFlowAllowed = false;
            }
        }

        /// <summary>
        /// Current dose
        /// </summary>
        public virtual void SendDose()
        {
            SendDose(CurPistol);
        }

        protected virtual void SendDose(int pistol)
        {
            if (pistol < 0 || pistol > pistols) return;
            //this.curMode = Modes.DOSE;
            DataCommunication(DOSE_COMMAND, ReceiveDose);
            //ReceiveDose(bytes);
        }

        protected virtual void ReceiveDose(byte[] bytes)
        {
            if (bytes == null)
            {
                //IsFlowAllowed = false;
                return;
            }
            this.currentDose = (DoseAlgorithm != null) ? DoseAlgorithm.Define(bytes) : 0;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public void OnTimerTick()
        //{

        //    switch (curMode)
        //    {
        //        case Modes.INIT:
        //            SendInit();
        //            break;
        //        case Modes.START_POLL:
        //            SendPoll();
        //            break;
        //        case Modes.START_FLOW:
        //            SendStartFlow();
        //            break;

        //        // ОШИБКА
        //        case Modes.STOP_FLOW1:
        //        case Modes.STOP_FLOW2:
        //            SendStopFlow();
        //            break;

        //        default:
        //            break;
        //    }
        //}
        public virtual void OnTimerTick()
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        protected void NotifyPropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }

    }
}
