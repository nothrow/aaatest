using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace aaatest.executor.testadapter
{
    /// <summary>
    /// Used as parameter to discoverer
    /// </summary>
    internal class DiscoverySink : ITestCaseDiscoverySink
    {
        private readonly ConcurrentBag<TestCase> _testCases = new ConcurrentBag<TestCase>();
        /// <inheritdoc />
        public void SendTestCase(TestCase discoveredTest)
        {
            _testCases.Add(discoveredTest);
        }

        public IEnumerable<TestCase> Tests => _testCases;
    }
}