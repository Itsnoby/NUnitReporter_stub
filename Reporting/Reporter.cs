using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OpenQA.Selenium.Remote;
using NUnitReporter.Reporting.Helpers;

namespace NUnitReporter.Reporting
{
    /// <summary>
    /// Static class for tests logging. Reporter manages reporting processes, initializes and finalizes connected reporter helpers.
    /// </summary>
    public static class Reporter
    {
        private static List<IReporterHelper> _reporters = new List<IReporterHelper>();
        public static List<IReporterHelper> Reporters
        {
            get { return _reporters; }
        }

        private static bool _initializedBeforeTest;

        /// <summary>
        /// Initialize all connected reporters before each test execution. The method can be used once, the second method call does nothing.
        /// To initialize reporter helpers again <see cref="Reporter.FinishTestReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InitTestReporting()
        {
            if (_initializedBeforeTest)
                return;

            foreach (var reporterHelper in _reporters)
            {
                reporterHelper.Init();
            }

            _initializedBeforeTest = true;
        }

        /// <summary>
        /// Finalize all connected reporters, save their results after test execution. The method can be used once, the second method call does nothing.
        /// To finalize reporter helpers again <see cref="Reporter.InitTestReporting"/> method should be executed before.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void FinishTestReporting()
        {
            if (!_initializedBeforeTest)
                return;

            foreach (var reporterHelper in _reporters)
            {
                reporterHelper.Finish();
            }

            _initializedBeforeTest = false;
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
                reporterHelper.AddProperty(name, value);
            }
        }

        /// <summary>
        /// Pass WebDriver object for connected reporters who can use it to make screen captures.
        /// </summary>
        /// <param name="driver">Active Selenium WebDriver instance. The driver object shout has 'takes skreenshot' capabilities enabled.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetSeleniumDriver(RemoteWebDriver driver)
        {
            foreach (var helper in _reporters.OfType<ISeleniumReporter>())
            {
                helper.WebDriver = driver;
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
                reporterHelper.Log(message);
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
                reporterHelper.Log(type, message);
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
                reporterHelper.Log(type, message);
            }
        }

        /// <summary>
        /// Connect <see cref="IReporterHelper"/> to reporting. Usage of the <see cref="Reporter.InitTestReporting"/> method after adding the helper is mandatory.
        /// </summary>
        /// <param name="reporterHelper">New helper.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddReporter(IReporterHelper reporterHelper)
        {
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

            _initializedBeforeTest = false;
        }
    }
}
