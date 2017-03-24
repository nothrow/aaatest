using System;

namespace aaatest.framework
{
    public interface ITracer
    {
        void Debug(string message);
        void Info(string message);
        void Error(string message);
        IDisposable MeasureTrace(string startMessage, Func<TimeSpan, string> endMessage);
    }
}