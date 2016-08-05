using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataLayer
{
    public class FullDataManager : DataManager
    {
        public override ComputerSummary GetComputerSummary()
        {
            return new ComputerSummary
            {
                AvailableDiskSpaceGb = GetAvailableDiskSpaceGb(),
                AverageDiskQueueLength = GetAverageDiskQueueLength(),
                Cpu = GetCpu(),
                Ip = GetIp(),
                Name = GetName(),
                Ram = GetRamGb(),
                RamUsage = GetRamUsage(),
                CpuUsage = GetCpuUsage(),
                User = GetUser(),
                VideoCard = GetVideoCard()
            };
        }
         
        public override List<string> GetApplicationList()
        {
            List<string> applications = new List<string>();
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                //if (!string.IsNullOrEmpty(process.MainWindowTitle)) //get applications only with active window
                    applications.Add(process.ProcessName);

            }
            return applications;
        }

        public override List<string> GetHardwareList()
        {
            List<string> hardwareList = new List<string>();
            hardwareList.Add("CPU: " + GetCpu());
            hardwareList.Add("VideoCard: " + GetVideoCard());
            hardwareList.Add("MotherBoard: " + GetMotherBoard());
            var hddList = GetHddList();
            foreach (var hddCaption in hddList)
            {
                hardwareList.Add("HDD: " + hddCaption);
            }
            hardwareList.Add("CdRom: " + GetCdRom());
            hardwareList.Add("RAM: " + GetRamPartNumber());
            return hardwareList;
        }

        public override ComputerUsageData GetComputerUsageData()
        {
            return new ComputerUsageData()
            {
                TimeMark = DateTime.Now,
                CpuUsage = GetCpuUsage(),
                RamUsage = GetRamUsage(),
                AvailableDiskSpaceGb = GetAvailableDiskSpaceGb(),
                AverageQueueLength = GetAverageDiskQueueLength()
            };
        }
    }
}
