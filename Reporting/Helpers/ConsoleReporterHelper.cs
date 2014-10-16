#region Usages

using System;
using System.Text;

#endregion

namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// A helper for logging to console.
    /// </summary>
    public class ConsoleReporterHelper : IReporterHelper
    {
        public void SuiteLogInit(){ }

        public void TestLogInit() { }

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

            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Log(string message)
        {
            Log(MessageTypes.Standard, message);
        }

        public void ClearLog() { }

        public void TestLogFinish() { }
        public void SuiteLogFinish() { }
    }
}
