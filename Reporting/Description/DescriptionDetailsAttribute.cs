using System;

namespace NUnitReporter.Reporting.Description
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DescriptionDetailsAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionDetailsAttribute(string description)
        {
            Description = description;
        }
    }
}
