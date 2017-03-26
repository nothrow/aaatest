using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aaatest.framework;

namespace aaatest.executor
{
    public class TestClassCrawler<T> where T : new()
    {
        private struct TestWithMethodInfo
        {
            public TestWithMethodInfo(MethodInfo method, TestCase testCase)
            {
                Method = method;
                TestCase = testCase;
            }

            public MethodInfo Method { get; }
            public TestCase TestCase { get; }
        }

        private readonly ITracer _tracer;

        public TestClassCrawler(ITracer tracer)
        {
            _tracer = tracer;
        }

        public IEnumerable<TestCase> EnumerateTests()
        {
            _tracer.Debug($"Enumerating tests in {typeof(T)}");

            return
                typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public).
                    Where(IsUnitTestMethod).
                    Select(TraceMethodFound).
                    Select(DiscoverTests).
                    SelectMany(SetTestIdentifier).
                    Select(SetTestName).
                    // finally
                    Select(tm => tm.TestCase);
        }

        private static IEnumerable<TestWithMethodInfo> SetTestIdentifier(IEnumerable<TestWithMethodInfo> tests)
        {
            return tests.Select((test, i) => new TestWithMethodInfo(test.Method, test.TestCase.SetTestIdentifier(
                    $"{test.Method.DeclaringType.FullName}.{test.Method.Name}.#{i}"
                ))
            );
        }

        private static TestWithMethodInfo SetTestName(TestWithMethodInfo test)
        {
            return new TestWithMethodInfo(test.Method, test.TestCase.SetName(test.Method.Name));
        }

        private static IEnumerable<TestWithMethodInfo> DiscoverTests(MethodInfo method)
        {
            IEnumerable<TestCase> testCases;

            if (method.ReturnType == typeof(TestCase))
                testCases = new[] {(TestCase) method.Invoke(new T(), null)};
            else if (method.ReturnType == typeof(IEnumerable<TestCase>))
                testCases = (IEnumerable<TestCase>) method.Invoke(new T(), null);
            else
                throw new InvalidOperationException();

            return testCases.Select(testCase => new TestWithMethodInfo(method, testCase));
        }


        private static bool IsUnitTestMethod(MethodInfo method)
        {
            return
                method.ReturnType == typeof(TestCase) || // single test
                method.ReturnType == typeof(IEnumerable<TestCase>); // multiple tests
        }

        private MethodInfo TraceMethodFound(MethodInfo method)
        {
            _tracer.Debug($"Discovering {method.Name}");
            return method;
        }
    }
}