using System;
using System.Globalization;
using System.IO;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using NUnitReporter.Reporting;
using NUnitReporter.Reporting.Helpers;


namespace NUnitReporter
{
    /// <summary>
    /// A connector for NUnit as add-in.
    /// </summary>
    [NUnitAddinAttribute(Type = ExtensionType.Core, Name = "NUnit Reporter", Description = "Add-in for reports generation")]
    public class Listener : IAddin, EventListener
    {
        public bool Install(IExtensionHost host)
        {
            IExtensionPoint listeners = host.GetExtensionPoint("EventListeners");
            if (listeners == null)
                return false;

            listeners.Install(this);
            return true;
        }

        #region EventListener Members
        public void RunStarted(string name, int testCount)
        {
            try
            {
                Reporter.AddReporter(new ConsoleReporterHelper());
                Reporter.AddReporter(new HtmlReporterHelper());

                Reporter.SetProperty(ReporterHelperProperties.WorkingDirectory, Path.Combine(Path.Combine(Utilities.GetProjectDirectory(), ".reports"), "html"));
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
        }

        public void RunFinished(TestResult result)
        {
            try
            {
                Reporter.RemoveReporters();
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            } 
        }

        public void RunFinished(Exception exception)
        {
        }

        public void TestStarted(TestName testName)
        {
            try
            {
                Reporter.InitTestReporting();
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
        }

        public void TestFinished(TestResult result)
        {
            try
            {
                Reporter.SetProperty(ReporterHelperProperties.TestName, string.IsNullOrEmpty(result.Description) ? result.Name : result.Description);
                Reporter.SetProperty(ReporterHelperProperties.TestDuration, Math.Round(result.Time).ToString(CultureInfo.InvariantCulture));

                TestStatus status;
                if (result.IsError)
                {
                    status = TestStatus.Failed;
                    Reporter.Log(MessageTypes.Failed, result.Message ?? "Test failed!");
                    Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                }
                else if (result.IsSuccess)
                {
                    status = TestStatus.Passed;
                }
                else
                {
                    status = TestStatus.Skipped;
                    Reporter.Log(MessageTypes.Skipped, result.Message ?? "Test skipped.");
                    Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                }
                Reporter.SetProperty(ReporterHelperProperties.TestStatus, status.ToString());

                Reporter.FinishTestReporting();
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
        }

        public void SuiteStarted(TestName testName)
        {
//            Console.WriteLine("Suite name: {0} || {1} || {2}", testName.Name, testName.FullName, testName.UniqueName);
        }

        public void SuiteFinished(TestResult result)
        {

        }

        public void UnhandledException(Exception exception)
        {

        }

        public void TestOutput(TestOutput testOutput) 
        { 
            // DO NOT produce any console outputs here! 
        } 
        #endregion

        private static void WarnAboutException(Exception e)
        {

            Console.WriteLine(e);
        }
    }
}
