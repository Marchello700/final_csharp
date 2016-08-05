using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SQLiteClassLibrary;

namespace WindowsFormsApplication1
{
    public class DataPullerThread
    {
        private readonly Thread _thread;
        private readonly FullDataManager _dataManager;
        private readonly MetricsContext _metricsContext;
        private readonly ComputerDetail _computerDetail;

        private void ThreadExecute()
        {
            try
            {
                while (true)
                {
                    DateTime start = DateTime.Now;
                    var CompUsageData = _dataManager.GetComputerUsageData();
                    var usageData = new UsageData
                    {
                        AvailableDiskSpaceGb = CompUsageData.AvailableDiskSpaceGb,
                        AverageQueueLength = CompUsageData.AverageQueueLength,
                        CpuUsage = CompUsageData.CpuUsage,
                        RamUsage = CompUsageData.RamUsage,
                        Time = CompUsageData.TimeMark
                    };
                    _computerDetail.UsageDataCollection.Add(usageData);
                    _metricsContext.Update(_computerDetail);
                    _metricsContext.SaveChanges();
                    DateTime end = DateTime.Now;
                    int difference = (int)end.Subtract(start).TotalMilliseconds;
                    var remainingTime = 1000 - difference;
                    if (remainingTime > 0)
                    {
                        Thread.Sleep(remainingTime);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // The thread was aborted... ignore this exception if it's safe to do so  
            }
        }

        private ComputerDetail GetComputerDetail()
        {
            ComputerDetail computerDetail = MetricsContextSupport.GetComputerDetail(_metricsContext, _dataManager.GetName());
            if (computerDetail == null)
            {
                computerDetail = MetricsContextSupport.AddComputerDetail(_metricsContext,_dataManager.GetComputerSummary());
            }
            return computerDetail;
        }

        public DataPullerThread()
        {
            _dataManager = new FullDataManager();
            _thread = new Thread(ThreadExecute)
            {
                IsBackground = true,
                Name = "UpdateThread",
                Priority = ThreadPriority.Normal
            };
            _metricsContext = new MetricsContext();
            _metricsContext.Database.EnsureCreated();
            _computerDetail = GetComputerDetail();
            if (_computerDetail.UsageDataCollection == null)
            {
                _computerDetail.UsageDataCollection = new List<UsageData>();
            }
        }

        public void Start()
        {
            if ((_thread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                _thread.Start();
            }
            else
            {
                throw new Exception("Thread has already been started and may have completed already");
            }
        }

        public void Abort()
        {
            _thread.Abort();
            while (_thread.IsAlive)
            {
            }
            _metricsContext.Dispose();
        }
    }
}
