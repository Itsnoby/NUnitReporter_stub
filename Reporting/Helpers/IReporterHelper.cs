﻿
namespace NUnitReporter.Reporting.Helpers
{
    public interface IReporterHelper
    {
        void SuiteLogInit();

        void TestLogInit();

        void AddProperty(ReporterHelperProperties name, string value);

        void Log(MessageTypes type, string message);

        void Log(string message);

        void ClearLog();

        void TestLogFinish();

        void SuiteLogFinish();
    }

    public enum ReporterHelperProperties
    {
        TestName,
        TestDuration,
        TestStatus,
        TestSuiteName,
        SuiteName,
        WorkingDirectory
    }
}
