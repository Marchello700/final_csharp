using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using DataLayer;
using Newtonsoft.Json;
using SendToWebAPI.Models;
using SQLiteClassLibrary;

namespace ConsoleApplication1
{
    public class DataPullerThread
    {
        private readonly Thread _thread;
        private readonly FullDataManager _dataManager;
        private readonly int _timePeriodMs;
        private readonly int _vmId;

        private void ThreadExecute()
        {
            try
            {
                while (true)
                {
                    DateTime start = DateTime.Now;
                    var compUsageData = _dataManager.GetComputerUsageData();
                    var usageData = new UsageData
                    {
                        AvailableDiskSpaceGb = compUsageData.AvailableDiskSpaceGb,
                        AverageQueueLength = compUsageData.AverageQueueLength,
                        CpuUsage = compUsageData.CpuUsage,
                        RamUsage = compUsageData.RamUsage,
                        Time = compUsageData.TimeMark
                    };
                    PostUsageData(usageData);
                    DateTime end = DateTime.Now;
                    int difference = (int)end.Subtract(start).TotalMilliseconds;
                    var remainingTime = _timePeriodMs - difference;
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

        private void PostUsageData(UsageData usageData)
        {
            using (var client = new HttpClient())
            {
                // New code:
                client.BaseAddress = new Uri("http://192.168.10.106/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var newUsageReport = new NewUsageData { TimeStamp = usageData.Time, ProcessorUsage = usageData.CpuUsage, MemoryUsage = usageData.RamUsage };

                var json = JsonConvert.SerializeObject(newUsageReport);

                var content = new StringContent(json);

                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                var response = client.PostAsync($"api/virtualmachines/{_vmId}/usagereports", content);

                var result = response.Result;
            }
        }

        public DataPullerThread(int vmId, int timePeriodMs)
        {
            _vmId = vmId;
            _timePeriodMs = timePeriodMs;
            _dataManager = new FullDataManager();
            _thread = new Thread(ThreadExecute)
            {
                IsBackground = true,
                Name = "DataPuller_Thread",
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
    }
}
