#region Usages

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using NUnitReporter.Reporting.Generators;
using NUnitReporter.Reporting.Helpers.Ext;

#endregion

namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// A helper for reports generation in HTML format.
    /// </summary>
    public class HtmlReporterHelper : IReporterHelperExtended
    {
        private TestResultGenerator _testResultGenerator;
        private SuiteResultGenerator _suiteContext;
        private ExtendedProperties _properties;
        private TextWriter _testWriter;

        private List<HtmlReporterHelperMessage> _log;

        private SuiteResults _suiteResults;
        private TestResults _currentTestResults;

        #region Directories
        private const string TestsResultsDir = "tests";
        private const string ResDir = "res";

        private string GetWorkPath()
        {
            var workPath = _properties.GetString(ReporterHelperProperties.WorkingDirectory.ToString(), Utilities.GetAssemblyDirectory());
            if (!Directory.Exists(workPath))
                Directory.CreateDirectory(workPath);
            return workPath;
        }

        private string GetTestsFilesPath()
        {
            var path = Path.Combine(GetWorkPath(), TestsResultsDir);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private string GetResFilesPath()
        {
            var path = Path.Combine(GetTestsFilesPath(), ResDir);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        } 
        #endregion


        public HtmlReporterHelper()
        {
            _properties = new ExtendedProperties();
            _log = new List<HtmlReporterHelperMessage>();
        }

        public void SuiteLogInit()
        {
            _suiteContext = new SuiteResultGenerator();
            _suiteResults = new SuiteResults(_properties.GetString(ReporterHelperProperties.SuiteTitle.ToString(), "Suite Execution Results"));

        }

        public void TestLogInit()
        {
            _testResultGenerator = new TestResultGenerator();
            var filename = string.Format("{0}_{1:dd-MMMM-yyyy_HH-mm-ss-fff}.html",
                _properties.GetString(ReporterHelperProperties.TestClassName.ToString(), "TestResult"), DateTime.Now);
            var filePath = Path.Combine(GetTestsFilesPath(), filename);

            _currentTestResults = new TestResults(string.Format("{0}/{1}", TestsResultsDir, filename));
            _testWriter = new StreamWriter(filePath, false, new UnicodeEncoding());

        }

        public void AddProperty(ReporterHelperProperties name, string value)
        {
            if (_properties.Contains(name.ToString()))
            {
                _properties.Remove(name.ToString());
            }

            if (value != null)
            {
                _properties.Add(name.ToString(), value);
            }
        }

        #region Logging
        public void Log(MessageTypes type, string message)
        {
            _log.Add(new HtmlReporterHelperMessage(type, HttpUtility.HtmlEncode(message))); 
            if (type.Equals(MessageTypes.Failed))
            {
                foreach (var reporterHelperExtension in Reporter.ReporterHelperExtensions.OfType<IScreenCaptureHelperExtension>())
                {
                    var image = reporterHelperExtension.MakeScreenshot(GetResFilesPath());
                    if (image != null)
                        Log(InternalMessageTypes.Image, string.Format("{0}/{1}", ResDir, image));
                }
            }
        }

        public void Log(InternalMessageTypes type, string message)
        {
            _log.Add(new HtmlReporterHelperMessage(type, message.Replace(Environment.NewLine, "<br>")));
        }

        public void Log(string message)
        {
            Log(MessageTypes.Standard, message);
        }

        public void Log(Exception e)
        {
            Log(MessageTypes.Failed, e.Message);
            Log(InternalMessageTypes.StackTrace, e.StackTrace);
        }

        public void ClearLog()
        {
            _log.Clear();
        } 
        #endregion

        public void TestLogFinish()
        {
            _currentTestResults.Name = _properties.GetString(ReporterHelperProperties.TestTitle.ToString(), "Automated Test Case");
            _currentTestResults.Status = _properties.GetString(ReporterHelperProperties.TestStatus.ToString(), TestStatus.Passed.ToString());
            _currentTestResults.Duration = Convert.ToInt32(_properties.GetString(ReporterHelperProperties.TestDuration.ToString(), "0"));
            _suiteResults.AddResult(_currentTestResults);
            _testResultGenerator.Log = _log;
            _testResultGenerator.Name = _currentTestResults.Name;
            _testResultGenerator.Status = _currentTestResults.Status;
            _testResultGenerator.Duration = _currentTestResults.Duration;
            _testResultGenerator.CreateReport();
            _testWriter.Write(_testResultGenerator.Report);
            _testWriter.Dispose();

            // Restore reporter state
            _log.Clear();
            _testWriter = null;
            _properties.Remove(ReporterHelperProperties.TestTitle.ToString());
            _properties.Remove(ReporterHelperProperties.TestStatus.ToString());
            _properties.Remove(ReporterHelperProperties.TestDuration.ToString());
        }

        public void SuiteLogFinish()
        {
            var filename = string.Format("SuiteResults_{0:dd-MMMM-yyyy_HH-mm-ss-fff}.html",DateTime.Now);
            _suiteContext.SuiteResults = _suiteResults;
            _suiteContext.CreateReport();
            using (var suiteWriter = new StreamWriter(Path.Combine(GetWorkPath(), filename), false, new UnicodeEncoding()))
            {
                suiteWriter.Write(_suiteContext.Report);
            }
            _suiteResults = null;
        }
    }



    public class HtmlReporterHelperMessage
    {
        public DateTime Time { get; private set; }
        public string Type { get; private set; }
        public string Message { get; private set; }

        public string GetTime()
        {
            return string.Format("{0:HH:mm:ss.fff}", Time);
        }

        public HtmlReporterHelperMessage(object type, string message)
        {
            Time = DateTime.Now;
            Type = type.ToString();
            Message = message;
        }
    }

    [Serializable]
    public class TestResults
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int Duration { get; set; }

        public string FileLink { get; set; }

        public TestResults(string fileLink)
        {
            FileLink = fileLink;
        }
    }

    [Serializable]
    public class SuiteResults
    {
        public string Name { get; private set; }

        public SuiteResults(string name)
        {
            Name = name;
        }

        private List<TestResults> _results = new List<TestResults>();
        public List<TestResults> Results
        {
            get { return _results; }
        }

        public void AddResult(TestResults result)
        {
            _results.Add(result);
        }
    }
}
