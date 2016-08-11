using System;
using static System.Int32;

namespace ConsoleApplication1
{
    class Program
    {
        private static void Log(string data)
        {
            Console.WriteLine($"[Log][{DateTime.Now.ToString("HH:mm:ss")}]: " + data);
        }

        static void Main(string[] args)
        {
            DataPullerThread dataPullerThread;
            int vmValue = 18;
            int timePeriodValue = 1000;
            if (args.Length == 2)
            {
                TryParse(args[0], out vmValue);
                TryParse(args[1], out timePeriodValue);
                dataPullerThread = new DataPullerThread(vmValue, timePeriodValue);
            }
            else
            {
                dataPullerThread = new DataPullerThread(18,1000);
            }
            Log($"vmId:{vmValue}");
            Log($"timePeriodValue:{timePeriodValue}");
            Log("Thread started");
            dataPullerThread.Start();
            Console.ReadKey();
            dataPullerThread.Abort();
            Log("Thread aborted");
        }
    }
}
