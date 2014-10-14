
namespace NUnitReporter.Reporting.Helpers
{
    public interface IReporterHelper
    {
        void Init();

        void AddProperty(ReporterHelperProperties name, string value);

        void Log(MessageTypes type, string message);

        void Log(string message);

        void ClearLog();

        void Finish();
    }

    public enum ReporterHelperProperties
    {
        TestName,
        TestDuration,
        TestStatus,
        SuiteName,
        WorkingDirectory
    }
}
