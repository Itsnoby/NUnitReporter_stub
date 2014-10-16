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

        /// <summary>
        /// Get attributes from method by its name and name of the class where the method is described.
        /// </summary>
        /// <typeparam name="T">The attribute type which to extract from the method.</typeparam>
        /// <param name="classTypeName">The full name of the class.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <returns></returns>
        static internal Attribute[] ExtractAttribute<T>(string classTypeName, string methodName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblyType in assemblies.Select(assembly => assembly.GetType(classTypeName))
                .Where(assemblyType => assemblyType != null))
            {
                return assemblyType.GetMethod(methodName).GetCustomAttributes(typeof(T), false) as Attribute[];
            }

            return null;
        }

        /// <summary>
        /// Determine if the given name is class or package full name.
        /// </summary>
        /// <param name="classTypeName">The full name.</param>
        /// <returns><c>True</c> if the given name is the name of some class or package.</returns>
        static internal bool IsClassExists(string classTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Select(assembly => assembly.GetType(classTypeName)).Any(assemblyType => assemblyType != null);
        }
    }
}
