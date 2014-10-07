using System;
using System.IO;
using System.Reflection;

namespace NUnitReporter
{
    class Utilities
    {
        static public string GetProjectDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
