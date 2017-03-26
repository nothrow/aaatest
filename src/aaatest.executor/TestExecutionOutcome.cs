namespace aaatest.executor
{
    public enum TestExecutionOutcome
    {
        /// <summary>
        /// Test execution was not valid (exception was thrown before the assertion)
        /// </summary>
        Invalid,
        /// <summary>
        /// Assertion failed
        /// </summary>
        Failure,
        /// <summary>
        /// Assertion was successful
        /// </summary>
        Success
    }
}