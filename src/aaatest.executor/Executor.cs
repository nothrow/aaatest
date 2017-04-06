using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using aaatest.framework;

namespace aaatest.executor
{
    public class Executor
    {
        private readonly ConcurrentDictionary<Type, Func<TestCase, ValueTask<TestExecutionResult>>> _executors
            = new ConcurrentDictionary<Type, Func<TestCase, ValueTask<TestExecutionResult>>>();

        private readonly IFailureDetector _failureDetector;

        private readonly ITracer _tracer;

        public Executor(ITracer tracer, IFailureDetector failureDetector)
        {
            _tracer = tracer;
            _failureDetector = failureDetector;
        }

        public ValueTask<TestExecutionResult> Execute(TestCase test, CancellationToken token = default(CancellationToken))
        {
            using (
                _tracer.MeasureTrace($"Executing {test.Name}",
                    executed => $"{test.Name} finished, took {executed.TotalSeconds:0.000}s"))
            {
                var testExecutor = _executors.GetOrAdd(test.GetType(), GenerateExecutor);
                token.ThrowIfCancellationRequested();
                return testExecutor(test);
            }
        }

        private Func<TestCase, ValueTask<TestExecutionResult>> GenerateExecutor(Type testCaseType)
        {
            var executeSpecificTestMethod = typeof(Executor).GetMethod(nameof(ExecuteSpecificTestAsync),
                BindingFlags.NonPublic | BindingFlags.Instance);

            var executeSpecificMethodGeneric =
                executeSpecificTestMethod.MakeGenericMethod(testCaseType.GetGenericArguments());

            return
                (Func<TestCase, ValueTask<TestExecutionResult>>)
                executeSpecificMethodGeneric.CreateDelegate(typeof(Func<TestCase, ValueTask<TestExecutionResult>>), this);
        }

        private ValueTask<TestExecutionResult> ExecuteSpecificTestAsync<TClass, TActResult>(
            TestCase testCaseNonGeneric)
        {
            return
                new ValueTask<TestExecutionResult>(ExecuteSpecificTest((TestCase<TClass, TActResult>) testCaseNonGeneric));
        }


        private TestExecutionResult ExecuteSpecificTest<TClass, TActResult>(
            TestCase<TClass, TActResult> testCase)
        {
            var sw = Stopwatch.StartNew();
            var context = new TestingContext<TClass>();
            TActResult result;

            try
            {
                var instance = testCase.Arrange(context);
                result = testCase.Act.Compile()(instance);
            }
            catch (Exception ex)
            {
                return new TestExecutionResult(testCase, TestExecutionOutcome.Invalid, ex, sw.Elapsed);
            }

            try
            {
                testCase.Assert(result);
            }
            catch (Exception ex)
            {
                var isAssertionIssue = _failureDetector.IsAssertionFailure(ex);
                return new TestExecutionResult(testCase,
                    isAssertionIssue ? TestExecutionOutcome.Failure : TestExecutionOutcome.Invalid, ex, sw.Elapsed);
            }

            return new TestExecutionResult(testCase, TestExecutionOutcome.Success, null, sw.Elapsed);
        }
    }
}