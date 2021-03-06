// Copyright (c) Peter Schlosser. All rights reserved.  Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// Copyright (c) .NET Foundation. All rights reserved.  Reference namespace Microsoft.Extensions.Logging.AzureAppServices.Test

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Castle.Extensions.Logging.FileLogger.Test
{
    public class FileLoggerTests : IDisposable
    {
        DateTimeOffset _timestamp = new DateTimeOffset(2016, 05, 04, 03, 02, 01, TimeSpan.Zero);

        public FileLoggerTests()
        {
            TempPath = Path.GetTempFileName() + "_";
        }

        public string TempPath { get; protected set; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(TempPath))
                {
                    Directory.Delete(TempPath, true);
                }
            }
            catch
            {
                // ignored
            }
        }

        [Fact]
        public async Task WritesToTextFile()
        {
            var provider = new TestFileLoggerProvider(TempPath);
            var logger = (CastleBatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestamp, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestamp.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(
                "2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + Environment.NewLine +
                "2016-05-04 04:02:01.000 +00:00 [Error] Cat: Error message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160504.txt")));
        }

        [Fact]
        public async Task RollsTextFile()
        {
            var provider = new TestFileLoggerProvider(TempPath);
            var logger = (CastleBatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;

            logger.Log(_timestamp, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
            logger.Log(_timestamp.AddDays(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            Assert.Equal(
                "2016-05-04 03:02:01.000 +00:00 [Information] Cat: Info message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160504.txt")));

            Assert.Equal(
                "2016-05-05 03:02:01.000 +00:00 [Error] Cat: Error message" + Environment.NewLine,
                File.ReadAllText(Path.Combine(TempPath, "LogFile.20160505.txt")));
        }

        [Fact]
        public async Task RespectsMaxFileCount()
        {
            Directory.CreateDirectory(TempPath);
            File.WriteAllText(Path.Combine(TempPath, "randomFile.txt"), "Text");

            var provider = new TestFileLoggerProvider(TempPath, maxRetainedFiles: 5);
            var logger = (CastleBatchingLogger)provider.CreateLogger("Cat");

            await provider.IntervalControl.Pause;
            var timestamp = _timestamp;

            for (int i = 0; i < 10; i++)
            {
                logger.Log(timestamp, LogLevel.Information, 0, "Info message", null, (state, ex) => state);
                logger.Log(timestamp.AddHours(1), LogLevel.Error, 0, "Error message", null, (state, ex) => state);

                timestamp = timestamp.AddDays(1);
            }

            provider.IntervalControl.Resume();
            await provider.IntervalControl.Pause;

            var actualFiles = new DirectoryInfo(TempPath)
                .GetFiles()
                .Select(f => f.Name)
                .OrderBy(f => f)
                .ToArray();

            Assert.Equal(6, actualFiles.Length);
            Assert.Equal(new[] {
                "LogFile.20160509.txt",
                "LogFile.20160510.txt",
                "LogFile.20160511.txt",
                "LogFile.20160512.txt",
                "LogFile.20160513.txt",
                "randomFile.txt"
            }, actualFiles);
        }
    }
}
