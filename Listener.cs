#region Usages

using System;
using System.Globalization;
using System.IO;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using NUnitReporter.Reporting;
using NUnitReporter.Reporting.Helpers;

#endregion

namespace NUnitReporter
{
    /// <summary>
    /// A connector for NUnit as add-in.
    /// </summary>
    [NUnitAddin(Type = ExtensionType.Core, Name = "NUnit Reporter", Description = "Add-in for reports generation")]
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

                Reporter.SetProperty(ReporterHelperProperties.WorkingDirectory, Path.Combine(Path.Combine(Utilities.GetAssemblyDirectory(), ".reports"), "html"));
                Reporter.SetProperty(ReporterHelperProperties.SuiteName, "Suite Execution Results"); // TODO need somehow get suite name

                Reporter.InitSuiteReporting();
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
                Reporter.FinishSuiteReporting();
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
                Reporter.SetProperty(ReporterHelperProperties.TestName,
                    string.IsNullOrEmpty(result.Description) ? result.Name : result.Description);
                Reporter.SetProperty(ReporterHelperProperties.TestDuration,
                    Math.Round(result.Time).ToString(CultureInfo.InvariantCulture));

                TestStatus status;
                // if internal exception or assertion exception
                if (result.IsError || result.IsFailure)
                {
                    status = TestStatus.Failed;
                    Reporter.Log(MessageTypes.Failed, result.Message ?? "Test failed!");
                    Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                }
                    // if success
                else if (result.IsSuccess)
                {
                    status = TestStatus.Passed;
                }
                    // neither exception nor success - test case was skipped
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
            finally
            {
                Reporter.SetSeleniumDriver(null);
            }
        }

        public void SuiteStarted(TestName testName)
        {
            Reporter.SetProperty(ReporterHelperProperties.TestSuiteName, testName.Name);
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
