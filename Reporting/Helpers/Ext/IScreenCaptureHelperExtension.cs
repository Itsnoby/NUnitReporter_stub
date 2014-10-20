
namespace NUnitReporter.Reporting.Helpers.Ext
{
    public interface IScreenCaptureHelperExtension : IReporterHelperExtension
    {
        /// <summary>
        /// Make screenshot and save it to the folder which is specified.
        /// </summary>
        /// <param name="imagePath">The folder where image should be saved.</param>
        /// <returns>The name of the created file or <c>null</c> if the image was not saved.</returns>
        string MakeScreenshot(string imagePath);
    }
}
