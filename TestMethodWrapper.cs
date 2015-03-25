﻿using NUnit.Core;

namespace NUnitReporter
{
    class TestMethodWrapper : NUnitTestMethod
    {
        private Test _test;

        public TestMethodWrapper(NUnitTestMethod testMethod) :
            base(testMethod.Method)
        {
            _test = testMethod;
        }

        public override TestResult Run(EventListener listener,
                                                   ITestFilter filter)
        {
            return base.Run(new CustomEventListener(listener), filter);
        }
    }
}
