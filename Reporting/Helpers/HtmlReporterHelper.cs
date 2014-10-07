using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Commons.Collections;
using NUnit.Framework;
using NVelocity;
using NVelocity.App;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// A helper for reports generation in HTML format.
    /// </summary>
    public class HtmlReporterHelper : IReporterHelperExtended, ISeleniumReporter
    {
        private VelocityContext _context;
        private ExtendedProperties _properties;
        private TextWriter _writer;

        private List<HtmlReporterHelperMessage> _log;

        public RemoteWebDriver WebDriver { get; set; }

        public HtmlReporterHelper()
        {
            _context = new VelocityContext();
            _properties = new ExtendedProperties();
            _log = new List<HtmlReporterHelperMessage>();
        }


        public void Init()
        {
            var reportPath = _properties.GetString(ReporterHelperProperties.WorkingDirectory.ToString(), Utilities.GetProjectDirectory());
            if (!Directory.Exists(reportPath))
                Directory.CreateDirectory(reportPath);
            var filename = Path.Combine(reportPath, string.Format("TestResult_{0:dd-MMMM-yyyy_HH-mm-ss-fff}.html", DateTime.Now));

            _writer = _writer = new StreamWriter(filename, false, new UnicodeEncoding());

            Velocity.Init();
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

        public void Log(MessageTypes type, string message)
        {
            _log.Add(new HtmlReporterHelperMessage(type, message.Replace(Environment.NewLine, "<br>")));
            if (type.Equals(MessageTypes.Failed))
            {
                var imagePath = Path.Combine(_properties.GetString(ReporterHelperProperties.WorkingDirectory.ToString(), Utilities.GetProjectDirectory()), "images");
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);
                var image = MakeDriverScreenshot(imagePath);
                if (image != null)
                    Log(InternalMessageTypes.Image, string.Format("images/{0}", image));
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

        public void ClearLog()
        {
            _log.Clear();
        }

        public void Finish()
        {
            if (_writer == null) return;

            _context.Put("log", _log);
            _context.Put("name", _properties.GetString(ReporterHelperProperties.TestName.ToString(), "Automated Test Case"));
            _context.Put("status", _properties.GetString(ReporterHelperProperties.TestStatus.ToString(), TestStatus.Passed.ToString()));
            _context.Put("duration",  _properties.GetString(ReporterHelperProperties.TestDuration.ToString(), "0"));

            var template = Encoding.UTF8.GetString(Properties.Resources.test_result_html);
            Velocity.Evaluate(_context, _writer, "test_result_html", template);
            _writer.Dispose();

            // Restore reporter state
            _log.Clear();
            _writer = null;
            _properties.Remove(ReporterHelperProperties.TestName.ToString());
            _properties.Remove(ReporterHelperProperties.TestStatus.ToString());
            _properties.Remove(ReporterHelperProperties.TestDuration.ToString());
        }

        private string MakeDriverScreenshot(string imagePath)
        {
            if (WebDriver == null)
                return null;

            var screenshot = (WebDriver as ITakesScreenshot).GetScreenshot();
            var imageName = string.Format("screenshot{0}.png", Environment.TickCount);
            screenshot.SaveAsFile(Path.Combine(imagePath, imageName), ImageFormat.Png);
            return imageName;
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
}
