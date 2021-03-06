﻿using System.Collections.Generic;

namespace SQLiteClassLibrary
{
    public class ComputerDetail
    {
        public int ComputerDetailId { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string Cpu { get; set; }
        public string Ram { get; set; }
        public string VideoCard { get; set; }
        public string Ip { get; set; }
        public ICollection<UsageData> UsageDataCollection { get; set; }
    }
}
