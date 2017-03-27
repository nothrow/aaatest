using System;
using aaatest.framework;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace aaatest.executor.testadapter
{
    internal class MessageLoggerTracer : TracerBase
    {
        private readonly IMessageLogger _logger;

        public MessageLoggerTracer([NotNull] IMessageLogger logger)
        {
            Check.NotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <inheritdoc />
        protected override void Write(Severity severity, string message)
        {
            switch (severity)
            {
                case Severity.Debug:
                    _logger.SendMessage(TestMessageLevel.Informational, message);
                    break;
                case Severity.Info:
                    _logger.SendMessage(TestMessageLevel.Warning, message);
                    break;
                case Severity.Error:
                    _logger.SendMessage(TestMessageLevel.Error, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }
    }
}