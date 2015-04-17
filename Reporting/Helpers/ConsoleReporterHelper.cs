#region Usages

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

#endregion

namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// A helper for logging to console.
    /// </summary>
    public class ConsoleReporterHelper : IReporterHelper
    {
        private const string LogName = "tests-log.txt";
        private string _workingDirectory = Utilities.GetAssemblyDirectory();
        private StreamWriter _log;

        public void SuiteLogInit() { }

        public void TestLogInit()
        {
            if (_log != null) return;

            if (!Directory.Exists(_workingDirectory))
                Directory.CreateDirectory(_workingDirectory);
            _log = new StreamWriter(Path.Combine(_workingDirectory, LogName));
        }

        public void AddProperty(ReporterHelperProperties name, string value)
        {
            switch (name)
            {
                case ReporterHelperProperties.TestTitle:
                    Log(string.Format("[*****] Finished test: '{0}'.", value));
                    break;
                case ReporterHelperProperties.TestDuration:
                    Log(string.Format("[*****] Test executed in {0} seconds.", value));
                    break;
                case ReporterHelperProperties.WorkingDirectory:
                    _workingDirectory = string.IsNullOrEmpty(value) ? Utilities.GetAssemblyDirectory() : value;
                    break;
            }
        }

        public void Log(MessageTypes type, string message)
        {
            var msg = new StringBuilder();
            msg.Append(string.Format("[{0}]\t", string.Format("{0:HH:mm:ss.fff}", DateTime.Now)));
            switch (type)
            {
                case MessageTypes.Standard:
                    break;
                case MessageTypes.ActionTitle:
                    msg.Append("\t");
                    break;
                case MessageTypes.Skipped:
                    msg.Append("[SKIPPED] ");
                    break;
                case MessageTypes.Failed:
                    msg.Append("[FAILED] ");
                    break;
            }
            msg.Append(message);

            Debug.WriteLine(msg);
            if (_log != null)
                _log.WriteLine(msg);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Log(string message)
        {
            Log(MessageTypes.Standard, message);
        }

        public void Log(Exception e)
        {
            Log(MessageTypes.Failed, e.Message);
        }

        public void ClearLog()
        {
            try
            {
                _log.Close();
                File.Delete(Path.Combine(_workingDirectory, LogName));
            }
            catch { }
        }

        public void TestLogFinish() { }

        public void SuiteLogFinish()
        {
            _log.Close();
            _log = null;
        }
    }
}
