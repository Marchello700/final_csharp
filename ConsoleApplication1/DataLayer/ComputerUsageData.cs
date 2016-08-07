using System;

namespace DataLayer
{
    public class ComputerUsageData
    {
        public DateTime TimeMark { get; set; }
        public int CpuUsage { get; set; }
        public int RamUsage { get; set; }
        public int AvailableDiskSpaceGb { get; set; }
        public int AverageQueueLength { get; set; }
    }
}
