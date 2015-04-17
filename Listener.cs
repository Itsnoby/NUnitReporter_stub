#region Usages

using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;
using NUnitReporter.Reporting;
using NUnitReporter.Reporting.Description;
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
        private int _testsCount, _currentTest;
        private DateTime _startTime;

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
            _testsCount = testCount;

            try
            {
                Reporter.AddReporter(new ConsoleReporterHelper());
                Reporter.AddReporter(new HtmlReporterHelper());

                Reporter.SetProperty(ReporterHelperProperties.WorkingDirectory, Path.Combine(Utilities.GetAssemblyDirectory(), ".reports"));
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
            Console.WriteLine(
                "[{0}]\tStarted test ({1} of {2}): {3}", 
                string.Format("{0:HH:mm:ss.fff}", _startTime = DateTime.Now), ++_currentTest, _testsCount, testName.FullName);
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
            var status = DetermineStatus(result);

            Console.WriteLine("[{0}]\tFinished test: {1}, result: {2}, duration: {3:g}",
                string.Format("{0:HH:mm:ss.fff}", DateTime.Now), result.Test.ClassName, status, DateTime.Now - _startTime);
            try
            {
                #region Get test description

                var testDescription = new StringBuilder();

                // try to get description from the own attribute
                var descriptionDetails = (DescriptionDetailsAttribute[])Utilities.ExtractAttribute<DescriptionDetailsAttribute>(result.Test.ClassName, result.Test.MethodName);
                if (descriptionDetails != null && descriptionDetails.Length > 0)
                {
                    foreach (var description in descriptionDetails)
                        testDescription.Append(description.Description);
                }
                // or get it manually from NUnit attribute
                else
                {
                    var nunitDetails = (DescriptionAttribute[])Utilities.ExtractAttribute<DescriptionAttribute>(result.Test.ClassName, result.Test.MethodName);
                    foreach (var description in nunitDetails)
                        testDescription.Append(description.Description);
                }

                Reporter.SetProperty(ReporterHelperProperties.TestTitle,
                    string.IsNullOrEmpty(testDescription.ToString()) ? result.Name : testDescription.ToString());

                #endregion

                Reporter.SetProperty(ReporterHelperProperties.TestDuration,
                    Math.Round(result.Time).ToString(CultureInfo.InvariantCulture));

                switch (status)
                {
                    case TestStatus.Failed:
                        Reporter.Log(MessageTypes.Failed, result.Message ?? "Test failed!");
                        Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                        break;
                    case TestStatus.Skipped:
                        Reporter.Log(MessageTypes.Skipped, result.Message ?? "Test skipped.");
                        Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                        break;
                    case TestStatus.Passed:
                        break;
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
            try
            {
                // set test class name and do not set when it is parametrized test method (we need class name but not method name)
                if (Utilities.IsClassExists(testName.FullName))
                {
                    Reporter.SetProperty(ReporterHelperProperties.TestClassName, testName.Name);
                }

                // get suite description attribute
                var descriptionDetails = (DescriptionDetailsAttribute[])Utilities.ExtractAttribute<DescriptionDetailsAttribute>(testName.FullName);
                if (descriptionDetails != null && descriptionDetails.Length > 0)
                {
                    var fullDescription = new StringBuilder();
                    foreach (var description in descriptionDetails)
                    {
                        fullDescription.Append(description.Description);
                    }
                    Reporter.SetProperty(ReporterHelperProperties.SuiteTitle, fullDescription.ToString());
                }

                Reporter.InitSuiteReporting();
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
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

        private static TestStatus DetermineStatus(TestResult result)
        {
            if (result.IsError || result.IsFailure)
                return TestStatus.Failed;
            return result.IsSuccess ? TestStatus.Passed : TestStatus.Skipped;
        }
    }
}
