
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NUnitReporter.Reporting.Helpers
{
    public interface IReporterHelper
    {
        void SuiteLogInit();

        void TestLogInit();

        void AddProperty(ReporterHelperProperties name, string value);

        void Log(MessageTypes type, string message);

        void Log(string message);

        void Log(Exception e);

        void ClearLog();

        void TestLogFinish();

        void SuiteLogFinish();
    }

    public enum ReporterHelperProperties
    {
        TestTitle,
        TestDuration,
        TestStatus,
        TestClassName,
        SuiteTitle,
        WorkingDirectory
    }
}
