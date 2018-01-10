using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace CryptoScheduler
{
    public partial class ExeScheduler : ServiceBase
    {
        private System.Diagnostics.EventLog eventLog1;
        private int eventId = 1;

        public ExeScheduler()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
            //eventLog1.WriteEntry("Created Logger", EventLogEntryType.Information, eventId++);
        }

        protected override void OnStart(string[] args)
        {
            // 15 min timer
            const double interval = 600000;
            Timer timer = new Timer(interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //eventLog1.WriteEntry("Time Elapsed", EventLogEntryType.Information, eventId++);
            TimeSpan start = new TimeSpan(11, 00, 0);
            TimeSpan end = new TimeSpan(6, 30, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;

            bool isTimeRange = false;

            if (start <= end)
            {
                // Same day
                if (now >= start && now <= end)
                {
                    isTimeRange = true;
                }
            }
            else
            {
                // Different days
                if (now >= start || now <= end)
                {
                    isTimeRange = true;
                }
            }

            if (isTimeRange)
            {
                if (Process.GetProcessesByName("xmrigService").Length == 0)
                {
                    eventLog1.WriteEntry("Starting CPU miner", EventLogEntryType.Information, eventId++);
                    Process processCpu = new Process();
                    processCpu.StartInfo.FileName = "C:\\Users\\sethp\\Desktop\\xmrig-2.4.3-msvc-win64\\Service\\xmrigService.exe";
                    processCpu.Start();
                }
                if (Process.GetProcessesByName("xmrig-nvidiaService").Length == 0)
                {
                    eventLog1.WriteEntry("Starting GPU miner", EventLogEntryType.Information, eventId++);
                    Process processGpu = new Process();
                    processGpu.StartInfo.FileName = "C:\\Users\\sethp\\Desktop\\xmrig-nvidia-2.4.2-cuda9-win64\\Service\\xmrig-nvidiaService.exe";
                    processGpu.Start();
                }
            }
            else
            {
                stopProcesses();
            }
        }

        protected override void OnStop()
        {
            stopProcesses();
        }

        private void stopProcesses()
        {
            foreach (Process p in Process.GetProcessesByName("xmrigService"))
            {
                p.Kill();
            }
            foreach (Process p in Process.GetProcessesByName("xmrig-nvidiaService"))
            {
                p.Kill();
            }
        }
    }
}
