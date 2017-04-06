using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace aaatest.executor.testadapter
{
    internal static class TestCaseExtensions
    {
        public static TestCase ConvertToVsTest(this framework.TestCase tc, string sourceAssembly)
        {
            return new TestCase(tc.Identifier, AaaTestExecutor.ExecutorUri, sourceAssembly)
            {
                CodeFilePath = tc.CallerInformation.SourceFilePath,
                LineNumber = tc.CallerInformation.SourceLineNumber,
                DisplayName  = $"{tc.Identifier} - {tc.CallerInformation.SourceFilePath}:{tc.CallerInformation.SourceLineNumber} {sourceAssembly}",
            };
        }

        public static TestResult ConvertToVsResult(this TestExecutionResult result, TestCase testCase)
        {
            return new TestResult(testCase)
            {
                Duration = result.ExecutionTime,
                Outcome = result.Outcome.ToVsResultOutcome(),
                DisplayName = result.TestCase.Name,
                ErrorMessage = result.Exception?.Message,
                ErrorStackTrace = result.Exception?.StackTrace
            };
        }

        public static TestOutcome ToVsResultOutcome(this TestExecutionOutcome outcome)
        {
            switch (outcome)
            {
                case TestExecutionOutcome.Invalid:
                    return TestOutcome.NotFound;
                case TestExecutionOutcome.Failure:
                    return TestOutcome.Failed;
                case TestExecutionOutcome.Success:
                    return TestOutcome.Passed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null);
            }

        }
    }
}