// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Test

using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Castle.Extensions.Logging.FileLogger.Test
{
    internal class TestFileLoggerProvider : CastleFileLoggerProvider
    {
        internal ManualIntervalControl IntervalControl { get; } = new ManualIntervalControl();

        public TestFileLoggerProvider(
            string path,
            string fileName = "LogFile.",
            int maxFileSize = 32_000,
            int maxRetainedFiles = 100)
            : base(new OptionsWrapper<CastleFileLoggerOptions>(new CastleFileLoggerOptions()
            {
                LogDirectory = path,
                FileName = fileName,
                FileSizeLimit = maxFileSize,
                RetainedFileCountLimit = maxRetainedFiles,
                IsEnabled = true
            }))
        {
        }

        protected override Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return IntervalControl.IntervalAsync();
        }
    }
}
