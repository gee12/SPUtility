using SPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsHubLib
{
    public class Gilbarco : IsHub
    {
        /// <summary>
        /// 
        /// </summary>
        public class GilbarcoDoseAlgorithms : DoseAlgorithms
        {
            int k1;
            int k2;
            int k3;
            Func<byte[], int, int, int, int> alg;

            public GilbarcoDoseAlgorithms() : base() { }

            public GilbarcoDoseAlgorithms(int value, string name, int k1, int k2, int k3, Func<byte[], int, int, int, int> alg)
                : base(value, name)
            {
                this.k1 = k1;
                this.k2 = k2;
                this.k3 = k3;
                this.alg = alg;
            }

            public override int Define(byte[] buf)
            {
                return (buf != null) ? alg.Invoke(buf, k1, k2, k3) : 0;
            }

            public static readonly GilbarcoDoseAlgorithms Algorithm1 = new GilbarcoDoseAlgorithms(1, "Алгоритм 1", 40960, 320, 4,
                (buf, k1, k2, k3) => { return (buf.Length >= 4) ? (buf[1] * k1 + buf[2] * k2 + (buf[3] / k3) * 10) : 0; });
            public static readonly GilbarcoDoseAlgorithms Algorithm2 = new GilbarcoDoseAlgorithms(2, "Алгоритм 2", 6512365, 5, 4,
                (buf, k1, k2, k3) => { return (buf.Length >= 4) ? (buf[1] * k1 + buf[2] * k2 + buf[3] * k3) : 0; });
        }

        public override byte[] INIT_COMMAND { get { return new byte[] { 0xEC }; } }
        public override byte[] POLL_COMMAND { get { return new byte[] { 0x8C }; } }
        public override byte[] START_FLOW_COMMAND { get { return new byte[] { 0x90, 0x45, 0x6E, 0x61, 0x5B }; } }
        public override byte[] DOSE_COMMAND { get { return new byte[] { 0x80 }; } }
        public override byte[] STOP_FLOW_COMMAND { get { return new byte[] { 0xA0, 0x44, 0x69, 0x73, 0x3F }; } }


        public override byte[] INIT_ANSWER { get { return new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }; } }
        public override byte[] POLL_ANSWER { get { return new byte[] { 0x0C, 0x70, 0x40, 0x40, 0x00 }; } }
        public override byte[] START_FLOW_ANSWER { get { return new byte[] { 0x00 }; } }
        public override byte[] DOSE_ANSWER { get { return new byte[] { 0x00, 1, 2, 3, 4, 0 }; } }
        public override byte[] STOP_FLOW_ANSWER { get { return new byte[] { 0x00 }; } }
        protected byte[] STOP_FLOW_ANSWER2 { get { return new byte[] { 0x30 }; } }

        /// <summary>
        /// 
        /// </summary>
        public bool Pistol12 { get { return (Side1Info & 0x42) == 0x42; } }
        public bool Pistol11 { get { return (Side1Info & 0x41) == 0x41; } }
        public bool Pistol13 { get { return (Side1Info & 0x44) == 0x44; } }
        public bool Pistol14 { get { return (Side1Info & 0x48) == 0x48; } }

        public bool Pistol21 { get { return (Side2Info & 0x41) == 0x41; } }
        public bool Pistol22 { get { return (Side2Info & 0x42) == 0x42; } }
        public bool Pistol23 { get { return (Side2Info & 0x44) == 0x44; } }
        public bool Pistol24 { get { return (Side2Info & 0x48) == 0x48; } }

        /// <summary>
        /// 
        /// </summary>
        public Gilbarco()
        {
            Init();
            this.Pistols = 8;
        }

        /// <summary>
        /// 
        /// </summary>
        public Gilbarco(DoseAlgorithms alg, Action<byte[]> sentHandler, Action<byte[]> receivedHandler)
            : base (8, alg, sentHandler, receivedHandler)
        {
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            //INIT_COMMAND = new byte[] { 0xE0 };
            //POLL_COMMAND = new byte[] { 0x8C };
            //START_FLOW_COMMAND = new byte[] { 0x90, 0x00, 0x00, 0x00, 0x00 };
            //STOP_FLOW_COMMAND = new byte[] { 0xA0, 0x00, 0x00, 0x00, 0x00 };
            //DOSE_COMMAND = new byte[] { 0x80 };

            //INIT_ANSWER = new byte[] { 1,2,3,4,5,6,7,8,9,0 };
            //POLL_ANSWER = new byte[] { 0x0C, 0x70, 0x40, 0x40, 0x00 };
            //START_FLOW_ANSWER = new byte[] { 0x00 };
            //DOSE_ANSWER = new byte[] { 0x00, 1, 2, 3, 4, 0 };
            //STOP_FLOW_ANSWER = new byte[] { 0x00 };
            //STOP_FLOW_ANSWER2 = new byte[] { 0x30 };
        }

        /// <summary>
        /// Poll
        /// </summary>
        protected override void ReceivePoll(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5)
            {
                //IsOnline = false;
                return;
            }
            this.Side1Info = bytes[2];
            this.Side2Info = bytes[3];

            int flag = (Side1Info & 0x42);

            // update pistols properties
            NotifyPropertiesChanged();
        }

        /// <summary>
        /// Start flow
        /// </summary>
        public override void SendStartFlow(int pistol)
        {
            if (pistol < 0 || pistol > Pistols) return;
            this.curMode = Modes.START_FLOW;
            var res = (byte[])START_FLOW_COMMAND.Clone();
            res[0] = (byte)(res[0] + pistol);
            this.CurPistol = pistol;
            var bytes = DataCommunication(res, ReceivePoll);
            //ReceivePoll(bytes);
        }

        protected override void ReceiveStartFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0 || CurPistol != (bytes[0] & 0x0F))
            {
                //IsFlowAllowed = false;
                return;
            }
            IsFlowAllowed = true;
        }

        /// <summary>
        /// Current dose
        /// </summary>
        public override void SendDose()
        {
            SendDose(CurPistol);
        }

        protected override void SendDose(int pistol)
        {
            if (pistol < 0 || pistol > Pistols) return;

            this.curMode = Modes.DOSE;
            var res = (byte[])DOSE_COMMAND.Clone();
            res[0] = (byte)(res[0] + pistol);
            byte[] bytes = DataCommunication(res, ReceiveDose);
            //ReceiveDose(bytes);
        }

        protected override void ReceiveDose(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5 || CurPistol != (bytes[0] & 0x0F))
            {
                //IsFlowAllowed = false;
                return;
            }
            this.CurrentDose = (DoseAlgorithm != null) ? DoseAlgorithm.Define(bytes) : 0;
        }

        /// <summary>
        /// Stop flow
        /// </summary>
        public override void SendStopFlow()
        {
            SendStopFlow(CurPistol);
        }

        protected override void SendStopFlow(int pistol)
        {
            if (pistol < 0 || pistol > Pistols) return;
            this.curMode = Modes.STOP_FLOW1;
            var res = (byte[])STOP_FLOW_COMMAND.Clone();
            res[0] = (byte)(res[0] + pistol);
            byte[] bytes = DataCommunication(res, ReceiveStopFlow);
            //ReceiveStopFlow(bytes);
        }

        protected override void ReceiveStopFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0 || CurPistol != (bytes[0] & 0x0F))
            {
                return;
            }
            IsFlowAllowed = false;
        }

    }
}
