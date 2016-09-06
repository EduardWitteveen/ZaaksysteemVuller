using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GenericUTurn.ServiceService
{
    public partial class MidofficeVullerService : ServiceBase
    {
        private System.Diagnostics.EventLog eventLog;
        //private int eventId = 0;

        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private Thread thread;

    public MidofficeVullerService()
        {
            InitializeComponent();

            eventLog = new System.Diagnostics.EventLog();
            // When: System.Security.SecurityException : Kan de bron niet vinden, maar kan sommige of alle gebeurtenislogboeken niet doorzoeken. Niet-toegankelijke logboeken: Security.
            // 1: give the user rights to: HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog
            // 2: run as Administrator
            // 3: turn of UAC
            if (!System.Diagnostics.EventLog.SourceExists(this.GetType().Assembly.GetName().Name))
            {
                System.Diagnostics.EventLog.CreateEventSource(this.GetType().Assembly.GetName().Name, Properties.Settings.Default.EventlogLog);
            }
            eventLog.Source = this.GetType().Assembly.GetName().Name;
            eventLog.Log = Properties.Settings.Default.EventlogLog;
            this.ServiceName = this.GetType().Assembly.GetName().Name;

            // now set the directory to the directory where our executable is,...
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var exe = new System.IO.FileInfo(Uri.UnescapeDataString(uri.Path));
            System.IO.Directory.SetCurrentDirectory(exe.Directory.FullName);
        }


        private void WorkerThread()
        {
            eventLog.WriteEntry("Worker thread: start in directory:" + System.IO.Directory.GetCurrentDirectory() , EventLogEntryType.Information);
            try
            {

                int sleeptime;
                do
                {
                    int changes = 0;
                    GenericUTurn uturn = new GenericUTurn();
                    foreach (Zaaktype zaaktype in uturn.GetTypes())
                    {
                        try {
                            changes += uturn.Synchronize(zaaktype);
                        }
                        catch (Exception ex)
                        {
                            // set the source correct
                            eventLog.Source = ex.Source;
                            string message = "Zaaktype: " + zaaktype.Description + "(" + zaaktype.Code + ") exception:" + ex.ToString();
                            eventLog.WriteEntry(message, EventLogEntryType.Warning);
                            // restore the source
                            eventLog.Source = this.GetType().Assembly.GetName().Name;
                        }
                        // look if we need to stop now
                        if (shutdownEvent.WaitOne(0))
                        {
                            eventLog.WriteEntry("Worker thread: stop while processing", EventLogEntryType.Information);
                            return;
                        }
                    }
                    // only sleep when there where no changes!
                    sleeptime = (changes == 0) ? Properties.Settings.Default.CheckIntervalSeconds * 1000 : 0;
                    Console.WriteLine("Going to sleep for:" + sleeptime / 1000 + " seconds");
                }
                while (!shutdownEvent.WaitOne(sleeptime));
                eventLog.WriteEntry("Worker thread: stop from wait", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                string message = "Worker thread: mainloop failure! Exception:" + ex.ToString();
                eventLog.WriteEntry(message, EventLogEntryType.Error);
            }
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart", EventLogEntryType.Information);

            thread = new Thread(WorkerThread);
            thread.Name = "WorkerThread";
            thread.IsBackground = true;
            thread.Start();
        }

        protected override void OnPause()
        {
            eventLog.WriteEntry("In OnPause", EventLogEntryType.Information);
        }

        protected override void OnCustomCommand(int command)
        {
            eventLog.WriteEntry("In OnCustomCommand:" + command, EventLogEntryType.Information);
        }

        protected override void OnShutdown()
        {
            eventLog.WriteEntry("In OnShutdown", EventLogEntryType.Information);
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue", EventLogEntryType.Information);
        }  

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop", EventLogEntryType.Information);
            shutdownEvent.Set();
            if (!thread.Join(Properties.Settings.Default.AbortIntervalSeconds * 1000))
            { 
                thread.Abort();
            }
        }
    }
}
