using System;
using aaatest.executor;
using FluentAssertions.Execution;

namespace tester
{
    internal class FluentAssertionsPlugin : IFailureDetector
    {
        private static readonly Lazy<FluentAssertionsPlugin> _instance =
            new Lazy<FluentAssertionsPlugin>(() => new FluentAssertionsPlugin());

        public static FluentAssertionsPlugin Instance => _instance.Value;

        public bool IsAssertionFailure(Exception ex)
        {
            return ex is AssertionFailedException;
        }
    }
}