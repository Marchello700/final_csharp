using System;
using DataLayer;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataManager = new FullDataManager();

            var applicationsNames = dataManager.GetApplicationList();
            Console.WriteLine("Application list:");
            foreach (var applicationName in applicationsNames)
            {
                Console.WriteLine(applicationName);
            }
            Console.WriteLine();

            var computerSummary = dataManager.GetComputerSummary();
            Console.WriteLine("Computer summary:");
            Console.WriteLine($"Name: {computerSummary.Name}");
            Console.WriteLine($"User: {computerSummary.User}");
            Console.WriteLine($"CPU: {computerSummary.Cpu}");
            Console.WriteLine($"Ram: {computerSummary.Ram}GB");
            Console.WriteLine($"Video Card: {computerSummary.VideoCard}");
            Console.WriteLine($"Ip Adress: {computerSummary.Ip}");
            Console.WriteLine($"CpuUsage: {computerSummary.CpuUsage}%");
            Console.WriteLine($"RamUsage: {computerSummary.RamUsage}%");
            Console.WriteLine($"AvailableDiskSpaceGb: {computerSummary.AvailableDiskSpaceGb}GB");
            Console.WriteLine($"AverageDiskQueueLength: {computerSummary.AverageDiskQueueLength}");
            Console.WriteLine();

            
            var hardwareList = dataManager.GetHardwareList();
            Console.WriteLine("Hardware list:");
            foreach (var hardware in hardwareList)
            {
                Console.WriteLine(hardware);
            }
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
