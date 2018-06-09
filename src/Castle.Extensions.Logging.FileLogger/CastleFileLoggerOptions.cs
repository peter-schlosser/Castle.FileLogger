// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices
using System;

namespace Castle.Extensions.Logging.FileLogger
{
    /// <summary>
    /// Options for Azure diagnostics file logging.
    /// </summary>
    public class CastleFileLoggerOptions : CastleBatchingLoggerOptions
    {
        private int? _fileSizeLimit = 10 * 1024 * 1024;
        private int? _retainedFileCountLimit = 2;
        private string _fileName = "log-";
        private string _logDirectory = "Logs";

        /// <summary>
        /// Gets or sets a strictly positive value representing the maximum log size in bytes or null for no limit.
        /// Once the log is full, no more messages will be appended.
        /// Defaults to <c>10MB</c>.
        /// </summary>
        public int? FileSizeLimit
        {
            get { return _fileSizeLimit; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FileSizeLimit)} must be positive.");
                }
                _fileSizeLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets a strictly positive value representing the maximum retained file count or null for no limit.
        /// Defaults to <c>2</c>.
        /// </summary>
        public int? RetainedFileCountLimit
        {
            get { return _retainedFileCountLimit; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(RetainedFileCountLimit)} must be positive.");
                }
                _retainedFileCountLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the filename prefix used to name daily log files.
        /// Defaults to <c>log-</c>.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(nameof(value));
                }
                _fileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the folder into which log files are written.
        /// Defaults to <c>Logs</c>.
        /// </summary>
        public string LogDirectory
        {
            get { return _logDirectory; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(nameof(value));
                }
                _logDirectory = value;
            }
        }
    }
}
