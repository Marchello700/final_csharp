using System;

namespace SendToWebAPI.Models
{
    public class NewUsageData
    {
        public DateTime TimeStamp { get; set; }
        
        public double ProcessorUsage { get; set; }
        
        public double MemoryUsage { get; set; }
    }
}