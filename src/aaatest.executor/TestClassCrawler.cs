using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aaatest.framework;

namespace aaatest.executor
{
    public class TestClassCrawler
    {
        private readonly ITracer _tracer;
        private readonly Type _type;

        public TestClassCrawler(ITracer tracer, Type type)
        {
            _tracer = tracer;
            _type = type;
        }


        public IEnumerable<TestCase> EnumerateTests()
        {
            _tracer.Debug($"Enumerating tests in {_type.FullName}");

            return
                _type.GetMethods(BindingFlags.Instance | BindingFlags.Public).
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

        protected virtual IEnumerable<TestWithMethodInfo> DiscoverTests(MethodInfo method)
        {
            IEnumerable<TestCase> testCases;

            if (method.ReturnType == typeof(TestCase))
                testCases = new[] { (TestCase)method.Invoke(Activator.CreateInstance(_type), null) };
            else if (method.ReturnType == typeof(IEnumerable<TestCase>))
                testCases = (IEnumerable<TestCase>)method.Invoke(Activator.CreateInstance(_type), null);
            else
                throw new InvalidOperationException();

            return testCases.
                Where(testCase => !testCase.HarnessOnly).
                Select(testCase => new TestWithMethodInfo(method, testCase));
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

        private static TestWithMethodInfo SetTestName(TestWithMethodInfo test)
        {
            return new TestWithMethodInfo(test.Method, test.TestCase.SetName(test.Method.Name));
        }

        protected struct TestWithMethodInfo
        {
            public TestWithMethodInfo(MethodInfo method, TestCase testCase)
            {
                Method = method;
                TestCase = testCase;
            }

            public MethodInfo Method { get; }
            public TestCase TestCase { get; }
        }
    }

    public class TestClassCrawler<T> : TestClassCrawler where T : new()
    {
        public TestClassCrawler(ITracer tracer)
            : base(tracer, typeof(T))
        {
        }
    }
}