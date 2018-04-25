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
        //private System.Diagnostics. eventLog;
        //private int eventId = 0;

        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private Thread thread;

    public MidofficeVullerService()
        {
            InitializeComponent();

            //SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            /*
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
            */
            this.ServiceName = this.GetType().Assembly.GetName().Name;

            // now set the directory to the directory where our executable is,...
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            var exe = new System.IO.FileInfo(Uri.UnescapeDataString(uri.Path));
            if(exe.Directory.Exists) { 
                System.IO.Directory.SetCurrentDirectory(exe.Directory.FullName);
            }
        }


        private void WorkerThread()
        {
            int waittime = (int) (1000 * Properties.Settings.Default.WaitAfterRequestSeconds);
            // eventLog.WriteEntry("Worker thread: start in directory:" + System.IO.Directory.GetCurrentDirectory() + " with an waittime: " + waittime, EventLogEntryType.Information);
#if !DEBUG
            try
            {
#endif

                int sleeptime;
                do
                {
                    int changes = 0;
                    GenericUTurn uturn = new GenericUTurn();
                    foreach (Zaaktype zaaktype in uturn.GetTypes())                    
                    {
#if !DEBUG
                        try {
#endif
                            changes += uturn.Synchronize(zaaktype, waittime);
#if !DEBUG
                        }
                        catch (Exception ex)
                        {
                            // set the source correct
                            //eventLog.Source = ex.Source;
                            string message = "Zaaktype: " + zaaktype.Description + "(" + zaaktype.Code + ") exception:" + ex.ToString();
                            //try { 
                            //eventLog.WriteEntry(message, EventLogEntryType.Warning);
                            Console.WriteLine(message);
                            //} catch(Exception) {
                            // Console.WriteLine("CRITICAL: Cannot write to EVENTLOG:" + eventLog.Source);
                            //}
                            // restore the source
                            //eventLog.Source = this.GetType().Assembly.GetName().Name;
                        }
#endif
                        // look if we need to stop now
                        if (shutdownEvent.WaitOne(0))
                        {
                            //eventLog.WriteEntry("Worker thread: stop while processing", EventLogEntryType.Information);
                            Console.WriteLine("Worker thread: stop while processing", EventLogEntryType.Information);
                            return;
                        }
                    }
                    // only sleep when there where no changes!
                    sleeptime = (changes == 0) ? Properties.Settings.Default.CheckIntervalSeconds * 1000 : 0;
                    Console.WriteLine("Going to sleep for:" + sleeptime / 1000 + " seconds");
                }
                while (!shutdownEvent.WaitOne(sleeptime));
                //eventLog.WriteEntry("Worker thread: stop from wait", EventLogEntryType.Information);
                Console.WriteLine("Worker thread: stop from wait", EventLogEntryType.Information);
#if !DEBUG
            }
            catch (Exception ex)
            {
                string message = "Worker thread: mainloop failure! Exception:" + ex.ToString();
                Console.WriteLine("ERROR:" + message);
                //eventLog.WriteEntry(message, EventLogEntryType.Error);
                Console.WriteLine(message);
            }
#endif
        }

        protected override void OnStart(string[] args)
        {
            //eventLog.WriteEntry("In OnStart", EventLogEntryType.Information);
            Console.WriteLine("In OnStart");

            thread = new Thread(WorkerThread);
            thread.Name = "WorkerThread";
            thread.IsBackground = true;
            thread.Start();
        }

        protected override void OnPause()
        {
            //eventLog.WriteEntry("In OnPause", EventLogEntryType.Information);
            Console.WriteLine("In OnPause");
        }

        protected override void OnCustomCommand(int command)
        {
            //eventLog.WriteEntry("In OnCustomCommand:" + command, EventLogEntryType.Information);
            Console.WriteLine("In OnCustomCommand: " + command);
        }

        protected override void OnShutdown()
        {
            //eventLog.WriteEntry("In OnShutdown", EventLogEntryType.Information);
            Console.WriteLine("In OnShutdown");
        }

        protected override void OnContinue()
        {
            //eventLog.WriteEntry("In OnContinue", EventLogEntryType.Information);
            Console.WriteLine("In OnContinue");
        }  

        protected override void OnStop()
        {
            //eventLog.WriteEntry("In OnStop", EventLogEntryType.Information);
            Console.WriteLine("In OnStop");
            shutdownEvent.Set();
            if (!thread.Join(Properties.Settings.Default.AbortIntervalSeconds * 1000))
            { 
                thread.Abort();
            }
        }
    }
}
