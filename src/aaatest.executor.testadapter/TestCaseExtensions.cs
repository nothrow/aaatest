using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace aaatest.executor.testadapter
{
    internal static class TestCaseExtensions
    {
        public static TestCase ConvertToVsTest(this framework.TestCase tc, string sourceAssembly)
        {
            return new TestCase(tc.Identifier, AaaTestExecutor.ExecutorUri, sourceAssembly)
            {

            };
        }
    }
}