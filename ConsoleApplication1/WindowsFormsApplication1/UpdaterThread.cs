using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SQLiteClassLibrary;

namespace WindowsFormsApplication1
{
    public class UpdaterThread
    {
        private readonly Thread _thread;
        private readonly SynchronizationContext _syncContext;
        private readonly FullDataManager _dataManager;

        public string TimeMark { get; set; }
        public int CpuUsage { get; set; }
        public int RamUsage { get; set; }
        public int AvailableDiskSpaceGb { get; set; }
        public int AverageQueueLength { get; set; }

        public event EventHandler UpdateFinished;

        private void ThreadExecute()
        {
            try
            {
                while (true)
                {
                    DateTime start = DateTime.Now;
                    TimeMark = DateTime.Now.ToString("hh:mm:ss");
                    CpuUsage = _dataManager.GetCpuUsage();
                    RamUsage = _dataManager.GetRamUsage();
                    AvailableDiskSpaceGb = _dataManager.GetAvailableDiskSpaceGb();
                    AverageQueueLength = _dataManager.GetAverageDiskQueueLength();
                    OnUpdateFinished(); // Notify all subscribers (on their own threads)  

                    using (var metricsContext = new MetricsContext())
                    {
                        var computerDetails = metricsContext.ComputerDetails.First();
                        var usageData = new UsageData
                        {
                            AvailableDiskSpaceGb = AvailableDiskSpaceGb,
                            AverageQueueLength = AverageQueueLength,
                            CpuUsage = CpuUsage,
                            RamUsage = RamUsage,
                            Time = start
                        };
                        if (computerDetails.UsageDataCollection == null)
                        {
                            computerDetails.UsageDataCollection = new List<UsageData>();
                        }
                        computerDetails.UsageDataCollection.Add(usageData);
                        metricsContext.Update(computerDetails);
                        metricsContext.SaveChanges();
                    }

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

        public UpdaterThread(FullDataManager dataManager)
        {
            _dataManager = dataManager;
            _syncContext = SynchronizationContext.Current;
            _thread = new Thread(ThreadExecute)
            {
                IsBackground = true,
                Name = "UpdateThread",
                Priority = ThreadPriority.Normal
            };
        }

        public UpdaterThread()
        {
            _dataManager = new FullDataManager();
            _syncContext = SynchronizationContext.Current;
            _thread = new Thread(ThreadExecute)
            {
                IsBackground = true,
                Name = "UpdateThread",
                Priority = ThreadPriority.Normal
            };
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
        }

        protected virtual void OnUpdateFinished()
        {
            if (UpdateFinished != null)
            {
                SendOrPostCallback method = delegate {
                    UpdateFinished(this, EventArgs.Empty);
                };
                _syncContext.Send(method, null);
            }
        }
    }
}
