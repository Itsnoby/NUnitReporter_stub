#region Usages

using System;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace NUnitReporter
{
    class Utilities
    {
        static public string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Get attributes from class.
        /// </summary>
        /// <typeparam name="T">The attribute type which to extract from the class.</typeparam>
        /// <param name="classTypeName">The full name of the class.</param>
        /// <returns></returns>
        static internal Attribute[] ExtractAttribute<T>(string classTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblyType in assemblies.Select(assembly => assembly.GetType(classTypeName))
                .Where(assemblyType => assemblyType != null))
            {
                return Attribute.GetCustomAttributes(assemblyType, typeof(T));
            }

            return null;
        }
    }
}
