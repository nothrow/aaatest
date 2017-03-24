using System;
using aaatest.framework;

namespace aaatest.executor
{
    public sealed class TestExecutionResult
    {
        internal TestExecutionResult(TestCase testCase, TestExecutionOutcome outcome, Exception exception,
            TimeSpan executionTime)
        {
            Outcome = outcome;
            Exception = exception;
            ExecutionTime = executionTime;
            TestCase = testCase;
        }

        public TestCase TestCase { get; }
        public TestExecutionOutcome Outcome { get; }
        public Exception Exception { get; }
        public TimeSpan ExecutionTime { get; }
    }
}