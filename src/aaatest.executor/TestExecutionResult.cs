using System;
using aaatest.framework;

namespace aaatest.executor
{
    /// <summary>
    /// Contains information about the test resume
    /// </summary>
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

        /// <summary>
        /// Which test case is this result for
        /// </summary>
        public TestCase TestCase { get; }
        /// <summary>
        /// What was the outcome of the test
        /// </summary>
        public TestExecutionOutcome Outcome { get; }
        /// <summary>
        /// Exception thrown in case of Invalid/Failed test
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// How long did the execution took
        /// </summary>
        public TimeSpan ExecutionTime { get; }
    }
}