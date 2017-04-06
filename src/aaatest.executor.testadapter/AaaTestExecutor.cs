using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly AaaTestDiscoverer _testDiscoverer;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public AaaTestExecutor()
        {
            _testDiscoverer = new AaaTestDiscoverer();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tracer = new MessageLoggerTracer(frameworkHandle);
            var executor = new Executor(tracer, new AssertionDetectorPlugin());
            var sink = new DiscoverySink();
            var testsArray = tests.ToArray();

            var sources = testsArray.Select(x => x.Source).Distinct();

            _testDiscoverer.DiscoverTests(sources, null, frameworkHandle, sink, _cancellationTokenSource.Token);

            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            ExecuteTests(testsArray, frameworkHandle, executor, _cancellationTokenSource.Token).Wait();
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tracer = new MessageLoggerTracer(frameworkHandle);
            var executor = new Executor(tracer, new AssertionDetectorPlugin());
            var sink = new DiscoverySink();

            _testDiscoverer.DiscoverTests(sources, null, frameworkHandle, sink, _cancellationTokenSource.Token);

            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            ExecuteTests(sink.Tests, frameworkHandle, executor, _cancellationTokenSource.Token).Wait();
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        private Task ExecuteTests(IEnumerable<TestCase> tests, ITestExecutionRecorder frameworkHandle, Executor executor, CancellationToken token)
        {
            return Task.WhenAll(tests.Select(test => Task.Run(async () =>
            {
                frameworkHandle.RecordStart(test);
                try
                {
                    var result = await executor.Execute(_testDiscoverer.GetTest(test.FullyQualifiedName), token);

                    frameworkHandle.RecordEnd(test, result.Outcome.ToVsResultOutcome());
                    frameworkHandle.RecordResult(result.ConvertToVsResult(test));
                }
                catch (Exception ex)
                {
                    frameworkHandle.RecordEnd(test, TestOutcome.None);
                    frameworkHandle.RecordResult(new TestResult(test)
                    {
                        Outcome = TestOutcome.None,
                        ErrorMessage = ex.Message,
                        ErrorStackTrace = ex.StackTrace
                    });
                }
            }, token)));
        }
    }
}
