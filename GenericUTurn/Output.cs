using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenericUTurn
{
    public class Output
    {

        private StreamWriter writer = null;
        //private GenericUTurn genericUTurn;
        
        FileInfo logfile = null;

        public Output(FileInfo logfile)
        {
            this.logfile = logfile;
        }

        public Output(Object usedin)
        {
            this.logfile = new FileInfo(usedin.GetType().Name + ".log");
            // whether the StreamWriter will flush its buffer to the underlying stream after every call to StreamWriter.Write.            
        }

        private string Stamp(string level)
        {
            return "[" + Environment.MachineName + " " + DateTime.Now.ToString("yyyyMMddHHmmss") + " " + level + "] ";
        }

        public void Info(string message)
        {
            writer = logfile.AppendText();
            writer.AutoFlush = true;
            writer.WriteLine(Stamp("info") + message);
            writer.Close();
            Console.Out.WriteLine(message);
        }

        public void Error(string message)
        {
            writer = logfile.AppendText();
            writer.AutoFlush = true;
            writer.WriteLine(Stamp("ERROR") + message);
            writer.Close();
            Console.Error.WriteLine(message);
        }

        public void Error(string message, Exception ex)
        {
            writer = logfile.AppendText();
            writer.AutoFlush = true;

            writer.WriteLine(Stamp("ERROR") + message);
            Console.Error.WriteLine(message);

            writer.WriteLine("\tCaught exception of type:" + ex.GetType().FullName);
            Console.Error.WriteLine("\tCaught exception of type:" + ex.GetType().FullName);

            writer.WriteLine("\t" + ex.Message);
            Console.Error.WriteLine("\t" + ex.Message);

            Exception inner = ex.InnerException;
            while (inner != null)
            {
                writer.WriteLine("\t" + inner.Message);
                Console.Error.WriteLine("\t" + inner.Message);

                inner = inner.InnerException;
            }

            writer.WriteLine(ex.StackTrace);
            Console.Error.WriteLine(ex.StackTrace);

            writer.Close();
        }

        public void Warn(string message)
        {
            writer = logfile.AppendText();
            writer.AutoFlush = true;

            writer.WriteLine(Stamp("warn") + message);
            Console.Error.WriteLine(message);

            writer.Close();
        }

        public void Warn(string message, Exception ex)
        {
            writer = logfile.AppendText();
            writer.AutoFlush = true;

            writer.WriteLine(Stamp("warn") + message);
            Console.Error.WriteLine(message);

            writer.WriteLine("\tCaught exception of type:" + ex.GetType().FullName);
            Console.Error.WriteLine("\tCaught exception of type:" + ex.GetType().FullName);

            writer.WriteLine("\t" + ex.Message);
            Console.Error.WriteLine("\t" + ex.Message);

            Exception inner = ex.InnerException;
            while (inner != null)
            {
                writer.WriteLine("\t" + inner.Message);
                Console.Error.WriteLine("\t" + inner.Message);

                inner = inner.InnerException;
            }

            writer.WriteLine(ex.StackTrace);
            Console.Error.WriteLine(ex.StackTrace);

            writer.Close();
        }


        public override String ToString()
        {
            return writer.ToString();
        }

        //public void Write(FileInfo logfile)
        //{
        //}
    }
}