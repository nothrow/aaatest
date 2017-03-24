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
                    Where(method => method.ReturnType == typeof(TestCase)).
                    Select(TraceMethodFound).
                    Select(method => (method: method, testCase: (TestCase) method.Invoke(new T(), null))).
                    Select(test => test.testCase.SetName(test.method.Name));
        }

        private MethodInfo TraceMethodFound(MethodInfo method)
        {
            _tracer.Debug($"Found {method.Name}");
            return method;
        }
    }
}