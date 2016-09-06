using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn
{
    /*
    class Program
    {

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            Output.Info("Working directory: " + System.IO.Directory.GetCurrentDirectory());
        
            while (42 == 42) {
                Output.Info("*** Starting loop ***");
                var changes = 0;

                GenericUTurn uturn = new GenericUTurn();
                foreach (Zaaktype zaaktype in uturn.GetTypes())
                {
                    changes += uturn.Synchronize(zaaktype);
                }
                if (changes == 0) {
                    var timeout = Properties.Settings.Default.UTurnSleepInterval;
                    Output.Info("*** Sleeping for: " + timeout + " seconds ***");
                    System.Threading.Thread.Sleep(timeout * 1000);
                }
            }
        }
    }
     */
}
