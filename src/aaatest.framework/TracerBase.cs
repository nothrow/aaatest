using System;
using System.Diagnostics;

namespace aaatest.framework
{
    public abstract class TracerBase : ITracer
    {
        /// <inheritdoc />
        public void Debug(string message)
        {
            Write(Severity.Debug, message);
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            Write(Severity.Info, message);
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            Write(Severity.Error, message);
        }

        /// <inheritdoc />
        public IDisposable MeasureTrace(string startMessage, Func<TimeSpan, string> endMessage)
        {
            Write(Severity.Debug, startMessage);
            var sw = Stopwatch.StartNew();
            return new OnFinished(sw, time => Write(Severity.Debug, endMessage(time)));
        }

        protected abstract void Write(Severity severity, string message);

        protected enum Severity
        {
            Debug,
            Info,
            Error
        }

        private class OnFinished : IDisposable
        {
            private readonly Action<TimeSpan> _finished;
            private readonly Stopwatch _sw;

            public OnFinished(Stopwatch sw, Action<TimeSpan> finished)
            {
                _sw = sw;
                _finished = finished;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                _sw.Stop();
                _finished(_sw.Elapsed);
            }
        }
    }
}