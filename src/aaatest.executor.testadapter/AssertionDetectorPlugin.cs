using System;

namespace aaatest.executor.testadapter
{
    internal class AssertionDetectorPlugin : IFailureDetector
    {
        public bool IsAssertionFailure(Exception ex)
        {
            return ex.GetType().Name.Contains("Assertion");
        }
    }
}