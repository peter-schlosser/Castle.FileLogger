// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Castle.Extensions.Logging.FileLogger
{
    /// <summary>
    /// Represents a type used to perform logging to disk file.
    /// </summary>
    public class CastleBatchingLogger : ILogger
    {
        private readonly CastleBatchingLoggerProvider _provider;
        private readonly string _category;

        public CastleBatchingLogger(CastleBatchingLoggerProvider loggerProvider, string categoryName)
        {
            _provider = loggerProvider;
            _category = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled;
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId unused, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var sb = new StringBuilder();
                sb.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
                //sb.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
                sb.Append($@" [{logLevel.ToString()}]");
                sb.Append($@" {_category}:");
                sb.AppendLine($@" {formatter(state, exception)}");
                if (exception != null)
                {
                    sb.AppendLine(exception.ToString());
                }
                _provider.AddMessage(timestamp, sb.ToString());
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.Now, logLevel, eventId, state, exception, formatter);
        }
    }
}
