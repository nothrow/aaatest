using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using aaatest.framework;

namespace aaatest.executor
{
    public class Executor
    {
        private readonly ConcurrentDictionary<Type, Func<TestCase, ValueTask<TestExecutionResult>>> _executors
            = new ConcurrentDictionary<Type, Func<TestCase, ValueTask<TestExecutionResult>>>();

        private readonly ITracer _tracer;

        public Executor(ITracer tracer)
        {
            _tracer = tracer;
        }

        public ValueTask<TestExecutionResult> Execute(TestCase test)
        {
            using (
                _tracer.MeasureTrace($"Executing {test.Name}",
                    executed => $"{test.Name} finished, took {executed.TotalSeconds:0.000}s"))
            {
                return _executors.GetOrAdd(test.GetType(), GenerateExecutor)(test);
            }
        }

        private static Func<TestCase, ValueTask<TestExecutionResult>> GenerateExecutor(Type testCaseType)
        {
            var executeSpecificTestMethod = typeof(Executor).GetMethod(nameof(ExecuteSpecificTestAsync),
                BindingFlags.NonPublic | BindingFlags.Static);

            var executeSpecificMethodGeneric =
                executeSpecificTestMethod.MakeGenericMethod(testCaseType.GetGenericArguments());

            return
                (Func<TestCase, ValueTask<TestExecutionResult>>)
                executeSpecificMethodGeneric.CreateDelegate(typeof(Func<TestCase, ValueTask<TestExecutionResult>>));
        }

        private static ValueTask<TestExecutionResult> ExecuteSpecificTestAsync<TClass, TActResult>(
            TestCase testCaseNonGeneric)
        {
            return
                new ValueTask<TestExecutionResult>(ExecuteSpecificTest((TestCase<TClass, TActResult>) testCaseNonGeneric));
        }


        private static TestExecutionResult ExecuteSpecificTest<TClass, TActResult>(
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
                return new TestExecutionResult(testCase, TestExecutionOutcome.Inconclusive, ex, sw.Elapsed);
            }

            try
            {
                testCase.Assert(result);
            }
            catch (Exception ex)
            {
                return new TestExecutionResult(testCase, TestExecutionOutcome.Inconclusive, ex, sw.Elapsed);
            }

            return new TestExecutionResult(testCase, TestExecutionOutcome.Success, null, sw.Elapsed);
        }
    }
}