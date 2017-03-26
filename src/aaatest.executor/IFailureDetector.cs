using System;

namespace aaatest.executor
{
    /// <summary>
    /// Used for detecting if the exception is assertion failure, or something else.
    /// </summary>
    public interface IFailureDetector
    {
        /// <summary>
        /// Returns true, if the exception is assertion failure, false otherwise (uncaught exception).
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool IsAssertionFailure(Exception ex);
    }
}