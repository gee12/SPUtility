using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SPLib
{
    public class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract class AsyncTask<TForm, TParam, TResult>
        {
            protected abstract void pre(TForm form);
            protected abstract TResult run(TParam param);
            protected abstract void post(TForm form, TResult res);

            public void Start(TForm form, Dispatcher dispatcher, TParam param)
            {
                dispatcher.Invoke((Action)(() => pre(form)));

                Thread thread = new Thread(() =>
                {
                    TResult res = run(param);
                    dispatcher.Invoke((Action)(() => post(form, res)));
                });
                thread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract class AsyncTask<TForm, TParam1, TParam2, TParam3, TParam4, TResult>
        {
            protected abstract void pre(TForm form);
            protected abstract TResult run(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
            protected abstract void post(TForm form, TResult res);

            public void Start(TForm form, Dispatcher dispatcher, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
            {
                dispatcher.Invoke((Action)(() => pre(form)));

                Thread thread = new Thread(() =>
                {
                    TResult res = run(param1, param2, param3, param4);
                    dispatcher.Invoke((Action)(() => post(form, res)));
                });
                thread.Start();
            }
        }

        public static byte[] ToByteHexArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] ToByteHexArray(string hex, string separ)
        {
            var hexes = hex.Split(new [] {separ}, StringSplitOptions.RemoveEmptyEntries);
            return hexes.Select(x => Convert.ToByte(x, 16))
                             .ToArray();
        }

        public static string ToString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:X2} ", b);
            return hex.ToString();
        }
    }
}
