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
        private readonly string _computerName;

        public event EventHandler<UpdateEventArgs> UpdateFinished;

        private void ThreadExecute()
        {
            try
            {
                while (true)
                {
                    DateTime start = DateTime.Now;
                    using (var _metricsContext = new MetricsContext())
                    {
                        OnUpdateFinished(MetricsContextSupport.GetComputerDetail(_metricsContext, _computerName));
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

        public UpdaterThread(string computerName)
        {
            _syncContext = SynchronizationContext.Current;
            _thread = new Thread(ThreadExecute)
            {
                IsBackground = true,
                Name = "UpdateThread",
                Priority = ThreadPriority.Normal
            };
            _computerName = computerName;
            using (var _metricsContext = new MetricsContext())
            {
                _metricsContext.Database.EnsureCreated();
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
        }

        protected virtual void OnUpdateFinished(ComputerDetail computerDetail)
        {
            if (UpdateFinished != null)
            {
                SendOrPostCallback method = delegate {
                    UpdateFinished(this, new UpdateEventArgs(computerDetail));
                };
                _syncContext.Send(method, null);
            }
        }
    }
}
