
using OpenQA.Selenium.Remote;

namespace NUnitReporter.Reporting.Helpers
{
    /// <summary>
    /// Interface indicates that reporter helper can use <see cref="RemoteWebDriver"/> instance during logging (make web page captures, ...).
    /// </summary>
    public interface ISeleniumReporter
    {
        RemoteWebDriver WebDriver { get; set; }
    }


}
