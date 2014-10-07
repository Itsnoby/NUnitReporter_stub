
namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// Interface indicates that reporter helper can log additional information (include images, stack trace).
    /// </summary>
    interface IReporterHelperExtended : IReporterHelper
    {
        void Log(InternalMessageTypes type, string message);
    }

    public enum InternalMessageTypes { StackTrace, Image };
}
