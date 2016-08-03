using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class UpdaterThread
    {
        private Thread _thread;
        private SynchronizationContext _syncContext;
        private FullDataManager _dataManager;

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
            _thread = new Thread(ThreadExecute);
            _thread.IsBackground = true;
            _thread.Name = "UpdateThread";
            _thread.Priority = ThreadPriority.Normal;
        }

        public UpdaterThread()
        {
            _dataManager = new FullDataManager();
            _syncContext = SynchronizationContext.Current;
            _thread = new Thread(new ThreadStart(ThreadExecute));
            _thread.IsBackground = true;
            _thread.Name = "UpdateThread";
            _thread.Priority = ThreadPriority.Normal;
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
                SendOrPostCallback method = new SendOrPostCallback(
                delegate (object state)
                {
                    UpdateFinished(this, EventArgs.Empty);
                });
                _syncContext.Send(method, null);
            }
        }
    }
}
