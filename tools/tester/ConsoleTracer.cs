using System;
using aaatest.framework;

namespace tester
{
    internal class ConsoleTracer : TracerBase
    {
        private static readonly Lazy<ConsoleTracer> _instance = new Lazy<ConsoleTracer>(() => new ConsoleTracer());
        public static ConsoleTracer Instance => _instance.Value;

        /// <inheritdoc />
        protected override void Write(Severity severity, string message)
        {
            Console.WriteLine($"{severity:G}: {message}");
        }
    }
}