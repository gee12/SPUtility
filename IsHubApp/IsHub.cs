using SPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsHubApp
{
    public class IsHub
    {
        //public const int ALG1_K1 = 40960;
        //public const int ALG1_K2 = 320;
        //public const int ALG1_K3 = 4;
        //public const int ALG2_K1 = 65536;
        //public const int ALG2_K2 = 512;
        //public const int ALG2_K3 = 4;

        //public enum DoseAlgorithms
        //{

        //}

        public class DoseAlgorithms : Enumeration
        {
            int k1;
            int k2;
            int k3;
            Func<byte[], int, int, int, int> alg;

            public DoseAlgorithms(int value, string name, int k1, int k2, int k3, Func<byte[], int, int, int, int> alg)
                : base(value, name)
            {
                this.k1 = k1;
                this.k2 = k2;
                this.k3 = k3;
                this.alg = alg;
            }

            public int Define(byte[] buf)
            {
                return (buf != null) ? alg.Invoke(buf, k1, k2, k3) : 0;
            }

            public static readonly DoseAlgorithms Algorithm1 = new DoseAlgorithms(0, "Алгоритм 1", 40960, 320, 4,
                (buf, k1, k2, k3) => { return (buf.Length >= 4) ? (buf[1] * k1 + buf[2] * k2 + (buf[3] / k3) * 10) : 0; });
            public static readonly DoseAlgorithms Algorithm2 = new DoseAlgorithms(1, "Алгоритм 2", 6512365, 5, 4,
                (buf, k1, k2, k3) => { return (buf.Length >= 4) ? (buf[1] * k1 + buf[2] * k2 + buf[3] * k3) : 0; });
        }

        private const byte[] INIT_COMMAND = { 0xE0 };
        private const byte[] POLL_COMMAND = { 0x8C };
        private const byte[] START_FLOW_COMMAND = { 0x90, 0x00, 0x00, 0x00 ,0x00 };
        private const byte[] STOP_FLOW_COMMAND = { 0xA0, 0x00, 0x00, 0x00 ,0x00 };
        private const byte DOSE_COMMAND = { 0x80 };

        private DoseAlgorithms doseAlgorithm = DoseAlgorithms.Algorithm1;
        public bool IsOnline { get; protected set; }
        public byte Side1Info { get; protected set; }
        public byte Side2Info { get; protected set; }
        public bool IsFlowAllowed { get; protected set; }

        private int curPistol;
        public Modes curMode = Modes.NONE;

        public bool Pistol11 { get { return (Side1Info & 41) > 0; } }
        public bool Pistol12 { get { return (Side1Info & 42) > 0; } }
        public bool Pistol13 { get { return (Side1Info & 44) > 0; } }
        public bool Pistol14 { get { return (Side1Info & 48) > 0; } }

        public bool Pistol21 { get { return (Side2Info & 41) > 0; } }
        public bool Pistol22 { get { return (Side2Info & 42) > 0; } }
        public bool Pistol23 { get { return (Side2Info & 44) > 0; } }
        public bool Pistol24 { get { return (Side2Info & 48) > 0; } }

        public IsHub()
        {
        }

        public void SetDoseAlgorithm(DoseAlgorithms alg)
        {
            this.doseAlgorithm = alg;
        }

        /// <summary>
        /// Init
        /// </summary>
        public byte[] SendInit()
        {
            this.curMode = Modes.INIT;
            return INIT_COMMAND;
        }

        public void ReceiveInit(byte[] bytes)
        {
            this.IsOnline = (bytes != null);
        }

        /// <summary>
        /// Poll
        /// </summary>
        public byte[] SendPoll()
        {
            this.curMode = Modes.START_POLL;
            return POLL_COMMAND;
        }

        public void ReceivePoll(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5)
            {
                //IsOnline = false;
                return;
            }

            Side1Info = bytes[2];
            Side2Info = bytes[3];
        }

        /// <summary>
        /// Start flow
        /// </summary>
        public byte[] SendStartFlow(int pistol)
        {
            if (pistol < 0 || pistol > 8) return null;

            var res = START_FLOW_COMMAND.Clone();
            res[0] + pistol;
            this.curPistol = pistol;
            this.curMode = Modes.START_FLOW;
            return res;
        }

        public void ReceiveStartFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1 || curPistol != (bytes[0] & 0x0F)
            {
                //IsFlowAllowed = false;
            }
        }

        /// <summary>
        /// Stop flow
        /// </summary>
        public byte[] SendStopFlow()
        {
            this.curMode = Modes.STOP_FLOW1;
            return SendStopFlow(curPistol);
        }

        private byte[] SendStopFlow(int pistol)
        {
            if (pistol < 0 || pistol > 8) return null;
            var res = STOP_FLOW_COMMAND.Clone();
            res[0] + pistol;
            this.curPistol = pistol;
            return res;
        }

        public void ReceiveStopFlow(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 1 || curPistol != (bytes[0] & 0x0F)
            {
                //IsFlowAllowed = false;
            }
        }

        /// <summary>
        /// Current dose
        /// </summary>
        public byte[] SendDose()
        {
            return SendDose(curPistol);
        }

        private byte[] SendDose(int pistol)
        {
            if (pistol < 0 || pistol > 8) return null;
            this.curPistol = pistol;
            return DOSE_COMMAND + pistol;
        }

        public int ReceiveDose(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5 || curPistol != (bytes[0] & 0x0F)
            {
                //IsFlowAllowed = false;
                return 0;
            }

            return doseAlgorithm.Define(bytes);
        }

        public enum Modes
        {
            NONE,
            INIT,
            START_POLL,
            //STOP_POLL,
            START_FLOW,
            STOP_FLOW1,
            STOP_FLOW2,
            DOSE
        }

        /// <summary>
        /// 
        /// </summary>
        public void Receive(byte[] bytes)
        {

            switch (curMode)
            {
                case Modes.INIT:
                    ReceiveInit(bytes);
                    break;
                case Modes.START_POLL:
                    ReceivePoll(bytes);
                    break;
                case Modes.START_FLOW:
                    ReceiveStartFlow(bytes);
                    break;
                case Modes.STOP_FLOW1:
                case Modes.STOP_FLOW2:
                    ReceiveStopFlow(bytes);
                    break;
            }
        }
    }
}
