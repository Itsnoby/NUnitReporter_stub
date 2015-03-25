using System;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using NUnitReporter.Reporting;
using NUnitReporter.Reporting.Description;
using NUnitReporter.Reporting.Helpers;

namespace NUnitReporter
{
    class CustomEventListener : EventListener
    {
        private EventListener _eventListener;

        public CustomEventListener(EventListener eventListener)
        {
            _eventListener = eventListener;
        }
        #region EventListener Members
        public void RunStarted(string name, int testCount)
        {
            if (_eventListener != null)
            {
                _eventListener.RunStarted(name, testCount);
                return;
            }
            try
            {
                Reporter.AddReporter(new ConsoleReporterHelper());
                Reporter.AddReporter(new HtmlReporterHelper());

                Reporter.SetProperty(ReporterHelperProperties.WorkingDirectory, Path.Combine(Path.Combine(Utilities.GetAssemblyDirectory(), ".reports"), "html"));
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
        }

        public void RunFinished(TestResult result)
        {
            if (_eventListener != null)
            {
                _eventListener.RunFinished(result);
                return;
            }
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
            if (_eventListener != null)
            {
                _eventListener.RunFinished(exception);
                return;
            }
        }

        public void TestStarted(TestName testName)
        {
            if (_eventListener != null)
            {
                _eventListener.TestStarted(testName);
                return;
            }
            Console.WriteLine("Test started for " + testName.FullName);
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
            if (_eventListener != null)
            {
                _eventListener.TestFinished(result);
                return;
            }
            Console.WriteLine("Test finished for " + result.Test.ClassName);
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
                    String.IsNullOrEmpty(testDescription.ToString()) ? result.Name : testDescription.ToString());

                #endregion

                Reporter.SetProperty(ReporterHelperProperties.TestDuration,
                    Math.Round(result.Time).ToString(CultureInfo.InvariantCulture));

                #region Determine test status

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
                // neither exception nor success - the test was skipped
                else
                {
                    status = TestStatus.Skipped;
                    Reporter.Log(MessageTypes.Skipped, result.Message ?? "Test skipped.");
                    Reporter.Log(InternalMessageTypes.StackTrace, result.StackTrace);
                }
                Reporter.SetProperty(ReporterHelperProperties.TestStatus, status.ToString());

                #endregion

                Reporter.FinishTestReporting();
            }
            catch (Exception e)
            {
                WarnAboutException(e);
            }
        }

        public void SuiteStarted(TestName testName)
        {
            if (_eventListener != null)
            {
                _eventListener.SuiteStarted(testName);
                return;
            }
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
            if (_eventListener != null)
            {
                _eventListener.SuiteFinished(result);
                return;
            }
        }

        public void UnhandledException(Exception exception)
        {
            if (_eventListener != null)
            {
                _eventListener.UnhandledException(exception);
                return;
            }
        }

        public void TestOutput(TestOutput testOutput)
        {
            if (_eventListener != null)
            {
                _eventListener.TestOutput(testOutput);
                return;
            }
            // DO NOT produce any console outputs here! 
        }
        #endregion

        private static void WarnAboutException(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
