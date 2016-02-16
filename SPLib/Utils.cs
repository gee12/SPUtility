using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SPLib
{
    public class Utils
    {
        public static string[] Ports =
        {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
        };

        public static int[] BaudRates = 
        {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            14400,
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

    }
}
