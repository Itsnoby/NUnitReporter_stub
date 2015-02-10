#region Usages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnitReporter.Reporting.Helpers;
using NUnitReporter.Reporting.Helpers.Ext;

#endregion

namespace NUnitReporter.Reporting
{
    /// <summary>
    /// Static class for tests logging. Reporter manages reporting processes, initializes and finalizes connected reporter helpers.
    /// </summary>
    public static class Reporter
    {
        private static long _id = Environment.TickCount;

        private static List<IReporterHelper> _reporters = new List<IReporterHelper>();
        public static List<IReporterHelper> Reporters
        {
            get { return _reporters; }
        }

        private static List<IReporterHelperExtension> _extensions = new List<IReporterHelperExtension>();
        public static List<IReporterHelperExtension> ReporterHelperExtensions
        {
            get { return _extensions; }
        }

        private static bool _testReportingInitializedBefore, _suiteReportingInitializedBefore;

        /// <summary>
        /// Connect <see cref="IReporterHelperExtension"/> to reporting.
        /// Adds only the one helper extension of the same type.
        /// </summary>
        /// <param name="extension">New helper extension.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddHelperExtension(IReporterHelperExtension extension)
        {
            var extensionPresent = false;
            foreach (var ext in _extensions.Where(reporter => reporter.GetType() == extension.GetType()))
            {
                extensionPresent = true;
            }
            if (!extensionPresent)
                _extensions.Add(extension);
        }


        /// <summary>
        /// Notify all connected reporters about suite execution. The method can be used once, the second method call does nothing.
        /// To notify reporter helpers again <see cref="Reporter.FinishSuiteReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void InitSuiteReporting()
        {
            if (_suiteReportingInitializedBefore)
                return;

            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.SuiteLogInit();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during suite log initialization! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }

            _suiteReportingInitializedBefore = true;
        }

        /// <summary>
        /// Notify all connected reporters that suite execution is finished. The method can be used once, the second method call does nothing.
        /// To notify reporter helpers again <see cref="Reporter.InitSuiteReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void FinishSuiteReporting()
        {
            if (!_suiteReportingInitializedBefore)
                return;

            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.SuiteLogFinish();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during suite log finalization! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }

            _suiteReportingInitializedBefore = false;
        }

        /// <summary>
        /// Initialize all connected reporters before each test execution. The method can be used once, the second method call does nothing.
        /// To initialize reporter helpers again <see cref="Reporter.FinishTestReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InitTestReporting()
        {
            if (_testReportingInitializedBefore)
                return;

            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.TestLogInit();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during tests log initialization! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }

            _testReportingInitializedBefore = true;
        }

        /// <summary>
        /// Finalize all connected reporters, save their results after test execution. The method can be used once, the second method call does nothing.
        /// To finalize reporter helpers again <see cref="Reporter.InitTestReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void FinishTestReporting()
        {
            if (!_testReportingInitializedBefore)
                return;

            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.TestLogFinish();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during tests log finalization! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }

            _testReportingInitializedBefore = false;
        }

        /// <summary>
        /// Set configuration property for connected reporters. To unset property set it with value <c>null</c>.
        /// </summary>
        /// <param name="name">Reporter helpers' property name</param>
        /// <param name="value">Property value</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetProperty(ReporterHelperProperties name, string value)
        {
            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.AddProperty(name, value);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during adding property [{1};{2}]! ({0})",
                        reporterHelper.GetType().FullName, name, value);
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Add a message to reports.
        /// </summary>
        /// <param name="message">A <see cref="MessageTypes.Standard"/>-type message</param>
        public static void Log(string message)
        {
            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.Log(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during logging! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Add a message of some type to reports.
        /// </summary>
        /// <param name="type">Type of the message. Some reporter helpers may interact with different messages in special way.</param>
        /// <param name="message">Message text</param>
        public static void Log(MessageTypes type, string message)
        {
            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.Log(type, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during logging! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Add a special message. Used by <see cref="Reporter"/> only to log stack trace or exception.
        /// </summary>
        /// <param name="type">Internal message type</param>
        /// <param name="message">HTML code of image, stack trace etc.</param>
        internal static void Log(InternalMessageTypes type, string message)
        {
            foreach (var reporterHelper in _reporters.OfType<IReporterHelperExtended>())
            {
                try
                {
                    reporterHelper.Log(type, message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception occurred during logging! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(e);
                }
            }
        }

        public static void Log(Exception e)
        {
            foreach (var reporterHelper in _reporters)
            {
                try
                {
                    reporterHelper.Log(e);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occurred during logging! ({0})", reporterHelper.GetType().FullName);
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Connect <see cref="IReporterHelper"/> to reporting. Usage of the <see cref="Reporter.InitTestReporting"/> method after adding the helper is mandatory.
        /// Adds only the one reporter helper of the same type.
        /// </summary>
        /// <param name="reporterHelper">New helper.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddReporter(IReporterHelper reporterHelper)
        {
            var reporterPresent = false;
            foreach (var reporter in _reporters.Where(reporter => reporter.GetType() == reporterHelper.GetType()))
            {
                reporterPresent = true;
            }
            if (!reporterPresent)
                _reporters.Add(reporterHelper);
        }

        /// <summary>
        /// Unconnect all connected reporter helpers, dispose resources if needed.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveReporters()
        {
            foreach (var reporterHelper in _reporters.Where(rh => rh is IDisposable))
            {
                (reporterHelper as IDisposable).Dispose();    
            }
            _reporters.Clear();

            _testReportingInitializedBefore = false;
        }
    }
}
