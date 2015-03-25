#region Usages

using System.Reflection;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;

#endregion

namespace NUnitReporter
{
    /// <summary>
    /// A connector for NUnit as add-in.
    /// </summary>
    [NUnitAddin(Type = ExtensionType.Core, Name = "NUnit Reporter", Description = "Add-in for reports generation")]
    public class CustomDecorator : IAddin, ITestDecorator
    {
        public bool Install(IExtensionHost host)
        {
            IExtensionPoint listeners = host.GetExtensionPoint("TestDecorators");
            if (listeners == null)
                return false;

            listeners.Install(this);
            return true;
        }

        public Test Decorate(Test test, MemberInfo member)
        {
            if (test.GetType() == typeof(NUnitTestMethod) &&
                ((NUnitTestMethod)test).Method.
                    GetCustomAttributes(typeof(IgnoreAttribute), true).
                    Length == 0)
            {
                return new TestMethodWrapper((NUnitTestMethod)test);
            }
            return test;
        }
    }
}
