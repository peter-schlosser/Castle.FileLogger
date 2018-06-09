// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Internal
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Castle.Extensions.Logging.FileLogger
{
    /// <summary>
    /// Represents a type that creates instances of <see cref="CastleBatchingLogger"/>.
    /// </summary>
    public abstract class CastleBatchingLoggerProvider : ILoggerProvider
    {
        private readonly List<CastleLogMessage> _currentBatch = new List<CastleLogMessage>();
        private readonly TimeSpan _interval;
        private readonly int? _queueSize;
        private readonly int? _batchSize;
        //private readonly IDisposable _optionsChangeToken;

        private BlockingCollection<CastleLogMessage> _messageQueue;
        private Task _outputTask;
        private CancellationTokenSource _cancellationTokenSource;

        protected CastleBatchingLoggerProvider(IOptions<CastleBatchingLoggerOptions> options)
        {
            var loggerOptions = options.Value;
            if (loggerOptions.BatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
            }
            if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
            }

            _interval = loggerOptions.FlushPeriod;
            _batchSize = loggerOptions.BatchSize;
            _queueSize = loggerOptions.BackgroundQueueSize;

            //_optionsChangeToken = options.OnChange(UpdateOptions);
            UpdateOptions(options.Value);
        }

        public bool IsEnabled { get; private set; }

        private void UpdateOptions(CastleBatchingLoggerOptions options)
        {
            var oldIsEnabled = IsEnabled;
            IsEnabled = options.IsEnabled;
            if (oldIsEnabled != IsEnabled)
            {
                if (IsEnabled)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        protected abstract Task WriteMessagesAsync(IEnumerable<CastleLogMessage> messages, CancellationToken token);

        private async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize ?? int.MaxValue;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                if (_currentBatch.Count > 0)
                {
                    try
                    {
                        await WriteMessagesAsync(_currentBatch, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        // ignored
                    }

                    _currentBatch.Clear();
                }

                await IntervalAsync(_interval, _cancellationTokenSource.Token);
            }
        }

        protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        internal void AddMessage(DateTimeOffset timestamp, string message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(new CastleLogMessage { Timestamp = timestamp, Message = message }, _cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token canceled or CompleteAdding called
                }
            }
        }

        private void Start()
        {
            _messageQueue = _queueSize == null ?
                new BlockingCollection<CastleLogMessage>(new ConcurrentQueue<CastleLogMessage>()) :
                new BlockingCollection<CastleLogMessage>(new ConcurrentQueue<CastleLogMessage>(), _queueSize.Value);

            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Factory.StartNew<Task>(
                ProcessLogQueue,
                null,
                TaskCreationOptions.LongRunning);
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(_interval);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CastleBatchingLogger(this, categoryName);
        }
    }
}
