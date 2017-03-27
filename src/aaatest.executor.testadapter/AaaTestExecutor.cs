using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace aaatest.executor.testadapter
{
    [ExtensionUri(ExecutorUrl)]
    [UsedImplicitly]
    public class AaaTestExecutor : ITestExecutor
    {
        public const string ExecutorUrl = "executor://AaaTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUrl);

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            foreach (var test in tests)
            {
                frameworkHandle.RecordResult(new TestResult(test) { Outcome =  TestOutcome.Passed });
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var testDiscoverer = new AaaTestDiscoverer();
            var sink = new DiscoverySink();

            testDiscoverer.DiscoverTests(sources, null, frameworkHandle, sink);

            RunTests(sink.Tests, runContext, frameworkHandle);
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }
}