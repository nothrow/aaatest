using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace aaatest.executor.testadapter
{
    [DefaultExecutorUri("executor://AaaTestExecutor")]
    public class AaaTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            discoverySink.SendTestCase(new TestCase("AHOJ", new Uri("executor://AaaTestExecutor"), "ahoj"));
        }
    }
}