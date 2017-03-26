using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace aaatest.executor.testadapter
{
    [ExtensionUri("executor://AaaTestExecutor")]
    public class AaaTestExecutor : ITestExecutor
    {
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //throw new System.NotImplementedException();
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //throw new System.NotImplementedException();
        }

        public void Cancel()
        {
            //throw new System.NotImplementedException();
        }
    }
}