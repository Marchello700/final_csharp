using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using DataLayer;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataManager = new FullDataManager();

            var computername = dataManager.GetMetric("computername");
            Console.WriteLine($"Computer Name: {computername}");

            var cpuUsage = dataManager.GetMetric("cpuusage");
            Console.WriteLine($"Current CPU usage: {cpuUsage}%");
            Console.WriteLine($"Current CPU usage: {cpuUsage}%");
            Console.ReadLine();
        }
    }
}
