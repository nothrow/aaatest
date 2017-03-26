using aaatest.executor.testadapter;

namespace aaatest.Tests
{
    internal class TestingSubject
    {
        private AaaTestDiscoverer atd;

        public int AddTwoValues(int a, int b)
        {
            return a + b;
        }
    }
}