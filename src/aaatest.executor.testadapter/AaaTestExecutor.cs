using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace aaatest.executor.testadapter
{
    internal class AssertionDetectorPlugin : IFailureDetector
    {
        public bool IsAssertionFailure(Exception ex)
        {
            return ex.GetType().Name.Contains("Assertion");
        }
    }

    [ExtensionUri(ExecutorUrl)]
    [UsedImplicitly]
    public class AaaTestExecutor : ITestExecutor
    {
        public const string ExecutorUrl = "executor://AaaTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUrl);
        private readonly AaaTestDiscoverer _testDiscoverer = new AaaTestDiscoverer();

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tracer = new MessageLoggerTracer(frameworkHandle);
            var executor = new Executor(tracer, new AssertionDetectorPlugin());


            foreach (var test in tests)
            {
                frameworkHandle.RecordStart(test);
                var result = executor.Execute(_testDiscoverer.GetTest(test.FullyQualifiedName)).Result;

                frameworkHandle.RecordEnd(test, result.Outcome.ToVsResultOutcome());
                frameworkHandle.RecordResult(result.ConvertToVsResult(test));
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var sink = new DiscoverySink();

            _testDiscoverer.DiscoverTests(sources, null, frameworkHandle, sink);

            RunTests(sink.Tests, runContext, frameworkHandle);
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }
}