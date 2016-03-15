using IsHubLib;
using SPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IsHubEmulator
{
    class Program
    {
        static SPConnection comPort;
        static IsHub gilbarco;

        static void Main(string[] args)
        {
            comPort = new SPConnection();
            comPort.DataReceivingHandler += DataReceived;

            gilbarco = new Gilbarco();

            gilbarco.POLL_ANSWER[2] += 1;
            gilbarco.POLL_ANSWER[3] += 6;

            //
            do
            {
                bool res = false;
                int comNum;
                do
                {
                    Console.WriteLine("Номер COM:");
                    string com = Console.ReadLine();
                    res = Int32.TryParse(com, out comNum);
                }
                while (!res);

                comPort.Port.PortName = "COM" + comNum;

                comPort.OpenPort();
            }
            while (!comPort.IsPortOpened);

            Console.WriteLine("Порт открыт");
            Console.WriteLine("Esc для выхода..");

            //
            while (true)
            {
                Thread.Sleep(1000);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                    break;
            }

            //
            comPort.ClosePort();
        }

        static void DataReceived(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;

            string receivedS = Utils.ToHexString(bytes);
            Console.WriteLine("Принято: " + receivedS);

            byte first = bytes[0];
            byte[] answer = new byte[1];

            if (first == gilbarco.INIT_COMMAND[0])
            {
                answer = gilbarco.INIT_ANSWER;
            }
            else if (first == gilbarco.POLL_COMMAND[0])
            {
                answer = gilbarco.POLL_ANSWER;
            }
            else if ((first & 0xF0) == 0x90)//(gilbarco.START_FLOW_COMMAND[0] & 0xF0))
            {
                answer = new byte[] { (byte)(gilbarco.START_FLOW_ANSWER[0] + (((byte)first) & 0x0F)) };
                //answer = new byte[] { 0xFF };
            }
            else if (first == gilbarco.DOSE_COMMAND[0])
            {
                gilbarco.DOSE_ANSWER[0] += (byte)(first & 0x0F);
                gilbarco.DOSE_ANSWER[3] += 1;
                if (gilbarco.DOSE_ANSWER[3] > 250)
                {
                    gilbarco.DOSE_ANSWER[2] += 1;
                }
                answer = gilbarco.DOSE_ANSWER;
            }
            else if (first == gilbarco.STOP_FLOW_COMMAND[0])
            {
                answer = new byte[] { (byte)(gilbarco.STOP_FLOW_ANSWER[0] + first & 0x0F) };
            }

            Thread.Sleep(500);
            comPort.SendData(answer);

            string sentS = Utils.ToHexString(answer);
            Console.WriteLine("Отправлено: " + sentS);
        }

    }
}
