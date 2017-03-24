using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aaatest.framework;

namespace aaatest.executor
{
    public class TestClassCrawler<T> where T : new()
    {
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
                    SelectMany(DiscoverTests).
                    Select(SetTestName);
        }

        private static TestCase SetTestName((MethodInfo method, TestCase testCase) test)
        {
            return test.testCase.SetName(test.method.Name);
        }

        private static IEnumerable<(MethodInfo method, TestCase testCase)> DiscoverTests(MethodInfo method)
        {
            IEnumerable<TestCase> testCases;
            if (method.ReturnType == typeof(TestCase))
                testCases = new[] {(TestCase) method.Invoke(new T(), null)};
            else if (method.ReturnType == typeof(IEnumerable<TestCase>))
                testCases = (IEnumerable<TestCase>) method.Invoke(new T(), null);
            else
                throw new InvalidOperationException();
            return testCases.Select(testCase => (method: method, testCase: testCase));
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