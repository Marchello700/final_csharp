using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteClassLibrary
{
    public static class MetricsContextSupport
    {
        public static ComputerDetail GetComputerDetail(MetricsContext metricsContext, string computerName)
        {
            var computerDetail = metricsContext.ComputerDetails.FirstOrDefault(x => x.ComputerName == computerName);
            computerDetail.UsageDataCollection = metricsContext.UsageDatas.ToList();
            return computerDetail;
        }

        public static ComputerDetail AddComputerDetail(MetricsContext metricsContext, ComputerSummary computerSummary)
        {
            var computerDetail = new ComputerDetail();
            computerDetail.ComputerName = computerSummary.Name;
            computerDetail.UserName = computerSummary.User;
            computerDetail.Cpu = computerSummary.Cpu;
            computerDetail.Ram = computerSummary.Ram.ToString();
            computerDetail.VideoCard = computerSummary.VideoCard;
            computerDetail.Ip = computerSummary.Ip.ToString();
            computerDetail.UsageDataCollection = new List<UsageData>();
            metricsContext.Add(computerDetail);
            metricsContext.SaveChanges();
            return computerDetail;
        }
    }
}
