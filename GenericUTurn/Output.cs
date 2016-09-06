using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenericUTurn
{
    public static class Output
    {

        private static StringWriter writer = new StringWriter();

        private static string Stamp(string level)
        {
            return "[" + Environment.MachineName + " " + DateTime.Now.ToString("yyyyMMddHHmmss") + " " + level + "] ";
        }

        public static void Info(string message)
        {
            writer.WriteLine(Stamp("info") + message);
            Console.Out.WriteLine(message);
        }

        public static void Error(string message)
        {
            writer.WriteLine(Stamp("ERROR") + message);
            Console.Error.WriteLine(message);
        }

        public static void Error(string message, Exception ex)
        {
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
        }

        public static void Warn(string message)
        {
            writer.WriteLine(Stamp("warn") + message);
            Console.Error.WriteLine(message);
        }

        public static void Warn(string message, Exception ex)
        {
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
        }


        public static String ToString()
        {
            return writer.ToString();
        }

        public static void Write(FileInfo logfile)
        {
            var output = logfile.AppendText();
            output.Write(writer);
            output.Close();
            // clear everything
            writer = new StringWriter();
        }
    }
}
