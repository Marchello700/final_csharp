using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace DataLayer
{
    public enum DataMetrics
    {
        Name, User, Cpu, Ram, VideoCard, Ip,
        CpuUsage, RamUsage, AvailableDiskSpaceGb,
        AverageDiskQueueLength
    }

    public abstract class DataManager
    {
        private ManagementObject GetManagementObject(string win32WmiClass)
        {
            string query = "SELECT * FROM " + win32WmiClass;
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(query);
            foreach (var o in objSearcher.Get())
            {
                var mo = (ManagementObject) o;
                return mo;
            }
            return null;
        }

        private List<ManagementObject> GetManagementObjectList(string win32WmiClass)
        {
            var result = new List<ManagementObject>();
            string query = "SELECT * FROM " + win32WmiClass;
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(query);
            foreach (var o in objSearcher.Get())
            {
                var mo = (ManagementObject) o;
                result.Add(mo);
            }
            return result;
        }

        private string GetManagementObjectStringField(ManagementObject obj, string fieldName)
        {
            return obj?[fieldName]?.ToString();
        }

        public virtual string GetMetric(DataMetrics type)
        {
            var value = "";
            switch (type)
            {
                case DataMetrics.CpuUsage:
                    var searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
                    foreach (var obj in searcher.Get().Cast<ManagementObject>())
                    {
                        value = obj["PercentProcessorTime"].ToString();
                        break;
                    }
                    break;
                case DataMetrics.Name:
                    value = Environment.MachineName;
                    break;
                case DataMetrics.User:
                    value = Environment.UserName;
                    break;
                case DataMetrics.Cpu:
                    ManagementObjectSearcher mos =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                    foreach (var o in mos.Get())
                    {
                        var mo = (ManagementObject) o;
                        value = mo["Name"].ToString();
                        break;
                    }
                    break;
                case DataMetrics.Ram:
                    break;
                case DataMetrics.VideoCard:
                    break;
                case DataMetrics.Ip:
                    break;
                case DataMetrics.RamUsage:
                    break;
                case DataMetrics.AvailableDiskSpaceGb:
                    break;
                case DataMetrics.AverageDiskQueueLength:
                    break;
                default:
                    value = "";
                    break;

            }

            return value;
        }

        public string GetName()
        {
            return Environment.MachineName;
        }

        public string GetUser()
        {
            return Environment.UserName;
        }

        public string GetCpu()
        {
            return GetManagementObjectStringField(GetManagementObject("Win32_Processor"), "Name");
        }

        public int GetRamGb()
        {
            long totalCapacity = 0;
            var moList = GetManagementObjectList("Win32_PhysicalMemory");
            foreach (var ram in moList)
            {
                totalCapacity += Convert.ToInt64(GetManagementObjectStringField(ram, "Capacity"));
            }
            return (int)(totalCapacity / 1024 / 1024 / 1024);
        }

        public string GetVideoCard()
        {
            return GetManagementObjectStringField(GetManagementObject("Win32_VideoController"), "Name");
        }

        public IPAddress GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public int GetCpuUsage()
        {
            var value = GetManagementObjectStringField(GetManagementObject("Win32_PerfFormattedData_PerfOS_Processor"), "PercentProcessorTime");
            //var cpuList = this.GetManagementObjectList("Win32_PerfFormattedData_PerfOS_Processor");
            //there is MO for each cpu core and additionally for "_total" cpu
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return 0;
        }

        public int GetRamUsage()
        {
            var mo = GetManagementObject("Win32_OperatingSystem");
            double free = double.Parse(GetManagementObjectStringField(mo,"FreePhysicalMemory"));
            double total = double.Parse(GetManagementObjectStringField(mo,"TotalVisibleMemorySize"));
            return (int)(Math.Round(((total - free) / total * 100), 2));

        }

        public int GetAvailableDiskSpaceGb()
        {
            long totalCapacity = 0;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                totalCapacity += drive.TotalFreeSpace;
            }
            return (int)(totalCapacity / 1024 / 1024 / 1024);
        }

        public int GetAverageDiskQueueLength()
        {
            var value = GetManagementObjectStringField(GetManagementObject("Win32_PerfFormattedData_PerfDisk_PhysicalDisk"), "AvgDiskQueueLength");
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return 0;
        }

        public string GetMotherBoard()
        {
            return GetManagementObjectStringField(GetManagementObject("Win32_BaseBoard"), "Product");
        }

        public List<string> GetHddList()
        {
            List<ManagementObject> moList = GetManagementObjectList("Win32_DiskDrive");
            var result = new List<string>();
            foreach (var mo in moList)
            {
                result.Add(GetManagementObjectStringField(mo, "Caption"));
            }
            return result;
        }

        public string GetCdRom()
        {
            return GetManagementObjectStringField(GetManagementObject("Win32_CDROMDrive"), "PNPDeviceID");
        }

        public string GetRamPartNumber()
        {
            return GetManagementObjectStringField(GetManagementObject("Win32_PhysicalMemory"), "PartNumber");
        }

        public abstract ComputerSummary GetComputerSummary();
        public abstract List<string> GetApplicationList();
        public abstract List<string> GetHardwareList();
    }
}
